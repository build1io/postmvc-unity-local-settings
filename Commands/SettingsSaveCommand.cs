using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    [Poolable]
    public sealed class SettingsSaveCommand : Command<bool>
    {
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute(bool force)
        {
            SettingsController.Save(force);
        }
    }
}