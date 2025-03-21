using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabse
{
    private static Dictionary<int, WeaponSO> weaponLookup = new Dictionary<int, WeaponSO>();

    // Static constructor (called once when the class is accessed)
    static ItemDatabse()
    {
        LoadWeapons();
    }

    private static void LoadWeapons()
    {
        weaponLookup.Clear();

        // Load all weapons from Resources folder
        WeaponSO[] allWeapons = Resources.LoadAll<WeaponSO>("Weapons");

        foreach (var weapon in allWeapons)
        {
            if (!weaponLookup.ContainsKey(weapon.itemID))
                weaponLookup.Add(weapon.itemID, weapon);
            else
                Debug.LogWarning($"Duplicate Weapon ID {weapon.itemID} found in {weapon.name}!");
        }
    }

    public static WeaponSO GetWeaponByID(int id)
    {
        return weaponLookup.TryGetValue(id, out var weapon) ? weapon : null;
    }
}
