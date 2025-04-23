using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CharacterType")]
public class CharacterSO : ScriptableObject
{
    //A data container containing all the variables a character type will have
    [SerializeField] public int characterTypeID;

    public string characterName;
    public float health;
    public float posture;
    public float stamina;
    public float resistance;

    public float parryWindow;
    public float rollWindow;

    public GameObject characterHead;
}
