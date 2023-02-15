#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Build1.PostMVC.Unity.Settings.Editor
{
    public static class SettingsMenu
    {
        [MenuItem("Tools/Build1/PostMVC/Settings/Open Folder", false, 20)]
        public static void OpenFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("Tools/Build1/PostMVC/Settings/Reset", false, 21)]
        public static void SettingsReset()
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
            
            settingsController.Reset();
            
            Debug.Log("Settings reset.");
        }
    }
}

#endif