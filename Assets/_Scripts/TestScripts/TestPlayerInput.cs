using System;
using Photon.Pun;
using UnityEngine;

public class TestPlayerInput : MonoBehaviourPun
{
    [SerializeField] float moveSpeed = 5f;
    Vector2 direction = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine) { return; }
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine) { return; }
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.y * moveSpeed);
    }
}
