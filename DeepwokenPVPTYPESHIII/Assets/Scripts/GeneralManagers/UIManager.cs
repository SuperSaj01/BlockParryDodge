using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Transform lobbySingleTemplate;
    [SerializeField] private Transform container;


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

        
    }

    void OnEnable()
    {
        LobbyManager.instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
    }

    void OnDisable()
    {
        LobbyManager.instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList) 
    {
        foreach (Lobby lobby in lobbyList) 
        {
            Transform lobbySingleTransform = Instantiate(lobbySingleTemplate);
            lobbySingleTransform.gameObject.SetActive(true);
            SingleLobbyUI lobbyListSingleUI = lobbySingleTransform.GetComponent<SingleLobbyUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
        }
    }
}
