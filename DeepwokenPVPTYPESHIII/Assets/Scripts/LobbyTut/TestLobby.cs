using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System.Threading.Tasks;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;

    private string RELAYCODE_KEY = "RELAYCODE_KEY";
    private string relayCode;


    private string playerName;
    public bool createLobbyPrivate = false;

    public GameObject inputField;
    public GameObject codeText;


    private void Start()
    {
        playerName = "User" + UnityEngine.Random.Range(0, 100);
        Authenticate(playerName);
    }

    public async void Authenticate(string playerName)
    {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyHeartbeat()
    {
        if(hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 27f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    private async void HandleLobbyPollForUpdates()
    {
        if(joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if(lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }

            StartRestofClients();
        }
        
    }
    public async void CreateLobby()
    {
        try
        {
            string lobbyname = "myLobby";
            int maxPlayers = 2;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions{
                IsPrivate = false,
                Player = GetPlayer(),
                //Can add a lobby data for different gamemodes. Might add in future
                Data = new Dictionary<string, DataObject>{
                    {RELAYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyname, maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            Debug.Log("We created: " + lobby.Name);
            Debug.Log(hostLobby.LobbyCode); // how to access lobby code;
            
            PrintPlayers(hostLobby);
            

            CreateLobbyBTN(hostLobby.LobbyCode);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    private async void UpdateLobby()
    {
        Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                {RELAYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
            }
        });

        joinedLobby = lobby;
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                { 
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Lobbies found " + queryResponse.Results.Count);
            foreach(Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    #region Buttons

    public void JoinLobbyBtn()
    {
        string joinCode = inputField.GetComponent<TMP_InputField>().text;
        Debug.Log("Joining lobby with code " + joinCode);
        JoinLobbyByCode(joinCode);
    }
    public void CreateLobbyBTN(string joinCode)
    {
        codeText.GetComponent<TMP_Text>().text = joinCode; 
    }
    #endregion 
    
    #region  Lobby Functions -- Joining
    public async void JoinLobbyByCode(string lobbyCode)
    {
        Debug.Log("Joining lobby " + lobbyCode);
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions  = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void QuickJoinLobby()
    {
        try{
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    #endregion 


    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby " + lobby.Name);
        foreach(var player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    public void LeaveLobby()
    {
        try{
            LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                    }
                };
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId,
            new UpdatePlayerOptions 
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName)}
                }
            });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private bool IsLobbyHost()
    {
        if(AuthenticationService.Instance.PlayerId == joinedLobby.HostId) return true;
        else return false;
    }
//Starting Game ------------------------------------#endregion

    public async void StartGameBTN()
    {
        relayCode = await CreateRelay(); //we will not be sending the relay code ONLY lobby code // This makes the host spawn

            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {RELAYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                }
            });

            joinedLobby = hostLobby;
    }

    private void StartRestofClients()
    {
        if(joinedLobby.Data[RELAYCODE_KEY].Value != "0")
            {
                if(!IsLobbyHost())
                {
                    JoinRelay(joinedLobby.Data[RELAYCODE_KEY].Value);
                }

                joinedLobby = null;
            }
    }


//RELAY --------------------------------------------#endregion
    private async Task<string> CreateRelay()
    {
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    async void JoinRelay(string joinCode)
    {
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
            joinAllocation.RelayServer.IpV4,
            (ushort)joinAllocation.RelayServer.Port,
            joinAllocation.AllocationIdBytes,
            joinAllocation.Key,
            joinAllocation.ConnectionData,
            joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    

}
