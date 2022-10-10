using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression.FileSystem;
using System.Threading;
using UnityEditor;

public class Builds
{
    private static readonly string BaseBuildPath = "./Builds";
    private static readonly string ClientPath = $"{BaseBuildPath}/Client/Client.exe";
    private static readonly string ServerPath = $"{BaseBuildPath}/Server/Server.exe";

    [MenuItem("Builds/Build/Client")]
    static void BuildClient()
    {
        string[] defaultScenes = { "Assets/Scenes/Client/Login.unity", "Assets/Scenes/Client/Client.unity" };
        BuildPipeline.BuildPlayer(defaultScenes, ClientPath, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("Builds/Build/Server")]
    static void BuildServer()
    {
        string[] defaultScenes = { "Assets/Scenes/Server/Server.unity"};
        BuildPipeline.BuildPlayer(defaultScenes, ServerPath, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("Builds/Build/Server+Client")]
    static void BuildServerAndClient()
    {
        BuildServer();
        BuildClient();
    }

    [MenuItem("Builds/Build+Run/Client")]
    static void BuildAndRunClient()
    {
        BuildClient();
        RunClient();
    }

    [MenuItem("Builds/Build+Run/Server")]
    static void BuildAndRunServer()
    {
        BuildServer();
        RunServer();
    }

    [MenuItem("Builds/Build+Run/Server+Client")]
    static void BuildAndRunServerWithClient()
    {
        BuildServerAndClient();
        RunServerAndClient();
    }

    [MenuItem("Builds/Run/Client")]
    static void RunClient()
    {
        string exeFileAndLocation = $"{Directory.GetCurrentDirectory()}/{ClientPath}";
        string arguments = $"-username test_{new Random().Next(100, 999)} -debugUtil";

        Process.Start(exeFileAndLocation, arguments);
    }

    [MenuItem("Builds/Run/Server")]
    static void RunServer()
    {
        string exeFileAndLocation = $"{Directory.GetCurrentDirectory()}/{ServerPath}";
        string arguments = "-debugUtil";

        Process.Start(exeFileAndLocation, arguments);
    }

    [MenuItem("Builds/Run/Server+Client")]
    static void RunServerAndClient()
    {
        RunServer();
        Thread.Sleep(2500);
        RunClient();
    }
    
    [MenuItem("Builds/Compress/Client")]
    static void CompressClient()
    {
        ZipFile.CreateFromDirectory($"{BaseBuildPath}/Client", $"{BaseBuildPath}/Client.zip");
    }
                                                                     
    [MenuItem("Builds/Compress/Server")]
    static void CompressServer()
    {
        ZipFile.CreateFromDirectory($"{BaseBuildPath}/Server", $"{BaseBuildPath}/Server.zip");
    }
                                                                     
    [MenuItem("Builds/Compress/Server+Client")]
    static void CompressServerAndClient()
    {
        CompressServer();
        CompressClient();
    }
}
