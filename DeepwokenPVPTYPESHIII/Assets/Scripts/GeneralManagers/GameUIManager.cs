using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEditor.Search;

public class GameUIManager : MonoBehaviour
{

    public static GameUIManager instance { get; private set; }

    public GameMenu gameMenu;
    public GameObject healthSlider;
    public GameObject postureSlider;
    public GameObject staminaSlider;
    
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;



    private void Awake()
    {
        //Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        //For debugging purposes I can start a host instantly without needing to make a lobby
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

}