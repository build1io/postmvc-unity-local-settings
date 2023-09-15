using System;
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

        private void OnSettingsLoad(SettingType type, Exception exception)
        {
            if (type != SettingType.User || SettingsController.UserId != Param01)
                return;
            
            if (exception != null)
                Fail(exception);
            else
                Release();
        }
    }
}