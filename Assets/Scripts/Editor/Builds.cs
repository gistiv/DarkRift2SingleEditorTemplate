using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEditor;

public class Builds
{
    private static readonly string BaseBuildPath = "./Builds";
    private static readonly string ClientPath = $"{BaseBuildPath}/Client/Client.exe";
    private static readonly string ServerPath = $"{BaseBuildPath}/Server/Server.exe";

    [MenuItem("Builds/Build/Client")]
    public static void BuildClient()
    {
        string[] defaultScenes = { "Assets/Scenes/Client/Login.unity", "Assets/Scenes/Client/Client.unity" };
        BuildPipeline.BuildPlayer(defaultScenes, ClientPath, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("Builds/Build/Server")]
    public static void BuildServer()
    {
        string[] defaultScenes = { "Assets/Scenes/Server/Server.unity"};
        BuildPipeline.BuildPlayer(defaultScenes, ServerPath, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("Builds/Build/Server+Client")]
    public static void BuildServerAndClient()
    {
        BuildServer();
        BuildClient();
    }

    [MenuItem("Builds/Build+Run/Client")]
    public static void BuildAndRunClient()
    {
        BuildClient();
        RunClient();
    }

    [MenuItem("Builds/Build+Run/Server")]
    public static void BuildAndRunServer()
    {
        BuildServer();
        RunServer();
    }

    [MenuItem("Builds/Build+Run/Server+Client")]
    public static void BuildAndRunServerWithClient()
    {
        BuildServerAndClient();
        RunServerAndClient();
    }

    [MenuItem("Builds/Run/Client")]
    public static void RunClient()
    {
        string exeFileAndLocation = $"{Directory.GetCurrentDirectory()}/{ClientPath}";
        string arguments = $"-username test_{new Random().Next(100, 999)} -debugUtil";

        Process.Start(exeFileAndLocation, arguments);
    }

    [MenuItem("Builds/Run/Server")]
    public static void RunServer()
    {
        string exeFileAndLocation = $"{Directory.GetCurrentDirectory()}/{ServerPath}";
        string arguments = "-debugUtil";

        Process.Start(exeFileAndLocation, arguments);
    }

    [MenuItem("Builds/Run/Server+Client")]
    public static void RunServerAndClient()
    {
        RunServer();
        Thread.Sleep(2500);
        RunClient();
    }

    [MenuItem("Builds/Compress/Client")]
    public static void CompressClient()
    {
        CompressFolder("Client");
    }

    [MenuItem("Builds/Compress/Server")]
    public static void CompressServer()
    {
        CompressFolder("Server");
    }

    [MenuItem("Builds/Compress/Server+Client")]
    public static void CompressServerAndClient()
    {
        CompressServer();
        CompressClient();
    }

    private static void CompressFolder(string folderName)
    {
        string targetPath = $"{BaseBuildPath}/{folderName}.zip";

        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }

        ZipFile.CreateFromDirectory($"{BaseBuildPath}/{folderName}", targetPath);
        UnityEngine.Debug.Log($"Done compressing {folderName}");
    }
}
