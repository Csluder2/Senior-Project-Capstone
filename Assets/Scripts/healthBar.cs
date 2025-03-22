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
    public PlayerCombat opponentCombat;
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
    public void takeDamage(float damage)
    {
        health -= damage;
        playerCombat.TriggerHitStun();
        if (health <= 0)
        {
            playerCombat.TriggerDefeat();
            opponentCombat.TriggerVictory();
            bool perfect = opponentHealth.health == opponentHealth.maxHealth;
            StartCoroutine(EndFight(perfect));
        }
    }

    private IEnumerator EndFight(bool perfect)
    {
        if (winScreen != null)
        {

            yield return new WaitForSeconds(2f);
            if (gameObject.name.Contains("Y Bot"))
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
