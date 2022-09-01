using System;
using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Events;
using Build1.PostMVC.Core.MVCS.Injection;
using Build1.PostMVC.Unity.App.Modules.Logging;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    public sealed class SettingsUnloadCommand : Command
    {
        [Log(LogLevel.Warning)] public ILog                Log                { get; set; }
        [Inject]                public IEventDispatcher    Dispatcher         { get; set; }
        [Inject]                public ISettingsController SettingsController { get; set; }

        public override void Execute()
        {
            if (!SettingsController.IsLoaded)
            {
                Log.Debug("Settings not loaded");
                return;
            }
            
            Log.Debug("Unloading settings...");

            Retain();

            Dispatcher.AddListener(SettingsEvent.UnloadSuccess, OnSuccess);
            Dispatcher.AddListener(SettingsEvent.UnloadFail, OnFail);

            SettingsController.Unload();
        }

        private void OnSuccess()
        {
            Log.Debug("Done");

            Dispatcher.RemoveListener(SettingsEvent.UnloadSuccess, OnSuccess);
            Dispatcher.RemoveListener(SettingsEvent.UnloadFail, OnFail);

            Release();
        }

        private void OnFail(Exception exception)
        {
            Log.Error(exception);

            Dispatcher.RemoveListener(SettingsEvent.UnloadSuccess, OnSuccess);
            Dispatcher.RemoveListener(SettingsEvent.UnloadFail, OnFail);

            Fail(exception);
        }
    }
}