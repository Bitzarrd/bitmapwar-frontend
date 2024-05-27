
using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEditor.SceneManagement;

public class BuildScripts
{
    [MenuItem("BuildBitmap/Build All")]
    public static void PerformBuild()
    {
        // Build for Scene 1
        string[] scenes1 = { "Assets/Scenes/SampleScene.unity" };
        BuildPlayerOptions buildPlayerOptions1 = new BuildPlayerOptions();
        buildPlayerOptions1.scenes = scenes1;
        buildPlayerOptions1.locationPathName = "web";
        buildPlayerOptions1.target = BuildTarget.WebGL;
        buildPlayerOptions1.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions1);

        // Build for Scene 2
        string[] scenes2 = { "Assets/Scenes/Release.unity" };
        BuildPlayerOptions buildPlayerOptions2 = new BuildPlayerOptions();
        buildPlayerOptions2.scenes = scenes2;
        buildPlayerOptions2.locationPathName = "3001/web";
        buildPlayerOptions2.target = BuildTarget.WebGL;
        buildPlayerOptions2.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions2);
        
        string[] scenes3 = { "Assets/Scenes/Production.unity" };
        BuildPlayerOptions buildPlayerOptions3 = new BuildPlayerOptions();
        buildPlayerOptions3.scenes = scenes3;
        buildPlayerOptions3.locationPathName = "pro/web";
        buildPlayerOptions3.target = BuildTarget.WebGL;
        buildPlayerOptions3.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions3);
    }
}