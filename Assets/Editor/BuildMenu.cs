using UnityEditor;

namespace MonsterWorld.Unity.Build
{
    public class BuildMenu
    {
        [MenuItem("Build/Build Server")]
        public static void BuildServer()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { 
                "Assets/Server/Scenes/Boot.unity"
            };
            buildPlayerOptions.locationPathName = "Builds/Server/Server.exe";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.EnableHeadlessMode;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Build Client")]
        public static void BuildClient()
        {
            var defaultOptions = new BuildPlayerOptions();
            var buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(defaultOptions);
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}