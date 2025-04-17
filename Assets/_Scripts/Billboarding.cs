using UnityEngine;

public class Billboarding : MonoBehaviour
{
    void Update()
    {
        transform.forward = -Camera.main.transform.position;       
    }
}
