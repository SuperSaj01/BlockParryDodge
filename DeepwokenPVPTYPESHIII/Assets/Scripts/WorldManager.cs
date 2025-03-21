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
    public event Action OnPlayerJoinedGame;

    [SerializeField] private SpawnPointSO testingSpawnPoints; //to be changed to a list of spawn points

    
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

        yield return null;   
    }
    #endregion

    #region[Player Management]
    #endregion
}
