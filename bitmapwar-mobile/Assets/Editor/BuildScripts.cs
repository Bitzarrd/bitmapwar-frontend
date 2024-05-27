
using UnityEditor;
using System.IO;
using Codice.Client.Common;
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
    }

    [MenuItem("BuildBitmap/GetUIPath ^g")]
    public static void GetPathOfUGUI()
    {
        Transform t = Selection.activeTransform;
        //Debug.Log(getPathofNode(t, t.name));
        EditorGUIUtility.systemCopyBuffer = getPathofNode(t, t.name);
    }

    public static string getPathofNode(Transform t, string pathNow)
    {
        if (t.parent.name == "MainPage")
        {
            return pathNow;
        }
        if (t.parent != null)
        {
            return getPathofNode(t.parent, t.parent.name + "/" + pathNow);
        }

        return pathNow;
    }
    
}