using UnityEditor;
using UnityEngine.Serialization;
using System;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildReleaseType
    {
        public string id;
        public string typeName = string.Empty;
        [FormerlySerializedAs("bundleIndentifier")]
        public string bundleIdentifier = string.Empty;
        public string companyName = string.Empty;
        public string productName = string.Empty;

        public BuildOptions buildOptions;
        public string customDefines = string.Empty;

        public SceneList sceneList = new SceneList();
    }
}
