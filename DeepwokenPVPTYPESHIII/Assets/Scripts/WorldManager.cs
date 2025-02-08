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

    [SerializeField] private SpawnPointSO testingSpawnPoints; //to be changed to a list of spawn points

    private Dictionary<uint, PlayerManager> playerDict = new Dictionary<uint, PlayerManager>();
    
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

    #region[Scenes]
    public void StartCoroutineLoadNewGameBTN()
    {
        StartCoroutine(LoadNewGame());
    }

    public IEnumerator LoadNewGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Testing", LoadSceneMode.Single);
           
        Debug.Log("Scene Loaded");
        
        OnLoadSceneEvent?.Invoke();
        

        if (playerDict.Count > 0)
        {
            foreach (var player in playerDict.Values)
            {
                player.transform.position = testingSpawnPoints.spawnPoints[0];
            }
        }
        yield return null;   
    }
    #endregion

    #region[Player Management]
    public void AddPlayer(ulong playerId, PlayerManager player)
    {
        if (!playerDict.ContainsKey((uint)playerId))
        {
            playerDict.Add((uint)playerId, player);
        }
        else
        {
            Debug.LogWarning($"Player with ID {playerId} already exists in the dictionary.");
        }

        Debug.Log(playerDict.Count + playerDict[(uint)playerId].name);
    }
    #endregion
}
