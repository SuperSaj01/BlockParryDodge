using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class MenuManager : MonoBehaviour
{
    public void StartNetworkHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        StartCoroutine(WorldManager.instance.LoadNewGame());
    }
}
