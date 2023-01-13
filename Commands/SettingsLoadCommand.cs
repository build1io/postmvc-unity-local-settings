using System;
using System.Collections.Generic;
using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Events;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    public sealed class SettingsLoadCommand : Command<IEnumerable<Setting>>
    {
        [Inject] public IEventDispatcher    Dispatcher         { get; set; }
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute(IEnumerable<Setting> existingSettings)
        {
            Retain();

            Dispatcher.AddListener(SettingsEvent.LoadSuccess, OnSuccess);
            Dispatcher.AddListener(SettingsEvent.LoadFail, OnFail);

            SettingsController.Load(existingSettings);
        }

        private void OnSuccess()
        {
            Dispatcher.RemoveListener(SettingsEvent.LoadSuccess, OnSuccess);
            Dispatcher.RemoveListener(SettingsEvent.LoadFail, OnFail);

            Release();
        }

        private void OnFail(Exception exception)
        {
            Dispatcher.RemoveListener(SettingsEvent.LoadSuccess, OnSuccess);
            Dispatcher.RemoveListener(SettingsEvent.LoadFail, OnFail);

            Fail(exception);
        }
    }
}