using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildPlatform
    {
        public bool enabled = false;
        public BuildDistributionList distributionList = new BuildDistributionList();
        public BuildArchitecture[] architectures = new BuildArchitecture[0];
        public BuildVariant[] variants = new BuildVariant[0];

        public string platformName;
        public string dataDirNameFormat;
        public BuildTargetGroup targetGroup;

        public virtual void Init()
        {
        }

        public virtual void ApplyVariant()
        {
        }

        #region Public Properties

        public bool atLeastOneArch
        {
            get
            {
                bool atLeastOneArch = false;
                for (int i = 0; i < architectures.Length && !atLeastOneArch; i++)
                {
                    atLeastOneArch |= architectures[i].enabled;
                }

                return atLeastOneArch;
            }
        }

        public bool atLeastOneDistribution
        {
            get
            {
                bool atLeastOneDist = false;
                for (int i = 0; i < distributionList.distributions.Length && !atLeastOneDist; i++)
                {
                    atLeastOneDist |= distributionList.distributions[i].enabled;
                }

                return atLeastOneDist;
            }
        }

        public string variantKey
        {
            get
            {
                string retVal = "";

                // Build key string.
                if (variants != null && variants.Length > 0)
                {
                    foreach (var variant in variants)
                        retVal += variant.variantKey + ",";
                }

                // Remove trailing delimiter.
                if (retVal.Length > 0)
                    retVal = retVal.Substring(0, retVal.Length - 1);

                return retVal;
            }
        }

        #endregion

        protected static T EnumValueFromKey<T>(string label)
        {
            return (T)Enum.Parse(typeof(T), label.Replace(" ", ""));
        }

        public static string[] EnumNamesToArray<T>(bool toWords = false)
        {
            return Enum.GetNames(typeof(T))
                .Select(item => toWords ? UnityBuildGUIUtility.ToWords(item) : item)
                .ToArray();
        }
    }
}
