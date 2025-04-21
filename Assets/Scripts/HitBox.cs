using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public float damage = 10f; // Damage dealt by the hitbox
    public healthBar opponentHealth;
    bool isPlayer2;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Opponent")) // Ensure the opponent has the correct tag
        {
            if (opponentHealth.gameObject.name == "Health Bar 2")
                isPlayer2 = true;
            Debug.Log("Collider hit!");

            if (opponentHealth != null)
            {
                opponentHealth.takeDamage(damage, isPlayer2);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
