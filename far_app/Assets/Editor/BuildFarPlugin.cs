// #if UNITY_EDITOR
// using UnityEngine;
// using UnityEditor;
// using UnityEditor.Build;
// using UnityEditor.Build.Reporting;
// using System.Diagnostics;
// using System.Runtime.InteropServices;
// using System.IO;

// public class BuildFarPlugin : IPreprocessBuildWithReport
// {
//     public int callbackOrder { get { return 0; }}
//     public void OnPreprocessBuild(BuildReport inReport)
//     {
//         ProcessStartInfo startInfo = new ProcessStartInfo();
//         const string GRADLE_PATH = "../far_plugin/gradlew";
//         if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
//         {
//             UnityEngine.Debug.Log("Using Linux/OSX Platform");
//             startInfo.FileName = GRADLE_PATH;
//         }
//         else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
//         {
//             UnityEngine.Debug.Log("Using Windows Platform");
//             startInfo.FileName = GRADLE_PATH + ".bat";
//         }
//         else
//         {
//             UnityEngine.Debug.Log("Error building far_plugin. Unknown platform type.");
//             return;
//         }

//         startInfo.Arguments = "exportPlugin";
//         startInfo.RedirectStandardOutput = true;
//         startInfo.UseShellExecute = false;

//         UnityEngine.Debug.Log("Building far_plugin...");
//         int exitCode = -1;
//         try
//         {
//             using (Process process = Process.Start(startInfo))
//             {
//                 StreamReader reader = process.StandardOutput;
//                 process.WaitForExit();
//                 exitCode = process.ExitCode;
//                 string output = reader.ReadToEnd();
//                 UnityEngine.Debug.Log($"far_plugin build output:\n'{output}'");
//             }
//         }
//         catch (System.Exception e)
//         {
//             UnityEngine.Debug.Log($"An error occurred while building far_plugin: {e}");
//         }
//         UnityEngine.Debug.Log($"far_plugin build complete with exit code: {exitCode}");
//         // AssetDatabase.Refresh();
//     }
// }
// #endif
