using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    [Poolable]
    public sealed class SettingSetIntCommand : Command<Setting<int>, int>
    {
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute(Setting<int> setting, int value)
        {
            SettingsController.SetSetting(setting, value);
        }
    }
}