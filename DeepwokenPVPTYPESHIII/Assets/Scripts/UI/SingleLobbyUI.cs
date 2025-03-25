using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class SingleLobbyUI : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;


    private Lobby lobby;


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            LobbyManager.instance.JoinLobbyByCode(lobbyCodeText);
        });
    }

    public void UpdateLobby(Lobby lobby) {
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        lobbyCodeText.text = lobby.Data[LobbyManager.RELAYCODE_KEY].Value; //change to lobby
    }
}
