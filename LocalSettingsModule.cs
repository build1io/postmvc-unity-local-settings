using Build1.PostMVC.Core.Extensions.MVCS.Injection;
using Build1.PostMVC.Core.Modules;
using Build1.PostMVC.UnityLocalSettings.Impl;

namespace Build1.PostMVC.UnityLocalSettings
{
    public sealed class LocalSettingsModule : Module
    {
        [Inject] public IInjectionBinder InjectionBinder { get; set; }
        
        [PostConstruct]
        public void PostConstruct()
        {
            InjectionBinder.Bind<ISettingsController, SettingsController>();
        }
    }
}