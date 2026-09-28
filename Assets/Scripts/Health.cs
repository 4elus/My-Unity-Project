using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    private const int MAX_HEALTH = 100;
    public void takeHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }



    public void setHealth(int health)
    {
        this.health += health;

        // Данное условие отвечает за то, чтобы наше здоровье не стало > 100
        if (health > 100)
        {
            health = MAX_HEALTH;
        }
    }




    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
