using System.Collections;
using System.Collections.Generic;
using Csluder2.FusionWork;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineHealthBar : NetworkBehaviour
{


    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;

    [Networked] public float health { get; set; }
    private float lerpSpeed = 0.05f;
    public OnlinePlayerCombat playerCombat;
    public OnlinePlayerCombat opponentCombat;
    public OnlineHealthBar opponentHealth;
    public GameObject winScreen;





    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void Spawned()
    {
        base.Spawned();
        health = maxHealth;
        Debug.Log("Spawned called");
        StartCoroutine(assignHealthBar());
    }
    private IEnumerator assignHealthBar()
    {
        yield return new WaitForSeconds(3f); // Wait for both fighters to spawn
        var players = FindObjectsOfType<OnlinePlayerCombat>();
        if (players.Length == 2)
        {
            foreach (var player in players)
            {
                Debug.Log("foreach loop started");
                if (player.HasStateAuthority)
                {
                    playerCombat = player;
                    Debug.Log("Player is linked to healthbar! animations should work");
                }
                else
                {
                    opponentCombat = player;
                    Debug.Log("Opponent is linked to healthbar! animations should work");
                }

            }


        }

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
    public void takeDamage(float damage, OnlineHealthBar playerHealth)
    {
        // Only the player with state authority should change the actual health value.
        if (HasStateAuthority)
        {
            Debug.Log("StateAuthority player attack landed!");

            if (playerHealth.gameObject == GameObject.Find("Canvas/Health Bar 2"))
            {

                if (opponentCombat.isBlocking == true)
                {
                    health -= (damage - 5);
                    opponentCombat.RPC_TriggerBlock();
                }
                else
                {
                    health -= damage;
                    opponentCombat.RPC_TriggerHitStun();
                }
                Debug.Log("This is healthbar 2. player 2 should be in hitstun.");

                if (health <= 0)
                {
                    Debug.Log("WINNER IS PLAYER 1");


                    bool perfect = false;
                    bool player1Win = true;
                    playerCombat.RPC_TriggerVictory();
                    opponentCombat.RPC_TriggerDefeat();
                    StartCoroutine(EndFight(perfect, player1Win));
                }

            }
            else
            {



                Debug.Log("This is healthbar 1. player 1 should be in hitstun.");
                if (playerCombat.isBlocking == true)
                {
                    health -= (damage - 5);
                    playerCombat.RPC_TriggerBlock();
                }
                else
                {
                    health -= damage;
                    playerCombat.RPC_TriggerHitStun();
                }
                if (health <= 0)
                {
                    Debug.Log("WINNER IS PLAYER 2");

                    bool perfect = false;
                    bool player1Win = false;
                    playerCombat.RPC_TriggerDefeat();
                    opponentCombat.RPC_TriggerVictory();
                    StartCoroutine(EndFight(perfect, player1Win));
                }

            }



        }



    }



    private IEnumerator EndFight(bool perfect, bool player1Win)
    {
        if (winScreen != null)
        {

            yield return new WaitForSeconds(2f);
            if (player1Win == true)
                RPC_DisplayVictoryText("PLAYER 1 WINS!");
            else
            {
                RPC_DisplayVictoryText("PLAYER 2 WINS!");
            }
            yield return new WaitForSeconds(2f);
            if (perfect)
                winScreen.GetComponent<Text>().text += "\n PERFECT!";


        }
        yield return new WaitForSeconds(2f);
        Runner.LoadScene(SceneRef.FromIndex(2));

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DisplayVictoryText(string victorText)
    {
        if (winScreen != null)
            winScreen.GetComponent<Text>().text = victorText;
    }


}
