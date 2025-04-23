using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class WorldManager : NetworkBehaviour
{
    public static WorldManager instance { get; private set; }

    public event Action OnLoadSceneEvent;
    
    public event Action OnRespawnEvent;

    public event Action OnOpenGlobalMenuCalled;
    public event Action OnCloseGlobalMenuCalled;

    public bool isAlreadySignedIn = false;

    
    private void Awake() 
    {
        DontDestroyOnLoad(this);

        //Singleton pattern
        if(instance == null)
        {
            instance = this;
        }    
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() 
    {
        DontDestroyOnLoad(gameObject);
    }

    #region Scenes
    public void StartCoroutineLoadNewGameBTN()
    {
        StartCoroutine(LoadNewGame());
    }

    public IEnumerator LoadNewGame()
    {
        //Loads game scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Testing", LoadSceneMode.Single);

           
        Debug.Log("Scene Loaded");
        
        //Tells other scripts listening (will be players)
        OnLoadSceneEvent?.Invoke();
        
        yield return null;   
    }

    public void LoadStartScene()
    {
        StartCoroutine(LoadStartSceneCoroutine());
    }

    private IEnumerator LoadStartSceneCoroutine()
    {
        //Load the menu scene back
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        

        Debug.Log("Scene Loaded");
        
        yield return null;   
    }
    #endregion

    #region RPCs
    /// summary of the methods
    /// These are called by server to call a method for every client.
    [ServerRpc]
    public void OpenDeathMenuServerRpc()
    {
        if(IsServer)
        {   
            OpenMenuClientRpc();
        }  
    }

    [ClientRpc]
    void OpenMenuClientRpc()
    {
        if (IsOwner)
        {
            OnOpenGlobalMenuCalled?.Invoke();
        }
    }
    [ClientRpc]
    void CloseMenuClientRpc()
    {
        if(IsOwner)
        {
            OnCloseGlobalMenuCalled?.Invoke();
        }
    }
    [ServerRpc]
    public void RespawnServerRpc()
    {
        CloseMenuClientRpc();
    }
    #endregion
}
