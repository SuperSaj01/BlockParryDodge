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

    [SerializeField] private SpawnPointSO testingSpawnPoints; //to be changed to a list of spawn points

    //private static Dictionary<PlayerManager, ulong> playerDict = new Dictionary<PlayerManager, ulong>();
    
    private void Awake() 
    {
        DontDestroyOnLoad(this);

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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Testing", LoadSceneMode.Single);

           
        Debug.Log("Scene Loaded");
        
        OnLoadSceneEvent?.Invoke();
        

        /*if (playerDict.Count > 0)
        {
            foreach (var player in playerDict.Keys)
            {
                player.transform.position = testingSpawnPoints.spawnPoints[0];
            }
        } */
        yield return null;   
    }

    public void LoadStartScene()
    {
        StartCoroutine(LoadStartSceneCoroutine());
    }

    private IEnumerator LoadStartSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        

        Debug.Log("Scene Loaded");
        
        yield return null;   
    }
    #endregion

    #region RPCs
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
            //GameMenu.instance.OpenGlobalMenu();
        }
    }
    [ClientRpc]
    void CloseMenuClientRpc()
    {
        if(IsOwner)
        {
            OnCloseGlobalMenuCalled?.Invoke();
            //GameMenu.instance.CloseMenu();
        }
    }
    [ServerRpc]
    public void RespawnServerRpc()
    {
        //OnRespawnEvent?.Invoke(); // Custom function to reset health, posture, etc.
        CloseMenuClientRpc();
    }
    #endregion



   /* #region[Player Management]
    public void AddPlayer(PlayerManager player, ulong playerId)
    {
        if (!playerDict.ContainsKey(player))
        {
            playerDict.Add(player, playerId);
        }
        else
        {
            Debug.LogWarning($"Player with ID {player} already exists in the dictionary.");
        }

    }

    public ulong GetPlayerId(PlayerManager playerManager)
    {
        if (playerDict.TryGetValue(playerManager, out ulong playerId))
        {
            return playerId;
        }
        return 0; // Return null if the player is not found
    }
    #endregion */
}
