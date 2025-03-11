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

    private static Dictionary<PlayerManager, ulong> playerDict = new Dictionary<PlayerManager, ulong>();
    
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
            foreach (var player in playerDict.Keys)
            {
                player.transform.position = testingSpawnPoints.spawnPoints[0];
            }
        }
        yield return null;   
    }
    #endregion

    #region[Player Management]
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

        Debug.Log(playerDict.Count + playerDict[player].ToString());
    }

    public ulong GetPlayerId(PlayerManager playerManager)
{
    if (playerDict.TryGetValue(playerManager, out ulong playerId))
    {
        return playerId;
    }
    return 0; // Return null if the player is not found
}
    #endregion
}
