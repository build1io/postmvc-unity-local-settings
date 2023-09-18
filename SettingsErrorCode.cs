namespace Build1.PostMVC.Unity.Settings
{
    public enum SettingsErrorCode
    {
        None                        = 0,
        SettingsNotInitialized      = 1,
        DeviceSettingsAlreadyLoaded = 2,
        UserSettingsAlreadyLoaded   = 3,
        Exception                   = 100
    }
}