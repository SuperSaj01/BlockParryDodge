using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CharacterType")]
public class CharacterSO : ScriptableObject
{

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
