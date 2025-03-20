using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

using Photon.Pun;
using UnityEngine.Serialization;

public abstract class BaseUnit : MonoBehaviourPun, ISelectable, IDestroyable
{
    public BehaviorGraphAgent behaviorAgent;
    public NavMeshAgent navMeshAgent;

    public float health;
    public float observationRadius;
    
    public List<OrderBase> ordersList = new List<OrderBase>();
    [FormerlySerializedAs("orderTargetPosition")]
    public List<Vector3> orderTargetPositions = new List<Vector3>();

    [SerializeField] protected float moveSpeed; 
    [SerializeField] protected float turnSpeed;
    private OrderBase emptyOrder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        BlackboardReference blackboard = behaviorAgent.BlackboardReference;
        if (blackboard.GetVariable<OrderBase>("CurrentOrder", out BlackboardVariable<OrderBase> order))
        {
            if (ordersList.Count > 0)
            {
                order.Value = ordersList[0];
            }
            else
            {
                order.Value = emptyOrder;
            }
            blackboard.SetVariableValue("CurrentOrder", order);
        }
    }

    public void OnSelect(GameObject owner)
    {
        print("OnSelect by: " + owner.name);
    }
    public void OnDeselect(GameObject owner)
    {
        
    }

    public void SendOrder(OrderBase order, Vector3 targetPosition)
    {
        ordersList.Add(order);
        this.orderTargetPositions.Add(targetPosition);
    }
    
    public void RemoveOrder(OrderBase order)
    {
        ordersList.RemoveAt(0);
        Destroy(order);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if(health <= 0){ OnDeath(); }
    }

    protected virtual void OnDeath()
    {
        
    }
}
