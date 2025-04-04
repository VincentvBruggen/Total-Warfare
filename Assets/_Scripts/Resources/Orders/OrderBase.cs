using System;
using TotalWarfare;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class OrderBase : MonoBehaviour
{

    public Node.Status status = Node.Status.Running;
    public Vector3 targetPosition;
    
    protected NavMeshAgent agent;
    protected BehaviorGraphAgent behaviorAgent;
    protected BaseUnit baseUnit;
    protected virtual void Start()
    {
        
    }

    protected virtual void OnEnable()
    {
        status = Node.Status.Running;
        agent = GetComponent<NavMeshAgent>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        baseUnit = GetComponent<BaseUnit>();
        targetPosition = baseUnit.orderTargetPositions[0];
    }
    protected virtual void OnDisable()
    {
        status = Node.Status.Uninitialized;
    }

    protected virtual void Update()
    {
        if (status == Node.Status.Success)
        {
            baseUnit.orderTargetPositions.RemoveAt(0);
            RemoveOrder();
        }
    }

    public void RemoveOrder()
    {
        baseUnit.RemoveOrder(this);
        //Destroy(this);
    }
}
