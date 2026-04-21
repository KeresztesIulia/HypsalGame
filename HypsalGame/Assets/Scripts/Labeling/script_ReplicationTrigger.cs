using UnityEngine;

public class script_ReplicationTrigger : MonoBehaviour
{
    public System.Action TriggerEntered;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        TriggerEntered();
    }
}
