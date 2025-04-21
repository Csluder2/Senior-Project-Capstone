using Csluder2.FusionWork;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class OnlineHitBox : NetworkBehaviour

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float damage = 10f; // Damage dealt by the hitbox
    public OnlineHealthBar opponentHealth;
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            GameObject healthBarObj = GameObject.Find("Canvas/Health Bar 2");
            if (healthBarObj != null)
            {
                opponentHealth = healthBarObj.GetComponent<OnlineHealthBar>();
                Debug.Log("Health bar 2 assigned!");
            }
        }
        else
        {
            GameObject healthBarObj = GameObject.Find("Canvas/Health Bar 1");
            if (healthBarObj != null)
            {
                opponentHealth = healthBarObj.GetComponent<OnlineHealthBar>();
                Debug.Log("Health bar 1 assigned!");
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Opponent")) // Ensure the opponent has the correct tag
        {
            Debug.Log("Collider hit!");

            if (opponentHealth != null)
            {
                opponentHealth.takeDamage(damage, opponentHealth);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}