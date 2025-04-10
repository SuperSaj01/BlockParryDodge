using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameMenu : MonoBehaviour
{
    
    [Header("Menu")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject sliders;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    public bool isInMenu = false;

    private void Awake() 
    {

    }

    void Start()
    {   
        menuPanel.SetActive(false);
        sliders.SetActive(true);
        respawnButton.gameObject.SetActive(false);

        WorldManager.instance.OnOpenGlobalMenuCalled += OpenGlobalMenu;
        WorldManager.instance.OnCloseGlobalMenuCalled += CloseMenu;
    }

     #region Menu
    public void ToggleLocalMenu()
    {
        if(menuPanel.activeSelf)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        isInMenu = true;
        menuPanel.SetActive(true);
        sliders.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        respawnButton.gameObject.SetActive(false); // No respawn option in local menu

            
    }

    public void CloseMenu()
    {   
        isInMenu = false;
        menuPanel.SetActive(false);
        sliders.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void OpenGlobalMenu()
    {
        OpenMenu();
        respawnButton.gameObject.SetActive(true); // Respawn option in global menu
    }
    
    void ContinueGame()
    {
        ToggleLocalMenu();
    }

    public void Respawn()
    {
        WorldManager.instance.RespawnServerRpc();
    }


    public void QuitToMainMenu()
    {  
        CloseMenu();
        Cursor.visible = true;
        QuitGame();   
    }

    void QuitGame()
    {
        // Logic to quit to main menu
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
