
namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BuildDistribution
    {
        public string id;
        public string distributionName;
        public bool enabled;

        public BuildDistribution()
        {
            this.distributionName = string.Empty;
            this.enabled = true;
        }

        public BuildDistribution(string distributionName, bool enabled)
        {
            this.distributionName = distributionName;
            this.enabled = enabled;
        }
    }
}
