using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
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
        AsyncOperation LoadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);
        yield return null;
    }
}
