using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public Transform player1Spawn, player2Spawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int player1Index = PlayerPrefs.GetInt("Player1Character", 0);
        int player2Index = PlayerPrefs.GetInt("Player2Character", 1);

        Instantiate(characterPrefabs[player1Index], player1Spawn.position, Quaternion.identity);
        Instantiate(characterPrefabs[player2Index], player2Spawn.position, Quaternion.identity);
    }

}



