using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class LobbyManager : MonoBehaviour
{

    public static LobbyManager instance { get; private set; }

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;

    private string RELAYCODE_KEY = "RELAYCODE_KEY";
    public string LOBBYCODE_KEY {get; private set;} = "LOBBYCODE_KEY";
    private string lobbyCode;
    private string relayCode;


    private string playerName;
    public bool createLobbyPrivate = false;

    public GameObject inputField;
    public GameObject codeText;

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }

    void Awake()
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
                    {RELAYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, "0")},
                    {LOBBYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, "0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyname, maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            lobbyCode = hostLobby.LobbyCode;
            PrintPlayers(hostLobby);
            

            CreateLobbyBTN(hostLobby.LobbyCode);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }

        UpdateLobbyCode();        
        
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

            QueryResponse lobbyListQueryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
         
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
            Debug.Log("Invoked");
            Debug.Log(lobbyListQueryResponse.Results.Count);
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

    public async void JoinLobby(Lobby lobby) {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
            Player = player
        });
    }

    public void CreateLobbyBTN(string joinCode)
    {
        codeText.GetComponent<TMP_Text>().text = joinCode; 
    }
    #endregion 
    
    #region  Lobby Functions -- Joining
    public async void JoinLobbyByCode(string lobbyCode)
    {
        Debug.Log(lobbyCode);
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

    private async void UpdateLobbyCode()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {LOBBYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, lobbyCode)}
                }
            });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
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


#region Temppossivble

   

#endregion

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
