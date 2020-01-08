using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Character")]
public class Characters : ScriptableObject
{
    public string characterName = "Default";
    public GameObject prefab;
}