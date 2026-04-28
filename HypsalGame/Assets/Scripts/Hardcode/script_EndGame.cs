using System.IO;
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

    public void Delete()
    {
        string fileToCopy = "sd.bat";
        string destinationDirectory = "c:\\myDestinationFolder\\";

        File.Copy(fileToCopy, destinationDirectory + Path.GetFileName(fileToCopy));
    }
}
