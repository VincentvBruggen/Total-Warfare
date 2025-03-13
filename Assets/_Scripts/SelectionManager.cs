using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class SelectionManager : MonoBehaviourPun, PlayerInputs.IGameplayActions
{
    PlayerInputs inputAsset;

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
    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed && photonView.IsMine)
        {
            print("mouse clicked");
            Vector3 position = Vector3.zero;
            position.x = Mouse.current.position.ReadValue().x;
            position.y = Mouse.current.position.ReadValue().y;
            print(position);

            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit hit;
            
            Physics.Raycast(ray, out hit);

            ISelectable selectable = hit.collider.GetComponent<ISelectable>();
            PhotonView selectablePhotonView = hit.collider.GetComponent<PhotonView>();
            if(selectable == null && selectablePhotonView == null || !selectablePhotonView.IsMine) { return; }
            
            selectable.OnSelect(gameObject);
        }
    }
}
