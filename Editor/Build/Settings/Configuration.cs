using System;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class Configuration
    {
        public string id;
        public bool enabled = true;
        public string[] childKeys = null;
    }

    [System.Serializable]
    public class ConfigDictionary : SerializableDictionary<string, Configuration> { }
}
