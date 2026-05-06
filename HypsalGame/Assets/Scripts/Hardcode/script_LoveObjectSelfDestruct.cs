using UnityEngine;
using UnityEngine.Events;

public class script_LoveObjectSelfDestruct : MonoBehaviour
{
    [SerializeField] float _sinkingSpeed = 0.3f;
    [SerializeField] float _destroyAfterTime = 5f;
    public UnityEvent OnDestruction;

    float sinkingTime = 0;

    private void Start()
    {
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

    private void Update()
    {
        sinkingTime += Time.deltaTime;
        var pos = transform.position;
        pos.y -= _sinkingSpeed * Time.deltaTime;
        transform.position = pos;

        if (sinkingTime > _destroyAfterTime)
        {
            Destroy(gameObject);
            OnDestruction?.Invoke();
        }
    }
}
