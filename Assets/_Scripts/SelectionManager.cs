using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using TotalWarfare;
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
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
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
            BaseUnit baseUnit = null;

            foreach (GameObject unit in selectedUnits)
            {
                baseUnit = unit.GetComponent<BaseUnit>();

                if (baseUnit.type == BaseUnit.UnitType.Builder || baseUnit.type == BaseUnit.UnitType.Commander) { break; }
            }

            if (baseUnit.type == BaseUnit.UnitType.Builder || baseUnit.type == BaseUnit.UnitType.Commander)
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
            currentBuilding.transform.position = hit.point;

            if (!hit.collider.CompareTag("Terrain"))
            {
                buildingIsPlacable = false;
            }
            else
            {
                buildingIsPlacable = true;
            }
        }

        if (!buildingIsPlacable)
        {
            currentBuilding.GetComponentInChildren<MeshRenderer>().material.SetColor("_FresnelColor", Color.red);
        }
        else
        {
            currentBuilding.GetComponentInChildren<MeshRenderer>().material.SetColor("_FresnelColor", currentGhostColor);
        }
    }
    

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if(!GameManager.instance.isGameStarted){ return;}

        if (currentBuilding != null && buildingIsPlacable)
        {
            BuildingBase buildingBase = currentBuilding.GetComponent<BuildingBase>();
            
            foreach (GameObject selectedUnit in selectedUnits)
            {
                BaseUnit unit = selectedUnit.GetComponent<BaseUnit>();
                targetPosition = currentBuilding.transform.position;
                SendingOrder(currentOrder, unit);
                
                buildingBase.assignedUnits.Add(unit.gameObject);
            }
            currentBuilding.GetComponentInChildren<Collider>().isTrigger = false;
            currentBuilding.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");

            if (Keyboard.current.shiftKey.IsPressed())
            {
                string buildingName = currentBuilding.name;
                string[] splitName = buildingName.Split("(Clone)");
                currentBuilding = null;
                currentBuilding = PhotonNetwork.Instantiate("BuildingPrefabs/" + splitName[0], targetPosition, Quaternion.identity);
                return;
            }
                
            buildingBase.isPlaced = true;
            currentBuilding = null;
            return;
        }

        Vector3 position = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ISelectable selectable = hit.collider.GetComponent<ISelectable>();
            PhotonView selectablePhotonView = hit.collider.GetComponent<PhotonView>();

            if (selectable == null || selectablePhotonView == null || !selectablePhotonView.IsMine)
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

            if (Keyboard.current.shiftKey.IsPressed())
            {
                if (!selectedUnits.Contains(selectedUnit))
                {
                    selectedUnits.Add(selectedUnit);
                    selectable.OnSelect(gameObject);
                }
                else
                {
                    selectedUnits.Remove(selectedUnit);
                    selectable.OnDeselect(gameObject);
                }
            }
            else
            {
                foreach (GameObject unit in selectedUnits)
                {
                    unit.GetComponent<ISelectable>().OnDeselect(gameObject);
                }
                
                selectedUnits.Clear();
                if (!selectedUnits.Contains(selectedUnit))
                {
                    selectedUnits.Add(selectedUnit);
                    selectable.OnSelect(gameObject);
                }
            }
        }
    }

    public void OnShiftClick(InputAction.CallbackContext context) { }

    public void OnOverrideOrder(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if(!GameManager.instance.isGameStarted){ return;}
        
        // 1. geen order -> move order
        // 2. move order -> move order
        // 3. build order ->
        
                    
        if (currentOrder == GetComponent<BuildOrder>())
        {
            if (currentBuilding != null)
            {
                PhotonNetwork.Destroy(currentBuilding);
            }
            currentOrder = null;
            return;
        }
        Vector3 position = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPosition = hit.point;
            foreach (GameObject selectedUnit in selectedUnits)
            {
                BaseUnit unit = selectedUnit.GetComponent<BaseUnit>();
                
                
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
                unit.ordersList[0].status = Node.Status.Success;
                unit.ordersList[0].enabled = false;
                unit.ordersList.Clear();
            }
            unit.orderTargetPositions.Clear();
            
            unit.state = UnitState.Idle;
        }
        if (unit.ordersList.Count == 0)
        {
            foreach (OrderBase script in unit.GetComponents<OrderBase>())
            {
                script.status = Node.Status.Success;
            }
        }
        unit.SendOrder(order, targetPosition); 
        
        // unit.behaviorAgent.BlackboardReference.SetVariableValue("CurrentOrder", unit.ordersList[0]);
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