using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

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