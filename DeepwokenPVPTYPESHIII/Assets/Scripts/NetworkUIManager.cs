using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private void Awake()
    {
        
        hostBtn.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartHost();
        });
        
        clientBtn.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartClient();
        });
    }

}