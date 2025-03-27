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

    ///Summary of the method
    ///This method is used to authenticate the player and assign a unique name
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

    ///Summary of the method
    ///This method is used to keep the lobby alive as no calls to the lobby will result in a shutdown of the lobby
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
    ///Summary of the method
    ///This method is used to check the lobby for updates every second
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
            //Creates default options for the lobby
            string lobbyName = "myLobby" + UnityEngine.Random.Range(0, 100); 
            int maxPlayers = 2;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions{
                IsPrivate = false, //allows it to be found by anyone
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>{ //Creates a new lobby with the data of the lobby code and relay code 
                    {RELAYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, "0")},
                    {LOBBYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, "0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions); //Creates the lobby with the options provided

            hostLobby = lobby;
            joinedLobby = hostLobby;

            lobbyCode = hostLobby.LobbyCode;
            

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
                Count = 5, //returns 5 lobbies
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) //returns lobbies with available slots greater than 0
                },
                Order = new List<QueryOrder>
                { 
                    new QueryOrder(false, QueryOrder.FieldOptions.Created) //orders the lobbies by the time they were created
                }
            };

            QueryResponse lobbyListQueryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions); //queries the lobbies with the options provided
         
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results }); 
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    #region Buttons

    ///Summary of the method
    ///This method is used to join a lobby by the code in input field for quick input
    public void JoinLobbyBtn()
    {
        string joinCode = inputField.GetComponent<TMP_InputField>().text;
        Debug.Log("Joining lobby with code " + joinCode);
        JoinLobbyByCode(joinCode);
    }

    public async void JoinLobby(Lobby lobby) {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
            Player = player //Creates a new player object in the lobby with the data of the clients player
        });
    }


    public void CreateLobbyBTN(string joinCode)
    {
        codeText.GetComponent<TMP_Text>().text = joinCode; 
    }
    #endregion 
    
    #region Lobby Functions
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions  = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions); //We join the lobby with the code and assign the player to the lobby
            
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

    #region Player functions
    ///Summary of the method
    ///This method is used to grab the player object to be used in the lobby
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

    #endregion

    ///Summary of the method
    ///This method is used to update the lobby code
    private async void UpdateLobbyCode()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions //finds the lobby with the same id and assigns the new options
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
    

    ///Summary of the method
    ///This method is used to change the name of a player in the lobby
    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, //finds the player with the same id in the same lobby id and assigns the new options
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

    ///Summary of the method
    ///A getter for the bool to check if the client is the host of the lobby
    private bool IsLobbyHost()
    {
        if(AuthenticationService.Instance.PlayerId == joinedLobby.HostId) return true;
        else return false;
    }

    ///Summary of the method
    ///This method is used to start the game for the host
    public async void StartGameBTN()
    {
        relayCode = await CreateRelay(); //Will not be sending the relay code ONLY lobby code // This makes the host spawn

            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    //Update the lobby data with the new relay code
                    //This will be used by the clients when they detect a change in the relay code indicating the start of the game
                    //the relay code is used to start as a client on the same relay server as the host
                    {RELAYCODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayCode)} 
                }
            });

            joinedLobby = hostLobby; //Updates the joined lobby to the hosts lobby
    }

    private void StartRestofClients()
    {
        if(joinedLobby.Data[RELAYCODE_KEY].Value != "0")
            {
                if(!IsLobbyHost())
                {
                    JoinRelay(joinedLobby.Data[RELAYCODE_KEY].Value);
                    //For player joined in lobby (not the host) if the relay code is not 0, join the relay server 
                }

                joinedLobby = null; //Resets the joined lobby (the player is no longer in a lobby and is now playing in the server)
            }
    }


    #region Relay
    ///Summary of the method
    ///This method is used to create a relay server
    private async Task<string> CreateRelay()
    {
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); //This gets the relay code to join the relay server

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost(); //The host of the lobby is now the host of the game

            return joinCode; //returns the relay code to be used by the clients to join the relay server
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
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode); //Joins the relay server with the relay code

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
            joinAllocation.RelayServer.IpV4,
            (ushort)joinAllocation.RelayServer.Port,
            joinAllocation.AllocationIdBytes,
            joinAllocation.Key,
            joinAllocation.ConnectionData,
            joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient(); //The client is now connected to the relay server
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    #endregion

}
