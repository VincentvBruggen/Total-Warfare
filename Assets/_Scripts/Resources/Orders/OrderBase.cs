using TotalWarfare;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class OrderBase : MonoBehaviour
{
    public enum Status
    {
        Running,
        Success,
        Failure
    }
    public Status status = Status.Running;
    public Vector3 targetPosition;
    
    protected NavMeshAgent agent;
    protected BehaviorGraphAgent behaviorAgent;
    protected BaseUnit baseUnit;
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        baseUnit = GetComponent<BaseUnit>();
        this.targetPosition = this.baseUnit.orderTargetPositions[0];
        this.baseUnit.orderTargetPositions.RemoveAt(0);
    }

    protected virtual void Update()
    {
        if (status == Status.Success)
        {
            this.baseUnit.RemoveOrder(this);
        }
    }
}
