using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class UIManager : MonoBehaviour
{

    [SerializeField] private Transform lobbySingleTemplate;
    [SerializeField] private Transform parent;

    private List<Lobby> temporaryFoundLobbies = new List<Lobby>();

    [Header("Menu")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

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

        if(LobbyManager.instance == null) return;
        LobbyManager.instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;

        menuPanel.SetActive(false);
        respawnButton.gameObject.SetActive(false);

        continueButton.onClick.AddListener(ContinueGame);
        quitButton.onClick.AddListener(QuitToMainMenu);
        respawnButton.onClick.AddListener(Respawn);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            UpdateLobbyList(temporaryFoundLobbies);
        }
    }

    #region Lobbies
    
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
    #endregion

    #region Menu
    public void ToggleLocalMenu()
    {
        if(menuPanel.activeSelf)
        {
            Debug.Log("Hiding the menu...");
            CloseMenu();
        }
        else
        {
            Debug.Log("Showing the menu...");
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        respawnButton.gameObject.SetActive(false); // No respawn option in local menu

            
    }

    public void CloseMenu()
    {   
        menuPanel.SetActive(false);
        Cursor.visible = false;

    }

    public void OpenGlobalMenu()
    {
        OpenMenu();
        respawnButton.gameObject.SetActive(true); // Respawn option in global menu
    }
    
    void ContinueGame()
    {
        CloseMenu();
    }

    void Respawn()
    {
        //RespawnServerRpc();
    }

    

    void QuitToMainMenu()
    {  
        QuitGame();   
    }

    void QuitGame()
    {
        // Logic to quit to main menu
        CloseMenu();
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        WorldManager.instance.LoadStartScene();
        Debug.Log("You quit");
    }

    #endregion

}

