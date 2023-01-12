using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        public bool IsLoaded { get; private set; }

        private IReadOnlyList<Setting>     _settings;
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
         * Configuration.
         */

        public void SetSettingsSet(IReadOnlyList<Setting> settings)
        {
            Log.Debug("Setting settings set...");

            if (!CheckNotLoaded())
                return;

            _settings = settings;

            Log.Debug("Setting set set");
        }

        public void SetSettingsFolder(string folder)
        {
            Log.Debug(f => $"Settings settings folder \"{f}\"...", folder);

            if (!CheckNotLoaded())
                return;

            _settingsFolder = folder;

            Log.Debug(p => $"Settings folder set: {p}", _settingsFolder);
        }

        /*
         * Loading.
         */

        public void Load(IReadOnlyList<Setting> settings, string folder)
        {
            if (!CheckNotLoaded())
                return;

            SetSettingsSet(settings);
            SetSettingsFolder(folder);
            Load();
        }

        public void Load(IReadOnlyList<Setting> settings)
        {
            if (!CheckNotLoaded())
                return;

            SetSettingsSet(settings);
            Load();
        }

        public void Load()
        {
            if (!CheckNotLoaded())
                return;

            if (_settings == null)
            {
                Dispatcher.Dispatch(SettingsEvent.LoadFail, new Exception("Settings set not specified"));
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
                    Log.Debug(() => setting.key + ": " + setting.defaultValue);

                    _settingsValues[setting.key] = setting.defaultValue;
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
            if (!IsLoaded)
            {
                Log.Error("Settings not loaded.");
                return;
            }

            try
            {
                _settings = null;
                _settingsValues = null;
                _settingsFilePath = null;
            }
            catch (Exception exception)
            {
                Dispatcher.Dispatch(SettingsEvent.UnloadFail, exception);
                return;
            }

            IsLoaded = false;
            Dispatcher.Dispatch(SettingsEvent.UnloadSuccess);
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
         * Management.
         */

        public T GetSetting<T>(Setting<T> setting)
        {
            if (!IsLoaded)
                throw new Exception("Settings not loaded");
            
            if (_settingsValues.TryGetValue(setting.key, out var value))
                return (T)value;
            
            return setting.defaultValue;
        }

        public void SetSetting<T>(Setting<T> setting, T value)
        {
            if (_settingsValues.ContainsKey(setting.key) && EqualityComparer<T>.Default.Equals((T)_settingsValues[setting.key], value))
                return;

            _settingsDirty = true;
            _settingsValues[setting.key] = value;
            Dispatcher.Dispatch(SettingsEvent.SettingChanged, setting);
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
                _settingsValues[setting.key] = setting.defaultValue;

            if (File.Exists(_settingsFilePath))
                File.Delete(_settingsFilePath);

            Log.Debug("Resetting ok");

            Dispatcher.Dispatch(SettingsEvent.Reset);
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