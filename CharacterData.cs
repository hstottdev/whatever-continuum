using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character",menuName = "My Assets/Character",order = 0)]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public Color frameColor = Color.white;
}
