using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class MoveOrder : OrderBase
{
    public int targetPositionIndex;
    protected override void Start()
    {
        base.Start();
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        agent.isStopped = false;
        
        targetPositionIndex = baseUnit.manager.selectedUnits.IndexOf(baseUnit.gameObject);
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
        if (Vector3.Distance(transform.position, targetPosition) < 1.25f + targetPositionIndex)
        {
            status = Node.Status.Success;
            agent.isStopped = true;
        }
    }
}
