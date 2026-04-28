using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class script_EndGame : MonoBehaviour
{


    public void Restart()
    {
#if !UNITY_EDITOR
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        Application.Quit(); //kill current process
#endif
    }

    public void Close()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void Delete()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        string fileToCopy = Path.Combine(streamingAssetsPath, "sd.bat");
        string destinationDirectory = Application.persistentDataPath;
        string copyDestination = Path.Combine(destinationDirectory, Path.GetFileName(fileToCopy));

        string buildDirectory = Path.GetDirectoryName(Application.dataPath); // send to the batch file

        File.Copy(fileToCopy, copyDestination, true);

        ProcessStartInfo ProcessInfo;
        Process Process;

        Process process = new Process();

        // redirect the output stream of the child process.
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = false;
        process.StartInfo.CreateNoWindow = false;
        process.StartInfo.FileName = copyDestination;
        //process.StartInfo.Arguments = arguments;
        //process.StartInfo.

        SubtitleManager.Instance?.ShowSubtitle(SubtitleManager.SubtitleType.Generic, copyDestination, 10);

        process.Start();
        Application.Quit();

    }
}
