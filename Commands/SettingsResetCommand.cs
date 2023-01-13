using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    public sealed class SettingsResetCommand : Command
    {
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute()
        {
            SettingsController.Reset();
        }
    }
}