using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class SelectionManager : MonoBehaviourPun, PlayerInputs.IGameplayActions
{
    public OrderBase currentOrder;
    public Vector3 targetPosition;
    [SerializeField] private string standardOrder;
    
    private List<GameObject> selectedUnits = new List<GameObject>();
    private PlayerInputs inputAsset;

    void Awake()
    {
        inputAsset = new PlayerInputs();
        
        inputAsset.Gameplay.AddCallbacks(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        inputAsset.Enable();
    }

    void OnDisable()
    {
        inputAsset.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }
    //TODO make it so i can interact with whatever is selected
    public void OnSelect(InputAction.CallbackContext context)
    {
        // print(context.control);
        if(!context.performed){ return; }

        Vector3 position = Vector3.zero;
        position.x = Mouse.current.position.ReadValue().x;
        position.y = Mouse.current.position.ReadValue().y;
        // print(position);

        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        
        Physics.Raycast(ray, out hit);

        
        ISelectable selectable = hit.collider.GetComponent<ISelectable>();
        PhotonView selectablePhotonView = hit.collider.GetComponent<PhotonView>();
        
        // print(hit.collider.gameObject.name);
        if (selectable == null)
        {
            selectedUnits.Clear();
            return;
        }
        
        if (selectablePhotonView == null || !selectablePhotonView.IsMine)
        {
            Debug.LogError("Object does not have a PhotonView component");
            selectedUnits.Clear();
            return;
        }
        
        
        GameObject selectedUnit = hit.collider.gameObject;

        if (selectedUnit != null)
        {
            if (!selectedUnits.Contains(selectedUnit))
            {
                print("Selected unit " + selectedUnit.name);
                selectedUnits.Add(selectedUnit);
                selectable.OnSelect(gameObject);
                return;
            }
            Debug.LogError("Object is already selected");
        }
  
           
    }

    public void OnShiftClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnSendOrder(InputAction.CallbackContext context)
    {
        if(!context.performed){ return; }
        
        print("Trying to do order");
        Vector3 position = Vector3.zero;
        position.x = Mouse.current.position.ReadValue().x;
        position.y = Mouse.current.position.ReadValue().y;
            
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        
        Physics.Raycast(ray, out hit);
            
        this.targetPosition = hit.point;
                
        foreach (GameObject selectedUnit in selectedUnits)
        {
            ISelectable selectable = selectedUnit.GetComponent<ISelectable>();
            BaseUnit unit = selectedUnit.GetComponent<BaseUnit>();
            if (currentOrder != null)
            {
                unit.orderTargetPositions.Clear();
                unit.ordersList.Clear();
                
                selectable.SendOrder(currentOrder, targetPosition);
            }
            else
            {
                MoveOrder moveOrder = OrderManager.Instance.gameObject.GetComponent<MoveOrder>();
                
                unit.orderTargetPositions.Clear();
                unit.ordersList.Clear();
                
                selectable.SendOrder(moveOrder, targetPosition);
            }
        }
    }
}
