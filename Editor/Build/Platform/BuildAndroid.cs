﻿using System;
using System.Linq;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildAndroid : BuildPlatform
    {
        #region Constants

        private const string _name = "Android";
        private const string _binaryNameFormat = "{0}.apk";
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Android;

        private const string _deviceTypeVariantId = "Device Type";
        private const string _textureCompressionVariantId = "Texture Compression";
        private const string _buildSystemVariantId = "Build System";
        private const string _splitApksVariantId = "Split APKs";
        private const string _minSdkVersionVariantId = "Min SDK Version";

        private const string _androidApiLevelEnumPrefix = "AndroidApiLevel";

        #endregion

        public BuildAndroid()
        {
            enabled = false;
            Init();
        }

        public override void Init()
        {
            platformName = _name;
            dataDirNameFormat = _dataDirNameFormat;
            targetGroup = _targetGroup;

            if (architectures == null || architectures.Length == 0)
            {
                architectures = new BuildArchitecture[] {
                new BuildArchitecture(BuildTarget.Android, "Android", true, _binaryNameFormat)
            };
            }
            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
#if UNITY_2018_1_OR_NEWER
                new BuildVariant(_deviceTypeVariantId, Enum.GetNames(typeof(AndroidArchitecture)).Skip(1).ToArray(), 0),
#else
                new BuildVariant(_deviceTypeVariantId, Enum.GetNames(typeof(AndroidTargetDevice)), 0),
#endif
                new BuildVariant(_textureCompressionVariantId, Enum.GetNames(typeof(MobileTextureSubtarget)), 0),
#if UNITY_2019_1_OR_NEWER
                new BuildVariant(_buildSystemVariantId, new string[] { "Gradle" }, 0),
#else
                new BuildVariant(_buildSystemVariantId, new string[] { "Internal", "Gradle" }, 0),
#endif
                new BuildVariant(_splitApksVariantId, new string[] { "Disabled", "Enabled" }, 0),
                new BuildVariant(_minSdkVersionVariantId, Enum.GetNames(typeof(AndroidSdkVersions)).Select(i => i.Replace(_androidApiLevelEnumPrefix, "")).ToArray(), 0)
            };
            }
        }

        public override void ApplyVariant()
        {
            foreach (var variantOption in variants)
            {
                string key = variantOption.variantKey;

                switch (variantOption.variantName)
                {
                    case _deviceTypeVariantId:
                        SetDeviceType(key);
                        break;
                    case _textureCompressionVariantId:
                        SetTextureCompression(key);
                        break;
                    case _buildSystemVariantId:
                        SetBuildSystem(key);
                        break;
                    case _splitApksVariantId:
                        SetSplitApks(key);
                        break;
                    case _minSdkVersionVariantId:
                        SetMinSdkVersion(key);
                        break;
                }
            }
        }

        private void SetDeviceType(string key)
        {
#if UNITY_2018_1_OR_NEWER
            PlayerSettings.Android.targetArchitectures = (AndroidArchitecture)Enum.Parse(typeof(AndroidArchitecture), key);
#else
        PlayerSettings.Android.targetDevice = (AndroidTargetDevice)Enum.Parse(typeof(AndroidTargetDevice), key);
#endif
        }

        private void SetTextureCompression(string key)
        {
            EditorUserBuildSettings.androidBuildSubtarget
                = (MobileTextureSubtarget)Enum.Parse(typeof(MobileTextureSubtarget), key);
        }

        private void SetBuildSystem(string key)
        {
            EditorUserBuildSettings.androidBuildSystem
                = (AndroidBuildSystem)Enum.Parse(typeof(AndroidBuildSystem), key);
        }

        private void SetSplitApks(string key)
        {
            PlayerSettings.Android.buildApkPerCpuArchitecture = key == "Enabled";
        }

        private void SetMinSdkVersion(string key)
        {
            PlayerSettings.Android.minSdkVersion
                = (AndroidSdkVersions)Enum.Parse(typeof(AndroidSdkVersions), _androidApiLevelEnumPrefix + key);
        }
    }
}