using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    using System.Linq;

    [Serializable]
    public class BuildAndroidBundle : BuildPlatform
    {
        #region Constants

        private const string _name = "Android Bundle";
        private const string _binaryNameFormat = "{0}.aab";
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Android;

        private const string _deviceTypeVariantId = "Device Type";
        private const string _textureCompressionVariantId = "Texture Compression";
        private const string _buildSystemVariantId = "Build System";
        private const string _splitApksVariantId = "Split APKs";
        private const string _minSdkVersionVariantId = "Min SDK Version";
        private const string _targetSdkVersionVariantId = "Target SDK Version";

        private const string _androidApiLevelEnumPrefix = "AndroidApiLevel";
        private const string _scriptingBackend = "Scripting backend";

        #endregion

        public BuildAndroidBundle()
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
                architectures = new BuildArchitecture[]
                {
                    new BuildArchitecture(BuildTarget.Android, "Android", true, _binaryNameFormat)
                };
            }

            if (variants == null || variants.Length == 0)
            {
                string[] sdks = EnumNamesToArray<AndroidSdkVersions>()
                    .Select(i => i.Replace(_androidApiLevelEnumPrefix, ""))
                    .ToArray();

                variants = new BuildVariant[]
                {
                    new BuildVariant(_deviceTypeVariantId, EnumNamesToArray<AndroidArchitecture>()
                            .Skip(1)
                            .ToArray(),
                        0),
                    new BuildVariant(_textureCompressionVariantId, EnumNamesToArray<MobileTextureSubtarget>(), 0),
                    new BuildVariant(_buildSystemVariantId, new string[] {"Gradle"}, 0),
                    new BuildVariant(_splitApksVariantId, new string[] {"Disabled", "Enabled"}, 0),
                    new BuildVariant(_minSdkVersionVariantId, sdks, 0),
                    new BuildVariant(_targetSdkVersionVariantId, sdks, 0),
                    new BuildVariant(_scriptingBackend, EnumNamesToArray<ScriptingImplementation>(), 1)
                };
            }
        }

        public override void ApplyVariant()
        {
            EditorUserBuildSettings.buildAppBundle = true;

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
                    case _targetSdkVersionVariantId:
                        SetMaxSdkVersion(key);
                        break;
                    case _scriptingBackend:
                        SetScriptingBackend(key);
                        break;
                }
            }
        }

        private void SetDeviceType(string key)
        {
            PlayerSettings.Android.targetArchitectures = EnumValueFromKey<AndroidArchitecture>(key);
        }

        private void SetTextureCompression(string key)
        {
            EditorUserBuildSettings.androidBuildSubtarget
                = EnumValueFromKey<MobileTextureSubtarget>(key);
        }

        private void SetBuildSystem(string key)
        {
            EditorUserBuildSettings.androidBuildSystem
                = EnumValueFromKey<AndroidBuildSystem>(key);
        }

        private void SetSplitApks(string key)
        {
            PlayerSettings.Android.buildApkPerCpuArchitecture = key == "Enabled";
        }

        private void SetMinSdkVersion(string key)
        {
            PlayerSettings.Android.minSdkVersion
                = EnumValueFromKey<AndroidSdkVersions>(_androidApiLevelEnumPrefix + key);
        }

        private void SetMaxSdkVersion(string key)
        {
            PlayerSettings.Android.targetSdkVersion = EnumValueFromKey<AndroidSdkVersions>(_androidApiLevelEnumPrefix + key);
        }

        private void SetScriptingBackend(string key)
        {
            PlayerSettings.SetScriptingBackend(_targetGroup, EnumValueFromKey<ScriptingImplementation>(key));
        }
    }
}
