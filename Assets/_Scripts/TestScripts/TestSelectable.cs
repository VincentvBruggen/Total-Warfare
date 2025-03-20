using Photon.Pun;
using UnityEngine;

public class TestSelectable : BaseUnit
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        Vector3 randomPos = Vector3.zero + (Random.insideUnitSphere * 5f);
        randomPos.y = 2;
        
        transform.position = randomPos;

        if (photonView.IsMine)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    public void OnSelect(GameObject owner)
    {
        print("I HAVE BEEN SELECTED BY: " + owner.name);
    }

    public void OnDeselect(GameObject owner)
    {
        print("I HAVE BEEN DESELECTED BY: " + owner.name);
    }
}
