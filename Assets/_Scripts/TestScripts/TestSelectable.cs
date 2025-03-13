using Photon.Pun;
using UnityEngine;

public class TestSelectable : MonoBehaviour, ISelectable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(GameObject owner)
    {
        print("I HAVE BEEN SELECTED BY: " + owner.name);
    }
}
