using UnityEngine;
using UnityEngine.AI;

public class MoveOrder : OrderBase
{
    protected override void Start()
    {
        base.Start();
        
        agent.SetDestination(targetPosition);
    }
    protected override void Update()
    {
        base.Update();
        
        Debug.Log(Vector3.Distance(transform.position, targetPosition));
        if (Vector3.Distance(transform.position, targetPosition) < 1.25f)
        {
            status = Status.Success;
        }
    }
}
