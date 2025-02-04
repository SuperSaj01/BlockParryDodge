using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class WorldManager : NetworkBehaviour
{
    public static WorldManager instance { get; private set; }
    [SerializeField] int worldSceneIndex = 1;
    
    private void Awake() 
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

    private void Start() 
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadNewGameBTN()
    {
        StartCoroutine(LoadNewGame());
    }

    public IEnumerator LoadNewGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Testing", LoadSceneMode.Single);
        yield return null;
    }
}
