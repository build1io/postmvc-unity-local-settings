using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Events;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    public sealed class LoadUserSettingsCommand : Command<string>
    {
        [Inject] public IEventDispatcher    Dispatcher         { get; set; }
        [Inject] public ISettingsController SettingsController { get; set; }
        
        public override void Execute(string userId)
        {
            if (SettingsController.UserSettingsLoaded && SettingsController.UserId == userId)
                return;
            
            Retain();
            
            Dispatcher.AddListenerOnce(SettingsEvent.LoadResult, OnSettingsLoad);

            SettingsController.LoadUserSettings(userId);
        }

        private void OnSettingsLoad(SettingsResult result)
        {
            if (result.settingsType != SettingType.User || SettingsController.UserId != Param01)
                return;
            
            if (result.isError)
                Fail(result.ToException());
            else
                Release();
        }
    }
}