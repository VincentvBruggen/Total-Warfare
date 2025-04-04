using System.Collections.Generic;
using NUnit.Framework;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using UnityEngine.Serialization;

public abstract class BaseUnit : MonoBehaviourPun, ISelectable, IDestroyable
{
    public BehaviorGraphAgent behaviorAgent;
    public NavMeshAgent navMeshAgent;
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
    protected virtual void Start()
    {
        foreach (OrderBase order in orderScripts)
        {
            order.enabled = false;
        }
        selectionVisual.SetActive(false);
        if (lineRenderer == null)
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
        }

        Vector3 randomPos = Vector3.zero + (Random.insideUnitSphere * 5f);
        randomPos.y = 2;
        
        transform.position = randomPos;

        if (photonView.IsMine)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
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
        icon.transform.forward = -Camera.main.transform.position;
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
        
        BlackboardReference blackboard =  behaviorAgent.BlackboardReference;
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
        bool successfullRemove = false;
        if (this.ordersList.Count > 0)
        {
            successfullRemove = ordersList.Remove(order);
            if (!successfullRemove)
            {
                ordersList.RemoveAt(0);
            }
        }
        order.enabled = false;
        return successfullRemove;
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
