using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabse
{
     private static List<WeaponSO> sortedWeapons = new List<WeaponSO>();

    static ItemDatabse()
    {
        LoadWeapons();
    }

    private static void LoadWeapons()
    {
        sortedWeapons.Clear();

        WeaponSO[] allWeapons = Resources.LoadAll<WeaponSO>("Weapons");

        foreach (var weapon in allWeapons)
        {
            if (!sortedWeapons.Exists(w => w.itemID == weapon.itemID))
            {
                sortedWeapons.Add(weapon);
            }
            else
            {
                Debug.LogWarning($"Duplicate Weapon ID {weapon.itemID} found in {weapon.name}!");
            }
        }

        // Sort the list using Merge Sort
        sortedWeapons = MergeSort(sortedWeapons);
    }

    // 🔁 Merge Sort (based on itemID)
    private static List<WeaponSO> MergeSort(List<WeaponSO> list)
    {
        if (list.Count <= 1) return list;

        int mid = list.Count / 2;
        var left = MergeSort(list.GetRange(0, mid));
        var right = MergeSort(list.GetRange(mid, list.Count - mid));

        return Merge(left, right);
    }

    private static List<WeaponSO> Merge(List<WeaponSO> left, List<WeaponSO> right)
    {
        List<WeaponSO> result = new List<WeaponSO>();
        int i = 0, j = 0;

        while (i < left.Count && j < right.Count)
        {
            if (left[i].itemID < right[j].itemID)
                result.Add(left[i++]);
            else
                result.Add(right[j++]);
        }

        while (i < left.Count) result.Add(left[i++]);
        while (j < right.Count) result.Add(right[j++]);

        return result;
    }

    // 🔍 Binary Search by itemID
    public static WeaponSO GetWeaponByID(int id)
    {
        int index = BinarySearch(sortedWeapons, id);
        return index != -1 ? sortedWeapons[index] : null;
    }

    private static int BinarySearch(List<WeaponSO> list, int targetID)
    {
        int left = 0;
        int right = list.Count - 1;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            int midID = list[mid].itemID;

            if (midID == targetID)
                return mid;
            else if (midID < targetID)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return -1;
    }
}
