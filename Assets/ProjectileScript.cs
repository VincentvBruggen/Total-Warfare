using System;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
   public float damage;

   private void Start()
   {
      GetComponent<Rigidbody>().linearVelocity = transform.forward * 10f;
   }
   private void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.GetComponent<IDestroyable>() != null)
      {
         other.gameObject.GetComponent<IDestroyable>().TakeDamage(damage);
      }
   }
}
