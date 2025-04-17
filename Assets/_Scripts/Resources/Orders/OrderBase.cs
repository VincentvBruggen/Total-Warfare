using System;
using TotalWarfare;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class OrderBase : MonoBehaviour
{
    public UnitState orderState;
    public Node.Status status = Node.Status.Running;
    public Vector3 targetPosition;
    
    protected NavMeshAgent agent;
    protected BehaviorGraphAgent behaviorAgent;
    protected BaseUnit baseUnit;

    protected virtual void Awake()
    {
        
    }
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
        
        agent.isStopped = false;
        agent.enabled = true;
    }
    protected virtual void OnDisable()
    {
        status = Node.Status.Uninitialized;
    }

    protected virtual void Update()
    {
        if (status == Node.Status.Success)
        {
            RemoveOrder();
        }
    }

    public void RemoveOrder()
    {
        baseUnit.RemoveOrder(this);
        //Destroy(this);
    }
}
