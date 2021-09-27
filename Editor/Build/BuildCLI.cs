using UnityEditor;
using Mono.Options;
using System.Collections.Generic;

namespace SuperUnityBuild.BuildTool
{
    using System;

    public static class BuildCLI
    {
        public static void PerformBuild()
        {
            //string[] args = System.Environment.GetCommandLineArgs();
            BuildProject.BuildAll();

            // Exit w/ 0 to indicate success.
            EditorApplication.Exit(0);
        }

        public static void BuildMultiple()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();

            List<string> ids = new List<string>();
            bool shouldShowHelp = false;

            var optionSet = new OptionSet
            {
                {"i|id=", "configuration id", id => ids.Add(id)},
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null },
            };

            try
            {
                optionSet.Parse(commandLineArgs);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (shouldShowHelp)
            {
                optionSet.WriteOptionDescriptions(Console.Out);
                return;
            }

            BuildProject.BuildMultiple(ids);

            // Exit w/ 0 to indicate success.
            EditorApplication.Exit(0);
        }
    }
}
