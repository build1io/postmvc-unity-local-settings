using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    [Poolable]
    public sealed class SettingSetIntCommand : Command<int, Setting<int>>
    {
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute(int value, Setting<int> setting)
        {
            SettingsController.SetSetting(setting, value);
        }
    }
}