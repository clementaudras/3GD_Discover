/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using UnityEditor;
using Opsive.UltimateCharacterController.Editor.Inspectors.Utility;

namespace Opsive.UltimateCharacterController.Editor.Managers
{
    /// <summary>
    /// Utility functions for the manager classes.
    /// </summary>
    public static class ManagerUtility
    {
        /// <summary>
        /// Draws a control box which allows for an action when the button is pressed.
        /// </summary>
        /// <param name="title">The title of the control box.</param>
        /// <param name="additionalControls">Any additional controls that should appear before the message.</param>
        /// <param name="message">The message within the box.</param>
        /// <param name="enableButton">Is the button enabled?</param>
        /// <param name="button">The name of the button.</param>
        /// <param name="action">The action that is performed when the button is pressed.</param>
        /// <param name="successLog">The message to output to the log upon success.</param>
        public static void DrawControlBox(string title, System.Action additionalControls, string message, bool enableButton, string button, System.Action action, string successLog)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(title, InspectorUtility.BoldLabel);
            GUILayout.Space(4);
            GUILayout.Label(message, InspectorUtility.WordWrapLabel);
            if (additionalControls != null) {
                additionalControls();
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = enableButton;
            if (GUILayout.Button(button, GUILayout.Width(130))) {
                action();
                if (!string.IsNullOrEmpty(successLog)) {
                    Debug.Log(successLog);
                }
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }

        private static string[] s_ItemCollectionGUIDs = new string[] { "5481010ef14c32f4cb7b6661b0c59fb4", "953a0db07d063054da7e2b3a7ce10d80", "b57384c4ae7c2c949b24223f35d0b049",
                                                                       "160c667855917db49836f0dfea94649a", "11ec5295d9f4c504ca0f2879e3c91562", "63d3918ddc86b8b46beb7890713f2950",
                                                                       "f796331b26639e1438aa77663f746cad"};
        private const string c_InvisibleShadowCasterGUID = "0a580a5ea04fdab47941095489aa23b7";
        private static string[] s_StateConfigurationGUIDs = new string[] { "8481381869bbb8b4d8b4d1386e322d67", "399b85de3b739e344bf2242d3ca34f59", "812f2955197251142842243aadef952a",
                                                                            "c0c9529e365437f4588218bbc88366b1", "edd30bc8e08dbd7489f4668cbb38c197", "59f8cfa6986ea2c43b3fbfadb7b4bdc4",
                                                                            "ce9f0d64fcd05da4eb5d388e0f80c07c"};

        private const string c_LastItemCollectionGUIDString = "LastItemCollectionGUID";
        private const string c_LastStateConfigurationGUIDString = "LastStateConfigurationGUID";

        public static string StateConfigurationGUID { get { return s_StateConfigurationGUIDs[0]; } }
        public static string LastItemCollectionGUIDString { get { return c_LastItemCollectionGUIDString; } }
        public static string LastStateConfigurationGUIDString { get { return c_LastStateConfigurationGUIDString; } }

        /// <summary>
        /// Searches for the default item collection.
        /// </summary>
        public static Inventory.ItemCollection FindItemCollection(ScriptableObject editorWindow)
        {
            // If an ItemCollection asset exists within the scene then use that.
            var itemSetManager = Object.FindObjectOfType<Inventory.ItemSetManager>();
            if (itemSetManager != null) {
                if (itemSetManager.ItemCollection != null) {
                    return itemSetManager.ItemCollection;
                }
            }

            // Retrieve the last used ItemCollection.
            var lastItemCollectionGUID = EditorPrefs.GetString(LastItemCollectionGUIDString, string.Empty);
            if (!string.IsNullOrEmpty(lastItemCollectionGUID)) {
                var lastItemCollectionPath = AssetDatabase.GUIDToAssetPath(lastItemCollectionGUID);
                if (!string.IsNullOrEmpty(lastItemCollectionPath)) {
                    var itemCollection = AssetDatabase.LoadAssetAtPath(lastItemCollectionPath, typeof(Inventory.ItemCollection)) as Inventory.ItemCollection;
                    if (itemCollection != null) {
                        return itemCollection;
                    }
                }
            }

            // The GUID should remain consistant.
            string itemCollectionPath;
            for (int i = 0; i < s_ItemCollectionGUIDs.Length; ++i) {
                itemCollectionPath = AssetDatabase.GUIDToAssetPath(s_ItemCollectionGUIDs[i]);
                if (!string.IsNullOrEmpty(itemCollectionPath)) {
                    var itemCollection = AssetDatabase.LoadAssetAtPath(itemCollectionPath, typeof(Inventory.ItemCollection)) as Inventory.ItemCollection;
                    if (itemCollection != null) {
                        return itemCollection;
                    }
                }
            }

            // The item collection doesn't have the expected guid. Try to find the asset based on the path.
            itemCollectionPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(editorWindow))).Replace("Editor/Managers", "Demo/Inventory/DemoItemCollection.asset");
            if (System.IO.File.Exists(Application.dataPath + "/" + itemCollectionPath.Substring(7))) {
                return AssetDatabase.LoadAssetAtPath(itemCollectionPath, typeof(Inventory.ItemCollection)) as Inventory.ItemCollection;
            }

            // Last chance: use resources to try to find the ItemCollection.
            var itemCollections = Resources.FindObjectsOfTypeAll<Inventory.ItemCollection>();
            if (itemCollections != null && itemCollections.Length > 0) {
                return itemCollections[0];
            }

            return null;
        }

        /// <summary>
        /// Searches for the invisible shadow caster material.
        /// </summary>
        public static Material FindInvisibleShadowCaster(ScriptableObject editorWindow)
        {
            // The GUID should remain consistant. 
            var shadowCasterPath = AssetDatabase.GUIDToAssetPath(c_InvisibleShadowCasterGUID);
            if (!string.IsNullOrEmpty(shadowCasterPath)) {
                var invisibleShadowCaster = AssetDatabase.LoadAssetAtPath(shadowCasterPath, typeof(Material)) as Material;
                if (invisibleShadowCaster != null) {
                    return invisibleShadowCaster;
                }
            }

            // The invisible shadow caster doesn't have the expected guid. Try to find the material based on the path.
            shadowCasterPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(editorWindow))).Replace("Editor/Managers", "FirstPersonController/Materials/InvisibleShadowCaster.mat");
            if (System.IO.File.Exists(Application.dataPath + "/" + shadowCasterPath.Substring(7))) {
                return AssetDatabase.LoadAssetAtPath(shadowCasterPath, typeof(Material)) as Material;
            }

            return null;
        }

        /// <summary>
        /// Searches for the default state configuration.
        /// </summary>
        public static StateSystem.StateConfiguration FindStateConfiguration(ScriptableObject editorWindow)
        {
            // Retrieve the last used StateConfiguration.
            var lastStateConfigurationGUID = EditorPrefs.GetString(LastStateConfigurationGUIDString, string.Empty);
            if (!string.IsNullOrEmpty(lastStateConfigurationGUID)) {
                var lastStateConfigurationPath = AssetDatabase.GUIDToAssetPath(lastStateConfigurationGUID);
                if (!string.IsNullOrEmpty(lastStateConfigurationPath)) {
                    var stateConfiguration = AssetDatabase.LoadAssetAtPath(lastStateConfigurationPath, typeof(StateSystem.StateConfiguration)) as StateSystem.StateConfiguration;
                    if (stateConfiguration != null) {
                        return stateConfiguration;
                    }
                }
            }

            // The GUID should remain consistant.
            string stateConfigurationPath;

            for (int i = 0; i < s_StateConfigurationGUIDs.Length; ++i) {
                stateConfigurationPath = AssetDatabase.GUIDToAssetPath(s_StateConfigurationGUIDs[i]);
                if (!string.IsNullOrEmpty(stateConfigurationPath)) {
                    var stateConfiguration = AssetDatabase.LoadAssetAtPath(stateConfigurationPath, typeof(StateSystem.StateConfiguration)) as StateSystem.StateConfiguration;
                    if (stateConfiguration != null) {
                        return stateConfiguration;
                    }
                }
            }

            // The state configuration doesn't have the expected guid. Try to find the asset based on the path.
            stateConfigurationPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(editorWindow))).Replace("Editor/Managers", "Demo/Presets/DemoStateConfiguration.asset");
            if (System.IO.File.Exists(Application.dataPath + "/" + stateConfigurationPath.Substring(7))) {
                return AssetDatabase.LoadAssetAtPath(stateConfigurationPath, typeof(StateSystem.StateConfiguration)) as StateSystem.StateConfiguration;
            }

            // Last chance: use resources to try to find the StateConfiguration.
            var stateConfigurations = Resources.FindObjectsOfTypeAll<StateSystem.StateConfiguration>();
            if (stateConfigurations != null && stateConfigurations.Length > 0) {
                return stateConfigurations[0];
            }

            return null;
        }
    }
}