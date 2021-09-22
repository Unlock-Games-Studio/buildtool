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

                // Draw all created/enabled platforms.
                int enabledCount = 0;
                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);
                    SerializedProperty platformEnabled = platformProperty.FindPropertyRelative("enabled");

                    if (platformEnabled.boolValue)
                    {
                        ++enabledCount;
                        EditorGUILayout.PropertyField(platformProperty, GUILayout.MaxHeight(0));
                    }
                }

                if (enabledCount > 0)
                {
                    GUILayout.Space(20);
                }

                // Draw all available platforms.
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                index = EditorGUILayout.Popup(index, availablePlatformNameList.ToArray(), UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(250));
                if (GUILayout.Button("Add Platform", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150)))
                {
                    BuildPlatform addedBuildPlatform = GetBuildPlatform(availablePlatformTypeList[index]);
                    platformList.platforms.Add(addedBuildPlatform);
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
            switch (platform)
            {
                case Platform.Android:
                    return new BuildAndroid();
                case Platform.iOS:
                    return new BuildIOS();
                case Platform.Linux:
                    return new BuildLinux();
                case Platform.OSX:
                    return new BuildOSX();
                case Platform.PC:
                    return new BuildPC();
                case Platform.WebGL:
                    return new BuildWebGL();
                case Platform.AndroidBundle:
                    return new BuildAndroidBundle();
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }
    }
}
