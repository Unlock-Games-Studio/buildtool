using System.Collections.Generic;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BuildPlatformList
    {
        [SerializeReference]
        public List<BuildPlatform> platforms = new List<BuildPlatform>();
    }
}
