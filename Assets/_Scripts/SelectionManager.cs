using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Unity.Behavior;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviourPun, PlayerInputs.IGameplayActions
{
    public static SelectionManager instance;
    
    public OrderBase currentOrder;
    public GameObject currentBuilding;
    public string buildingName;
    public Vector3 targetPosition;
    [SerializeField] private string standardOrder;

    public List<GameObject> selectedUnits = new List<GameObject>();
    private PlayerInputs inputAsset;
    public Color currentGhostColor;
    private bool buildingIsPlacable = true;
    void Awake()
    {
        inputAsset = new PlayerInputs();
        inputAsset.Gameplay.AddCallbacks(this);
        
        instance = this;
    }

    private void Start()
    {
        foreach (Button button in UIManager.Instance.orderButtons)
        {
            button.onClick.AddListener((UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, button.name + "Order")); // credit to Programmer on StackOverflow
        }

        foreach (GameObject contentField in GameObject.FindGameObjectsWithTag("UIContents"))
        {
            foreach (Button button in contentField.GetComponentsInChildren<Button>())
            {
                button.onClick.AddListener(delegate { BuildOrder(button.name); });
            }
        }
    }

    void OnEnable() => inputAsset.Enable();
    void OnDisable() => inputAsset.Disable();

    private void Update()
    {
        if (selectedUnits.Count > 0)
        {
            ConstructionUnit constructionUnit = null;
            Commander commander = null;

            foreach (GameObject unit in selectedUnits)
            {
                constructionUnit = unit.GetComponent<ConstructionUnit>();
                commander = unit.GetComponent<Commander>();

                if (constructionUnit != null || commander != null) { break; }
            }

            if (constructionUnit != null || commander != null)
            {
                UIManager.Instance.constructionPanel.SetActive(true);
            }
            else
            {
                UIManager.Instance.constructionPanel.SetActive(false);
            }
        }
        else if (UIManager.Instance.constructionPanel.activeSelf == true)
        {
            UIManager.Instance.constructionPanel.SetActive(false);
        }
        
        Vector3 position = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(position);

        if (currentBuilding == null)
        {
            return;
        }
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentBuilding.GetComponentInChildren<Collider>().enabled = false;
            currentBuilding.transform.position = hit.point;

            if (!hit.collider.CompareTag("Terrain"))
            {
                buildingIsPlacable = false;
                
                currentBuilding.GetComponentInChildren<MeshRenderer>().material.SetColor("_FresnelColor", Color.red);
            }
            else
            {
                buildingIsPlacable = true;
                currentBuilding.GetComponentInChildren<MeshRenderer>().material.SetColor("_FresnelColor", currentGhostColor);
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentBuilding != null && buildingIsPlacable)
        {
            foreach (GameObject selectedUnit in selectedUnits)
            {
                BaseUnit unit = selectedUnit.GetComponent<BaseUnit>();
                targetPosition = currentBuilding.transform.position;
                SendingOrder(currentOrder, unit);
            }
            
            currentBuilding.GetComponentInChildren<Collider>().enabled = true;
            currentBuilding = null;
        }

        Vector3 position = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ISelectable selectable = hit.collider.GetComponent<ISelectable>();
            PhotonView selectablePhotonView = hit.collider.GetComponent<PhotonView>();

            if ((selectable == null || selectablePhotonView == null || !selectablePhotonView.IsMine ) && !Keyboard.current.shiftKey.isPressed)
            {
                if(EventSystem.current.IsPointerOverGameObject()){return;}
                    
                foreach (GameObject unit in selectedUnits)
                {
                    unit.GetComponent<ISelectable>().OnDeselect(gameObject);
                }
                selectedUnits.Clear();
                return;
            }

            GameObject selectedUnit = hit.collider.gameObject;
            if (!selectedUnits.Contains(selectedUnit))
            {
                selectedUnits.Add(selectedUnit);
                selectable.OnSelect(gameObject);
            }
        }
    }

    public void OnShiftClick(InputAction.CallbackContext context) { }

    public void OnOverrideOrder(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        // 1. geen order -> move order
        // 2. move order -> move order
        // 3. build order ->
        
                    
        Vector3 position = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPosition = hit.point;
            foreach (GameObject selectedUnit in selectedUnits)
            {
                BaseUnit unit = selectedUnit.GetComponent<BaseUnit>();
                
                if (currentOrder == GetComponent<BuildOrder>())
                {
                    PhotonNetwork.Destroy(currentBuilding);
                    currentOrder = null;
                }
                
                if (currentOrder != null)
                {
                    
                    SendingOrder(currentOrder, unit);
                    return;
                }

                currentOrder = unit.GetComponent<MoveOrder>();

                SendingOrder(currentOrder, unit);
            }
        }
    }

    private void SendingOrder(OrderBase order, BaseUnit unit)
    {
        if (!Keyboard.current.shiftKey.isPressed)
        {
            foreach (OrderBase script in unit.GetComponents<OrderBase>())
            {
                script.status = Node.Status.Success;
            }

            if (unit.ordersList.Count > 0)
            {
                unit.ordersList.Clear();
            }
            unit.orderTargetPositions.Clear();

            unit.behaviorAgent.Graph.Restart();
        }
        unit.SendOrder(order, targetPosition); 
        unit.behaviorAgent.BlackboardReference.SetVariableValue("CurrentOrder", unit.ordersList[0]);
    }

    private void BuildOrder(string building)
    {
        // placing the ghost of the building
        if (currentBuilding != null)
        {
            PhotonNetwork.Destroy(currentBuilding);
        }
        currentBuilding = PhotonNetwork.Instantiate("BuildingPrefabs/" + building, targetPosition, Quaternion.identity);
        
        currentGhostColor = currentBuilding.GetComponentInChildren<MeshRenderer>().material.GetColor("_FresnelColor");
        Debug.Log(building);

        currentOrder = GetComponent<BuildOrder>();
    }

    private void MoveOrder()
    {
        currentOrder = GetComponent<MoveOrder>();
    }
    private void PatrolOrder()
    {
        // PatrolOrder
    }
    private void AttackOrder()
    {
        // AttackOrder
    }
    private void ReclaimOrder()
    {
        // ReclaimOrder
    }
}