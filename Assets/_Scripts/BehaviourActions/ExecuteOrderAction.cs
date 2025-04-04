using System;
using Photon.Pun.Demo.PunBasics;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Execute Order", story: "Make [Unit] Do [Order]", category: "Action", id: "77bf8fc21069ed189df882dc38d04a5f")]
public partial class ExecuteOrderAction : Action
{
    [SerializeReference] public BlackboardVariable<OrderBase> Order;
    [SerializeReference] public BlackboardVariable<GameObject> Unit;

    private GameObject _unit;
    private OrderBase _order;
    protected override Status OnStart()
    {
        _unit = Unit.Value;
        // this.Order.OnValueChanged -= OrderOnOnValueChanged;
        // this.Order.OnValueChanged += OrderOnOnValueChanged;

        if (Order.Value != null)
        {
            _order = (OrderBase)_unit.GetComponent(Order.Value.GetType());
            _order.enabled = true;
        }
        
        return Status.Running;
    }
    // private void OrderOnOnValueChanged()
    // {
    //     OrderBase lastOrder = _unit.GetComponent<OrderBase>();
    //     BaseUnit m_unit = _unit.GetComponent<BaseUnit>();
    //     m_unit.RemoveOrder(lastOrder);
    //     
    //     _unit.AddComponent(Order.Value.GetType());
    // }

    protected override Status OnUpdate()
    {
        return _order.status;
    }

    protected override void OnEnd()
    {
    }
}

