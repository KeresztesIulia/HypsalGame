using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class script_LoveObject : MonoBehaviour
{
    [SerializeField] string _loveObjectFollowPositionName = "LoveObjectFollowPosition";
    [SerializeField] float _maxFollowSpeed = 700f;

    Transform followTransform;
    Vector3 followPosition => followTransform.position;

    Vector3 velocity;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        followTransform = FindFirstObjectByType<PlayerInput>()?.transform.Find(_loveObjectFollowPositionName);
    }

    private void Update()
    {
        if (followTransform == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, followPosition, ref velocity, 0f, _maxFollowSpeed);
    }
}
