using UnityEngine;
using Photon.Pun;   
public class BuildingBase : MonoBehaviourPun
{
    public float metalCost;
    public float buildProgress;
    
    [SerializeField] GameObject ghost;
    [SerializeField] GameObject building;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buildProgress > 0)
        {
            if (!photonView.IsMine)
            {
                gameObject.SetActive(true);
            }

            if (buildProgress >= metalCost)
            {
                ghost.SetActive(false);
                building.SetActive(true);
            }
        }
    }
}
