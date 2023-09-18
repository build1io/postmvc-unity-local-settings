using System.Collections.Generic;
using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Events;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    public sealed class InitializeAndLoadDeviceSettingsCommand : Command<IEnumerable<Setting>>
    {
        [Inject] public IEventDispatcher    Dispatcher         { get; set; }
        [Inject] public ISettingsController SettingsController { get; set; }
        
        public override void Execute(IEnumerable<Setting> settings)
        {
            if (!SettingsController.Initialized)
                SettingsController.Initialize(settings);
            
            if (SettingsController.DeviceSettingsLoaded)
                return;
            
            Retain();
            
            Dispatcher.AddListenerOnce(SettingsEvent.LoadResult, OnSettingsLoaded);
            
            SettingsController.LoadDeviceSettings();
        }
        
        private void OnSettingsLoaded(SettingsResult result)
        {
            if (result.settingsType != SettingType.Device)
                return;
            
            if (result.isError)
                Fail(result.ToException());
            else
                Release();
        }
    }
}