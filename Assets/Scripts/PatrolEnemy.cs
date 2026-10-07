using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public Transform[] points;
    float speed = 1;

    int currentPoint = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        if (points.Length == 0) return;

        Transform targetPoint = points[currentPoint];

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (targetPoint.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (targetPoint.position.x < transform.position.x)
        {
            
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPoint++;
            if (currentPoint >= points.Length)
            {
                currentPoint = 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = 
                collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.takeHit(10);
            }
        }
    }
}
