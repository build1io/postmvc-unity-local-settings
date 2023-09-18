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
            
            Dispatcher.AddListener(SettingsEvent.LoadResult, OnSettingsLoaded);
            
            SettingsController.LoadDeviceSettings();
        }

        private void OnSettingsLoaded(SettingsResult result)
        {
            if (result.settingsType != SettingType.Device)
                return;
            
            Dispatcher.RemoveListener(SettingsEvent.LoadResult, OnSettingsLoaded);
            
            if (result.isError)
                Fail(result.ToException());
            else
                Release();
        }
    }
}