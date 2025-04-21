using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{


    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    private float lerpSpeed = 0.05f;
    public PlayerCombat playerCombat;
    public Player2CombatScript opponentCombat;
    public healthBar playerHealth;
    public healthBar opponentHealth;

    public GameObject winScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthSlider != null && healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (healthSlider != null && healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
        }
    }
    public void takeDamage(float damage, bool isPlayer2)
    {
        if (isPlayer2 == true)
        {
            if (opponentCombat.isBlocking == true)
            {
                health -= (damage - 5);
                opponentCombat.TriggerBlock();
            }
            else
            {
                health -= damage;
                opponentCombat.TriggerHitStun();
            }
            if (health <= 0)
            {
                playerCombat.TriggerVictory();
                opponentCombat.TriggerDefeat();
                bool perfect = playerHealth.health == playerHealth.maxHealth;
                StartCoroutine(EndFight(perfect));
            }
        }
        else
        {
            if (playerCombat.isBlocking == true)
            {
                health -= (damage - 5);
                playerCombat.TriggerBlock();
            }
            else
            {
                health -= damage;
                playerCombat.TriggerHitStun();
            }
            if (health <= 0)
            {
                playerCombat.TriggerDefeat();
                opponentCombat.TriggerVictory();
                bool perfect = opponentHealth.health == opponentHealth.maxHealth;
                StartCoroutine(EndFight(perfect));
            }
        }
    }

    private IEnumerator EndFight(bool perfect)
    {
        if (winScreen != null)
        {

            yield return new WaitForSeconds(2f);
            if (playerHealth.health == 0)
                winScreen.GetComponent<Text>().text = "PLAYER 2 WINS!";
            else
            {
                winScreen.GetComponent<Text>().text = "PLAYER 1 WINS!";
            }
            yield return new WaitForSeconds(2f);
            if (perfect)
                winScreen.GetComponent<Text>().text += "\n PERFECT!";


        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainMenu");
    }



}
