using UnityEngine;

public class HitBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public float damage = 10f; // Damage dealt by the hitbox
    public healthBar opponentHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Opponent")) // Ensure the opponent has the correct tag
        {
            Debug.Log("Collider hit!");

            if (opponentHealth != null)
            {
                opponentHealth.takeDamage(damage);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
