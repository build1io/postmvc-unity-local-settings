using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    [Poolable]
    public sealed class SettingSetStringCommand : Command<string, Setting<string>>
    {
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute(string value, Setting<string> setting)
        {
            SettingsController.SetSetting(setting, value);
        }
    }
}