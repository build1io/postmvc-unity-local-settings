using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    [Poolable]
    public sealed class SettingSetFloatCommand : Command<float, Setting<float>>
    {
        [Inject] public ISettingsController SettingsController { get; set; }

        public override void Execute(float value, Setting<float> setting)
        {
            SettingsController.SetSetting(setting, value);
        }
    }
}