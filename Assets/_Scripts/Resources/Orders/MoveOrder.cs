using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class MoveOrder : OrderBase
{
    [FormerlySerializedAs("targetPositionIndex")]
    public int groupCount;
    
    protected override void Awake()
    {
        orderState = UnitState.Move;
    }
    protected override void Start()
    {
        base.Start();
        
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        agent.isStopped = false;

        groupCount = baseUnit.manager.selectedUnits.Count;
        
        targetPosition = targetPosition + (Random.insideUnitSphere * groupCount * 0.5f);
        agent.SetDestination(targetPosition);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void Update()
    {
        base.Update();
        
        // Debug.Log(Vector3.Distance(transform.position, targetPosition));
        if (Vector3.Distance(transform.position, targetPosition) < 1.5f || agent.destination == null)
        {
            status = Node.Status.Success;
            agent.isStopped = true;
            
            print("move complete");
        }
    }
}
