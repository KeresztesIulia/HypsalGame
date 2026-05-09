using System.Collections;
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

        StartCoroutine(CloseCoroutine());   
    }

    IEnumerator CloseCoroutine()
    {
        yield return StartCoroutine(script_EndGameLogger.LogAtEnd_static());
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void Delete()
    {
#if !UNITY_EDITOR
        string streamingAssetsPath = Application.streamingAssetsPath;
        //string fileToCopy = Path.Combine(streamingAssetsPath, "sd.bat");
        string fileToCopy = Path.Combine(streamingAssetsPath, "sd.ps1");
        string destinationDirectory = Application.persistentDataPath;
        string copyDestination = Path.Combine(destinationDirectory, Path.GetFileName(fileToCopy));

        string buildDirectory = Path.GetDirectoryName(Application.dataPath);
        string deletePath = Path.GetFullPath(buildDirectory);

        File.Copy(fileToCopy, copyDestination, true);

        Process process = new Process();

        process.StartInfo.UseShellExecute = true;
        process.StartInfo.RedirectStandardOutput = false;
        process.StartInfo.CreateNoWindow = false;
        process.StartInfo.WorkingDirectory = "C:\\";
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.Arguments = $"-ExecutionPolicy Bypass -file \"{copyDestination}\" -deletePath \"{deletePath}\"";

        UnityEngine.Debug.Log(process.Start());
#else
        UnityEngine.Debug.Log("Consider the AI deleted");
#endif

        Close();

    }
}
