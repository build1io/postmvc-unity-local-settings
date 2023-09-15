#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Build1.PostMVC.Unity.Settings.Editor
{
    public static class SettingsMenu
    {
        [MenuItem("Tools/Build1/PostMVC/Settings/Reset Device Settings", false, 20)]
        public static void SettingsResetDevice()
        {
            ResetImpl(SettingType.Device);
        }
        
        [MenuItem("Tools/Build1/PostMVC/Settings/Reset User Settings", false, 21)]
        public static void SettingsResetUser()
        {
            ResetImpl(SettingType.User);
        }
        
        [MenuItem("Tools/Build1/PostMVC/Settings/Open Folder", false, 122)]
        public static void OpenFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        private static void ResetImpl(SettingType type)
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Settings Tool",
                                            "Can't reset settings in Editor Mode.\n" +
                                            "Reset them in Play Mode.",
                                            "Ok");
                return;
            }
            
            var settingsController = Core.PostMVC.GetInstance<ISettingsController>();
            if (settingsController == null)
            {
                Debug.LogError("Settings controller not found.");    
                return;
            }
            
            settingsController.Reset(type);
            
            Debug.Log($"Settings reset. Type: {type}");
        }
    }
}

#endif