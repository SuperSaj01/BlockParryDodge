using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    
    public static UIManager instance;
    
    public bool isInMenu = false;
    public LobbyMenu? lobbyMenu;
    
    void Start()
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

}

