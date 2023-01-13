using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    [Poolable]
    public sealed class SettingSetFloatCommand : Command<Setting<float>, float>
    {
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute(Setting<float> setting, float value)
        {
            SettingsController.SetSetting(setting, value);
        }
    }
}