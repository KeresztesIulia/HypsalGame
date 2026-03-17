using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class script_PushCube : script_Interactable
{
    [SerializeField] float _pushForce = 100;
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public override void Interact(Vector3 playerPosition)
    {
        Vector3 forceDirection = (rb.position - playerPosition).normalized;
        rb.AddForce(forceDirection * _pushForce);
    }
}
