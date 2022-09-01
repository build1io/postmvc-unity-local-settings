using Build1.PostMVC.Core.Modules;
using Build1.PostMVC.Core.MVCS.Injection;
using Build1.PostMVC.Unity.Settings.Impl;

namespace Build1.PostMVC.Unity.Settings
{
    public sealed class SettingsModule : Module
    {
        [Inject] public IInjectionBinder InjectionBinder { get; set; }
        
        [PostConstruct]
        public void PostConstruct()
        {
            InjectionBinder.Bind<ISettingsController, SettingsController>();
        }
    }
}