using System.Collections.Generic;
using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    public sealed class InitializeSettingsCommand : Command<IEnumerable<Setting>>
    {
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute(IEnumerable<Setting> settings)
        {
            SettingsController.Initialize(settings);
        }
    }
}