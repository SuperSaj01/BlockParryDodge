using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerDatabase
{
    private static Dictionary<ulong, CharacterManager> playerLookup = new Dictionary<ulong, CharacterManager>();

    public static void AddPlayer(ulong clientID, CharacterManager player)
    {
         if (!playerLookup.ContainsKey(clientID))
                playerLookup.Add(clientID, player);
            else
                Debug.LogWarning($"Duplicate Player ID {clientID} found in {player.name}!");

        Debug.Log("Player added to database: " + player.name + " with ID: " + clientID);
    }

    public static CharacterManager GetPlayerByID(ulong id)
    {
        return playerLookup.TryGetValue(id, out var player) ? player : null;
    }
       
}