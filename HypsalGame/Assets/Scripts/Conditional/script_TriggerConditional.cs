using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class script_TriggerConditional : abstract_ConditionalRunner
{
    Rigidbody rb;

    bool insideTrigger = false;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        base.Initialize();

    }
    protected override bool Condition()
    {
        return insideTrigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (insideTrigger || !other.CompareTag("Player")) return;

        insideTrigger = ConditionMet = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!insideTrigger || !other.CompareTag("Player")) return;

        insideTrigger = ConditionMet = false;
    }
}
