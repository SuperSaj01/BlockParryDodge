using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items")]
public class ItemSO : ScriptableObject
{
    //A data container containing all the variables an item type will have
    [SerializeField] private string name;
    [SerializeField] public int itemID;
    public GameObject itemPrefab; //The visual object that will be spawned in the world
    public Vector3 offset; //The offset applied to the object when spawned to properly be placed in world space
}
