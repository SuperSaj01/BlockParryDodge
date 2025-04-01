using System.Collections.Generic;
using UnityEngine;

public static class CharacterDatabase
{
    private static Dictionary<int, CharacterSO> characterTypeLookup = new Dictionary<int, CharacterSO>();

    // Static constructor (called once when the class is accessed)
    static CharacterDatabase()
    {
        LoadCharacterType();
    }

    private static void LoadCharacterType()
    {
        characterTypeLookup.Clear();

        // Load all characters from Resources folder
        CharacterSO[] allCharacterTypes = Resources.LoadAll<CharacterSO>("CharacterTypes");

        foreach (var type in allCharacterTypes)
        {
            if (!characterTypeLookup.ContainsKey(type.characterTypeID))
                characterTypeLookup.Add(type.characterTypeID, type);
            else
                Debug.LogWarning($"Duplicate type ID {type.characterTypeID} found in {type.characterName}!");
        }

    }

    public static CharacterSO GetCharacterTypeByID(int id)
    {
        return characterTypeLookup.TryGetValue(id, out var type) ? type : null;
    }
}
