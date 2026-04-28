using UnityEngine;

public class script_RestartGame : MonoBehaviour
{
    public void Restart()
    {
#if !UNITY_EDITOR
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        Application.Quit(); //kill current process
#endif
    }
}
