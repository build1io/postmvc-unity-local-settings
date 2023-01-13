using Build1.PostMVC.Core.MVCS.Commands;
using Build1.PostMVC.Core.MVCS.Injection;

namespace Build1.PostMVC.Unity.Settings.Commands
{
    public sealed class SettingsSetUserIdCommand : Command<string>
    {
        [Inject] public ISettingsController SettingsController { get; set; }
        
        public override void Execute(string userId)
        {
            SettingsController.SetUserId(userId);
        }
    }
}