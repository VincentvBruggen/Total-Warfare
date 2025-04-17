using System;
using System.Collections.Generic;
using FischlWorks_FogWar;
using NUnit.Framework;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum UnitState
  {
      Idle,
      Move,
      Patrol,
      Attack,
      Build
  } 
public abstract class BaseUnit : MonoBehaviourPun, ISelectable, IDestroyable
{
    public enum UnitType
    {
        Commander,
        Attack,
        Builder
    }
    
    public UnitType type;
    public UnitState state;
    
    [SerializeField] protected csFogWar.FogRevealer fogRevealer;
    public BehaviorGraphAgent behaviorAgent;
    public NavMeshAgent navMeshAgent;
    public csFogVisibilityAgent fogAgent;
    public SelectionManager manager;

    public float health;
    public float observationRadius;
    
    public List<OrderBase> ordersList = new List<OrderBase>();
    [FormerlySerializedAs("orderTargetPosition")]
    public List<Vector3> orderTargetPositions = new List<Vector3>();
    public List<OrderBase> orderScripts = new List<OrderBase>();

    [SerializeField] protected float moveSpeed; 
    [SerializeField] protected float turnSpeed;
    [HideInInspector] public OrderBase emptyOrder;

    [SerializeField] private GameObject selectionVisual;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject icon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected void Awake()
    {
        fogRevealer = new csFogWar.FogRevealer(transform, (int)observationRadius, false);
    }

    protected virtual void Start()
    {
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.angularSpeed = turnSpeed;
        if (photonView.IsMine)
        {
            fogAgent.enabled = false;
            FindFirstObjectByType<csFogWar>().AddFogRevealer(fogRevealer);
        }
        else
        {
            lineRenderer.enabled = false;
        }
        foreach (OrderBase order in orderScripts)
        {
            order.enabled = false;
        }
        
        selectionVisual.SetActive(false);
        if (lineRenderer == null)
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
        }


        if (photonView.IsMine)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
        
        while (manager == null)
        {
            SelectionManager[] managers = FindObjectsByType<SelectionManager>(FindObjectsSortMode.None);

            foreach (SelectionManager m_manager in managers)
            {
                if (m_manager.photonView.IsMine)
                {
                    manager = m_manager;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        lineRenderer.SetPosition(0, transform.position);
        // if (GetComponent<OrderBase>() != null && ordersList.Count == 0)
        // {
        //     ordersList.Add(GetComponent<OrderBase>());
        // }

        if (orderTargetPositions.Count > 0)
        {
            lineRenderer.positionCount = orderTargetPositions.Count + 1;
            for (int i = 0; i < orderTargetPositions.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, orderTargetPositions[i] + Vector3.up * 0.01f);
        
            }
        }
        else
        {
            lineRenderer.positionCount = 1;
        }
        
        // BlackboardReference blackboard =  behaviorAgent.BlackboardReference;
        // if (blackboard.GetVariable<OrderBase>("CurrentOrder", out BlackboardVariable<OrderBase> order))
        // {
        //     if (ordersList.Count > 0)
        //     {
        //         order.Value = ordersList[0];
        //     }
        //     else
        //     {
        //         order.Value = emptyOrder;
        //     }
        //     blackboard.SetVariableValue("CurrentOrder", order);
        // }
        
        StateMachine();
    }

    public void OnSelect(GameObject owner)
    {
        print("OnSelect by: " + owner.name);
        selectionVisual.SetActive(true);
    }
    public void OnDeselect(GameObject owner)
    {
        selectionVisual.SetActive(false);   
    }

    public void SendOrder(OrderBase order, Vector3 targetPosition)
    {
        orderTargetPositions.Add(targetPosition);
        ordersList.Add(order);
    }
    
    public bool RemoveOrder(OrderBase order)
    {
        order.enabled = false;
        
        bool successfullRemove = false;
        if (this.ordersList.Count > 0)
        {
            ordersList.RemoveAt(0);
        }
        return successfullRemove;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if(health <= 0){ OnDeath(); }
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    protected void StateMachine()
    {
        switch (state)
        {
            case UnitState.Idle:
                if (ordersList.Count > 0)
                {
                    state = ordersList[0].orderState;
                }
                break;
            
            case UnitState.Move:
                if (ordersList.Count > 0)
                {
                    MoveOrder move = GetComponent<MoveOrder>();
                    move.enabled = true;
                    
                    if(move.status == Node.Status.Success)
                    {
                        orderTargetPositions.RemoveAt(0);
                        move.enabled = false;
                        ordersList.RemoveAt(0);

                        state = UnitState.Idle;
                    }
                }
                else
                {
                    state = UnitState.Idle;
                }
                
                break;
            
            case UnitState.Patrol:
                break;
            
            case UnitState.Attack:
                break;
            
            case UnitState.Build:
                if (ordersList.Count > 0)
                {
                    BuildOrder build = GetComponent<BuildOrder>();
                    build.enabled = true;
                    
                    if(build.status == Node.Status.Success)
                    {
                        orderTargetPositions.RemoveAt(0);
                        build.enabled = false;
                        ordersList.RemoveAt(0);

                        state = UnitState.Idle;
                    }
                }
                else
                {
                    state = UnitState.Idle;
                }
                break;
        }
    }
}
