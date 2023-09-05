using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Build1.PostMVC.Core.MVCS.Events;
using Build1.PostMVC.Core.MVCS.Injection;
using Build1.PostMVC.Unity.App.Modules.App;
using Build1.PostMVC.Unity.App.Modules.Logging;
using Newtonsoft.Json;

namespace Build1.PostMVC.Unity.Settings.Impl
{
    internal sealed class SettingsController : ISettingsController
    {
        public const string SettingsFileName = "settings.json";

        [Log(LogLevel.Warning)] public ILog             Log           { get; set; }
        [Inject]                public IEventDispatcher Dispatcher    { get; set; }
        [Inject]                public IAppController   AppController { get; set; }

        public IEnumerable<Setting> ExistingSettings => _settings;
        public bool                 IsLoaded         { get; private set; }

        private IEnumerable<Setting>       _settings;
        private string                     _settingsFolder;
        private Dictionary<string, object> _settingsValues;
        private string                     _settingsFilePath;
        private bool                       _settingsDirty;

        [PostConstruct]
        public void PostConstruct()
        {
            Dispatcher.AddListener(AppEvent.Pause, OnAppPause);
            Dispatcher.AddListener(AppEvent.Restarting, OnAppRestarting);
            Dispatcher.AddListener(AppEvent.Quitting, OnAppQuitting);
        }

        [PreDestroy]
        public void PreDestroy()
        {
            Dispatcher.RemoveListener(AppEvent.Pause, OnAppPause);
            Dispatcher.RemoveListener(AppEvent.Restarting, OnAppRestarting);
            Dispatcher.RemoveListener(AppEvent.Quitting, OnAppQuitting);
        }
        
        /*
         * Setup.
         */

        public void SetUserId(string userId)
        {
            Log.Debug(f => $"Setting userId: \"{f}\" ...", userId);

            if (!CheckNotLoaded())
            {
                Log.Error("Settings already loaded. In order to change userId unload settings first.");
                return;
            }

            _settingsFolder = userId;

            Log.Debug(p => $"UserId set: {p}", _settingsFolder);
        }

        /*
         * Loading.
         */

        public void Load(IEnumerable<Setting> settings)
        {
            Unload();
            
            _settings = settings;
            
            Load();
        }
        
        public void Load(IEnumerable<Setting> settings, string userId)
        {
            Unload();
            
            _settings = settings;
            _settingsFolder = userId;
            
            Load();
        }

        private void Load()
        {
            if (_settings == null || !_settings.Any())
            {
                Dispatcher.Dispatch(SettingsEvent.LoadFail, new Exception("Settings set can't be null or empty"));
                return;
            }

            _settingsValues = new Dictionary<string, object>();
            _settingsFilePath = string.IsNullOrEmpty(_settingsFolder)
                                    ? Path.Combine(AppController.PersistentDataPath, SettingsFileName)
                                    : Path.Combine(AppController.PersistentDataPath, _settingsFolder, SettingsFileName);

            Log.Debug(p => $"Settings file path: {p}", _settingsFilePath);

            try
            {
                LoadImpl();
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                Dispatcher.Dispatch(SettingsEvent.LoadFail, exception);
                return;
            }

            IsLoaded = true;
            Dispatcher.Dispatch(SettingsEvent.LoadSuccess);
        }

        private void LoadImpl()
        {
            Log.Debug("Loading...");

            if (!File.Exists(_settingsFilePath))
            {
                Log.Debug("Settings file doesn't exist. Initializing default settings.");

                foreach (var setting in _settings)
                {
                    Log.Debug(() => setting.key + ": " + setting.DefaultValue);

                    _settingsValues[setting.key] = setting.DefaultValue;
                }

                return;
            }

            Log.Debug("Reading file...");

            var json = File.ReadAllText(_settingsFilePath);

            Log.Debug(j => $"Json: {j}", json);
            Log.Debug("Parsing...");

            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            foreach (var setting in _settings)
            {
                if (!settings.TryGetValue(setting.key, out var value))
                    continue;

                Log.Debug(() => setting.key + ": " + value);

                if (bool.TryParse(value, out var valueBool))
                    _settingsValues[setting.key] = valueBool;
                else if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueInt))
                    _settingsValues[setting.key] = valueInt;
                else if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var valueFloat))
                    _settingsValues[setting.key] = valueFloat;
            }

            Log.Debug("Loading ok");
        }

        /*
         * Unloading.
         */

        public void Unload()
        {
            _settings = null;
            _settingsValues = null;
            _settingsFilePath = null;

            IsLoaded = false;
        }

        /*
         * Save.
         */

        public void Save(bool force)
        {
            Log.Debug("Saving...");

            if (!IsLoaded)
            {
                Log.Debug("Settings not loaded.");
                return;
            }

            if (!_settingsDirty && !force)
            {
                Log.Debug("Saving prevented. Settings not dirty.");
                return;
            }

            // We put in try/catch as this operation is performed when app is shutting down or put on pause.
            // If it'll fail with an exception, other operations must not be interrupted.   
            try
            {
                var json = JsonConvert.SerializeObject(_settingsValues);

                Log.Debug(() => "Json: " + json);

                File.WriteAllText(_settingsFilePath, json);

                _settingsDirty = false;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                Dispatcher.Dispatch(SettingsEvent.SaveFail, exception);
                return;
            }

            Log.Debug("Saving ok");
        }

        /*
         * Resetting.
         */

        public void Reset()
        {
            Log.Debug("Resetting...");

            if (!CheckNotLoaded())
                return;

            foreach (var setting in _settings)
                _settingsValues[setting.key] = setting.DefaultValue;

            if (File.Exists(_settingsFilePath))
                File.Delete(_settingsFilePath);

            Log.Debug("Resetting ok");

            Dispatcher.Dispatch(SettingsEvent.Reset);
        }
        
        /*
         * Management.
         */

        public T GetSetting<T>(Setting<T> setting) where T : struct
        {
            if (!IsLoaded)
                throw new Exception("Settings not loaded");

            return GetSettingValue(setting);
        }

        public void SetSetting<T>(Setting<T> setting, T value) where T : struct
        {
            if (_settingsValues.ContainsKey(setting.key) && EqualityComparer<T>.Default.Equals(GetSettingValue(setting), value))
                return;

            _settingsDirty = true;
            _settingsValues[setting.key] = value;
            Dispatcher.Dispatch(SettingsEvent.SettingChanged, setting);
        }

        private T GetSettingValue<T>(Setting<T> setting) where T : struct
        {
            if (_settingsValues.TryGetValue(setting.key, out var value))
            {
                if (typeof(T).IsEnum)
                    return (T)Enum.ToObject(typeof(T), Convert.ToInt32(value));
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return setting.DefaultValue;
        }

        /*
         * Helpers.
         */

        private bool CheckNotLoaded()
        {
            if (!IsLoaded)
                return true;

            Log.Error("Settings already loaded.");
            return false;
        }

        /*
         * Event Handlers.
         */

        private void OnAppPause(bool paused)
        {
            if (paused)
                Save(false);
        }

        private void OnAppRestarting() { Save(false); }
        private void OnAppQuitting()   { Save(false); }
    }
}