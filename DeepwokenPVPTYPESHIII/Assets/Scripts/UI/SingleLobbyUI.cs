using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class SingleLobbyUI : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    string code;


    private Lobby lobby;


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            LobbyManager.instance.JoinLobbyByCode(code); //When button is clicked it calls join lobby feeding in the code
        });
    }

    public void UpdateLobby(Lobby lobby) {
        
        //Assigns all the data to UI elements and refers the button to the specific lobby 
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
        lobbyCodeText.text = lobby.Data[LobbyManager.instance.LOBBYCODE_KEY].Value;
        code = lobby.Data[LobbyManager.instance.LOBBYCODE_KEY].Value;
    }
}
