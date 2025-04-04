using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviourPun
{
    public static UIManager Instance;
    
    public GameObject constructionPanel;
    public GameObject orderPanel;

    public Button[] orderButtons;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        constructionPanel.SetActive(false);

        orderButtons = orderPanel.GetComponentsInChildren<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
