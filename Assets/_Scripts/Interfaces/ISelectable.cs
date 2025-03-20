using Photon.Pun;
using UnityEngine;

public interface ISelectable
{
    void OnSelect(GameObject owner);
    void OnDeselect(GameObject owner);
    void SendOrder(OrderBase order, Vector3 targetPosition);
    void RemoveOrder(OrderBase order);
}
