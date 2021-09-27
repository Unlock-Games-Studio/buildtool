using UnityEditor;
using System;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildArchitecture
    {
        public string id;
        public BuildTarget target;
        public string name;
        public bool enabled;
        public string binaryNameFormat;

        public BuildArchitecture(BuildTarget target, string name, bool enabled, string binaryNameFormat)
        {
            this.target = target;
            this.name = name;
            this.enabled = enabled;
            this.binaryNameFormat = binaryNameFormat;
            this.id = Guid.NewGuid().ToString();
        }
    }
}
