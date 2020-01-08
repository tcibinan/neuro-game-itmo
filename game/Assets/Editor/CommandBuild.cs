using UnityEngine;
using UnityEditor;

public class CommandBuild
{
    public static void BuildAndroid()
    {
        string[] levels = {"Assets/Scenes/Game.unity"};
        BuildPipeline.BuildPlayer(levels, "build/neuro-game.apk", BuildTarget.Android, BuildOptions.None);
    }
}
