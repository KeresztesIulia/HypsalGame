using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class script_PlayerBeaming : MonoBehaviour
{
    [SerializeField] Vector3 beamPosition;
    [SerializeField] float beamDuration;
    Vector3 startPosition;


    Vector3 beamSpeed;

    PlayerInput player;
    FirstPersonController playerFPC;
    //Rigidbody playerRB;
    //RigidbodyConstraints originalConstraints;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerInput>();
        playerFPC = player?.GetComponent<FirstPersonController>();

        if (player == null) return;

        if (beamDuration == 0)
        {
            player.transform.position = beamPosition;
            enabled = false;
        }

        player.enabled = false;
        
        if (playerFPC != null) playerFPC.enabled = false;
        //playerRB.constraints = RigidbodyConstraints.FreezeAll;

        startPosition = player.transform.position;
    }

    private void Update()
    {
        player.transform.position = Vector3.SmoothDamp(player.transform.position, beamPosition, ref beamSpeed, beamDuration);
        if (Vector3.Distance(player.transform.position, beamPosition) <= 0.1f)
        {
            player.transform.position = beamPosition;
            //playerRB.constraints = originalConstraints;
            if (playerFPC != null) playerFPC.enabled = true;

            player.enabled = true;
            enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(beamPosition, 1);
    }
}
