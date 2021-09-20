using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CustomPropertyDrawer(typeof(BuildPlatformList))]
    public class BuildPlatformListDrawer : PropertyDrawer
    {
        private int index = 0;
        private SerializedProperty list = null;
        private BuildPlatformList platformList = null;

        private Platform[] availablePlatformTypeList;
        private string[] availablePlatformNameList;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.BeginHorizontal();
            property.serializedObject.Update();

            bool show = property.isExpanded;
            UnityBuildGUIUtility.DropdownHeader("Build Platforms", ref show, false, GUILayout.ExpandWidth(true));
            property.isExpanded = show;

            UnityBuildGUIUtility.HelpButton("Parameter-Details#build-platforms");
            EditorGUILayout.EndHorizontal();

            platformList = fieldInfo.GetValue(property.serializedObject.targetObject) as BuildPlatformList;
            PopulateAvailablePlatforms();
            list = property.FindPropertyRelative("platforms");
            list.serializedObject.Update();

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                DrawPlatforms(property);

                if (list.arraySize > 0)
                    GUILayout.Space(20);

                // Draw all available platforms.
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                index = EditorGUILayout.Popup(index, availablePlatformNameList.ToArray(), UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(250));
                if (GUILayout.Button("Add Platform", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150)))
                {
                    BuildPlatform addedBuildPlatform = GetBuildPlatform(availablePlatformTypeList[index]);
                    platformList.platforms.Add(addedBuildPlatform);

                    AssetDatabase.AddObjectToAsset(addedBuildPlatform, BuildSettings.instance);
                    AssetDatabaseUtility.ImportAsset(AssetDatabase.GetAssetPath(BuildSettings.instance));

                    platformList.platforms[platformList.platforms.Count - 1].enabled = true;

                    for (int i = platformList.platforms.Count - 1; i >= 0; i--)
                    {
                        if (!platformList.platforms[i].enabled)
                            platformList.platforms.RemoveAt(i);
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndProperty();
        }

        private void DrawPlatforms(SerializedProperty property)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty listEntry = list.GetArrayElementAtIndex(i);

                BuildPlatform buildPlatform = listEntry.objectReferenceValue as BuildPlatform;
                if (buildPlatform == null)
                {
                    list.DeleteArrayElementAtIndex(i);
                    --i;
                    continue;
                }

                SerializedObject serializedBuildPlatform = new SerializedObject(buildPlatform);

                EditorGUILayout.BeginHorizontal();
                bool show = listEntry.isExpanded;

                UnityBuildGUIUtility.DropdownHeader(buildPlatform.platformName, ref show, false, GUILayout.ExpandWidth(true));

                listEntry.isExpanded = show;

                if (GUILayout.Button("X", UnityBuildGUIUtility.helpButtonStyle))
                {
                    List<BuildPlatform> buildPlatforms = BuildSettings.platformList.platforms;

                    // Destroy underlying object.
                    ScriptableObject.DestroyImmediate(buildPlatforms[i], true);
                    AssetDatabaseUtility.ImportAsset(AssetDatabase.GetAssetPath(BuildSettings.instance));

                    // Remove object reference from list.
                    // TODO: Why do I need to call this twice? First call nulls reference, second one then deletes null entry.
                    list.DeleteArrayElementAtIndex(i);
                    list.DeleteArrayElementAtIndex(i);
                    show = false;
                }

                EditorGUILayout.EndHorizontal();

                if (show && buildPlatform.enabled)
                {
                    EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);
                    buildPlatform.Draw(serializedBuildPlatform);
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PopulateAvailablePlatforms()
        {
            if (availablePlatformTypeList == null)
            {
                availablePlatformTypeList = Enum.GetValues(typeof(Platform)).Cast<Platform>().ToArray();
                availablePlatformNameList = availablePlatformTypeList.Select(platform => platform.ToString()).ToArray();
            }
        }

        private BuildPlatform GetBuildPlatform(Platform platform)
        {
            Type t;
            switch (platform)
            {
                case Platform.Android:
                    t = typeof(BuildAndroid);
                    break;
                case Platform.iOS:
                    t = typeof(BuildIOS);
                    break;
                case Platform.Linux:
                    t = typeof(BuildLinux);
                    break;
                case Platform.OSX:
                    t = typeof(BuildOSX);
                    break;
                case Platform.PC:
                    t = typeof(BuildPC);
                    break;
                case Platform.WebGL:
                    t = typeof(BuildWebGL);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }

            return ScriptableObject.CreateInstance(t) as BuildPlatform;
        }
    }
}
