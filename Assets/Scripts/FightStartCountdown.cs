using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FightStartCountdown : MonoBehaviour
{
    public Text countdownText;
    public GameObject player1, player2; // Assign both players in Inspector
    public float countdownTime = 3f;

    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        // Disable Player Movement
        player1.GetComponent<OnlinePlayerCombat>().enabled = false;
        player2.GetComponent<OnlinePlayerCombat>().enabled = false;

        for (int i = (int)countdownTime; i > 0; i--)
        {
            countdownText.text = i.ToString(); // Display 3, 2, 1
            yield return new WaitForSeconds(1f);
        }

        // Show "FIGHT!"
        countdownText.text = "FIGHT!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false); // Hide text after fight starts

        // Enable Player Movement
        player1.GetComponent<PlayerCombat>().enabled = true;
        player2.GetComponent<PlayerCombat>().enabled = true;
    }
}

