using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingBase : MonoBehaviourPun, ISelectable, IDestroyable
{
    public float maxHealth;
    public float currentHealth;
    
    public bool isPlaced = false;
    public float metalCost;
    public float buildProgress;
    
    public List<GameObject> assignedUnits;
    
    [SerializeField] GameObject ghost;
    [SerializeField] GameObject building;
    [SerializeField] GameObject selectionVisual;
    [SerializeField] Slider progressBar;

    private bool isCompleted;

    private Coroutine _coroutine;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
        
        progressBar.maxValue = metalCost;
    }

    // Update is called once per frame
    void Update()
    {
        progressBar.value = buildProgress;
        if (buildProgress > 0)
        {
            if (assignedUnits.Count == 0 && !isCompleted)
            {
                buildProgress -= Time.deltaTime;
            }
            if (!photonView.IsMine)
            {
                gameObject.SetActive(true);
            }

            if (buildProgress >= metalCost)
            {
                isCompleted = true;
                
                ghost.SetActive(false);
                
                
                progressBar.gameObject.SetActive(false);
                building.SetActive(true);
            }
        }
        else if(isPlaced)
        {
            if (assignedUnits.Count == 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            _coroutine = StartCoroutine(BuildUnits());
        }
    }

    IEnumerator BuildUnits()
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.Instantiate("AttackUnit", transform.position, Quaternion.identity, 0);
    }
    public void OnSelect(GameObject owner)
    {
        selectionVisual.SetActive(true);
    }

    public void OnDeselect(GameObject owner)
    {
        selectionVisual.SetActive(false);
    }

    public void SendOrder(OrderBase order, Vector3 targetPosition)
    {
        
    }
    public bool RemoveOrder(OrderBase order)
    {
        return false;
    }

    public void TakeDamage(float damage)
    {
        
    }
}
