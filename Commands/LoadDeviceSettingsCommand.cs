using System;
using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Events;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    public sealed class LoadDeviceSettingsCommand : Command
    {
        [Inject] public IEventDispatcher    Dispatcher         { get; set; }
        [Inject] public ISettingsController SettingsController { get; set; }
        
        public override void Execute()
        {
            if (SettingsController.DeviceSettingsLoaded)
                return;

            Retain();
            
            Dispatcher.AddListenerOnce(SettingsEvent.LoadResult, OnSettingsLoaded);
            
            SettingsController.LoadDeviceSettings();
        }

        private void OnSettingsLoaded(SettingType type, Exception exception)
        {
            if (type != SettingType.Device)
                return;
            
            if (exception != null)
                Fail(exception);
            else
                Release();
        }
    }
}