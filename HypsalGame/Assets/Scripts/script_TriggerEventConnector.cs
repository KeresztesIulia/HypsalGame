using UnityEngine;
using System;

public class script_TriggerEventConnector : MonoBehaviour
{
    public Action TriggerEntered;
    public Action TriggerExited;
    public Action InTrigger;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (TriggerEntered != null) TriggerEntered();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (TriggerExited != null) TriggerExited();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (InTrigger != null) InTrigger();
    }
}
