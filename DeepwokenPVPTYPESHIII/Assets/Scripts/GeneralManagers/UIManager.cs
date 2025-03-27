using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Transform lobbySingleTemplate;
    [SerializeField] private Transform parent;

    private List<Lobby> temporaryFoundLobbies = new List<Lobby>();


    public static UIManager instance;
    void Start()
    {
    
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        
        LobbyManager.instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            UpdateLobbyList(temporaryFoundLobbies);
        }
    }


    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        if(e.lobbyList == null) return; //Stops any null reference exceptions
        UpdateLobbyList(e.lobbyList);
        temporaryFoundLobbies = e.lobbyList;
    }

    private void UpdateLobbyList(List<Lobby> lobbyList) 
    {
        foreach(Transform child in parent)
        {
            Destroy(child);
        }

        foreach (Lobby lobby in lobbyList) //For each lobby in the list it will create a UI element and place its position relative to its order 
        {
            Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, parent);
            SingleLobbyUI lobbyListSingleUI = lobbySingleTransform.GetComponent<SingleLobbyUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
            Debug.Log(lobby.Data[LobbyManager.instance.LOBBYCODE_KEY].Value);
        }
    }
}
