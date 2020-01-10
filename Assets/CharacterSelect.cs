using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public GameObject Player1;
    public Vector3 Player1Position;

    public GameObject Player2;
    public Vector3 Player2Position;

    public Characters[] characters;
    public GameObject characterSelectPanel;
    public void OnCharacterSelect(int characterChoice)
    {
        if(Player1 == null)
        {
            GameObject spawnedPlayer = Instantiate(Player1, Player1Position, Quaternion.identity) as GameObject;
            characterSelectPanel.SetActive(false);
            Characters selectedCharacter = characters[characterChoice];
        }
        if (Player1 != null && Player2 == null)
        {
            GameObject spawnedPlayer = Instantiate(Player2, Player2Position, Quaternion.identity) as GameObject;
            characterSelectPanel.SetActive(false);
            Characters selectedCharacter = characters[characterChoice];
        }
    }
}