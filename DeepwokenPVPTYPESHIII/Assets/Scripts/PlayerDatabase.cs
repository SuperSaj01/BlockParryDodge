using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerDatabase
{
    private static Dictionary<ulong, PlayerManager> playerLookup = new Dictionary<ulong, PlayerManager>();

    public static void AddPlayer(ulong clientID, PlayerManager player)
    {
         if (!playerLookup.ContainsKey(clientID))
                playerLookup.Add(clientID, player);
            else
                Debug.LogWarning($"Duplicate Player ID {clientID} found in {player.name}!");
    }

    public static PlayerManager GetPlayerByID(ulong id)
    {
        return playerLookup.TryGetValue(id, out var player) ? player : null;
    }
       
}
