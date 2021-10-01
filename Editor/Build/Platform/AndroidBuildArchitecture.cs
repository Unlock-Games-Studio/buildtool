using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class AndroidBuildArchitecture: BuildArchitecture
    {
        public bool isAppBundle;
        public AndroidBuildArchitecture(BuildTarget target,
            string name,
            bool enabled,
            string binaryNameFormat,
            bool isAppBundle) : base(target,
            name,
            enabled,
            binaryNameFormat)
        {
            this.isAppBundle = isAppBundle;
        }

        public override void OnPreBuild()
        {
            EditorUserBuildSettings.buildAppBundle = isAppBundle;
        }
    }
}
