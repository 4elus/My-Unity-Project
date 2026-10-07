using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;



public class Player : MonoBehaviour
{

    public float speed = 5;
    Rigidbody2D rigidbody;
    int[] numbers = new int[5] {9, 11, -2, 3, 14};

    public GameObject[] gameObjects = new GameObject[3];
    public Transform spawnPoint;
    public bool isJump = false;
    private Animator animator;
    private SpriteRenderer spr;

    private bool isHurt = false;
    public float knockbackForce = 0f;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log( name(2, 3));
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        // Debug.Log(string.Join("," , gameObjects.ToString()));

        //Vector2 position = spawnPoint != null ? spawnPoint.position : transform.position;
        //Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        //for (int i = 0; i < gameObjects.Length; i++)
        //{
        //    Debug.Log($"GameObject {i}: {gameObjects[i].name}");
        //    Instantiate(gameObjects[0], position, rotation);
        //    position.x += 2; // Move the spawn position to the right for the next object
        //}    
    }

    // Update is called once per frame
    void Update()
    {

        float move = Input.GetAxis("Horizontal");

        if (move != 0)
        {
            animator.SetInteger("State", 1);
        }
        else
        {
            animator.SetInteger("State", 0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);
            //animator.SetInteger("State", 1);
            spr.flipX = true;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);
            //animator.SetInteger("State", 1);
            spr.flipX = false;
        }
      

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJump)
            {
                isJump = true;
                rigidbody.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
            }
           
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;
        }

        if (collision.gameObject.CompareTag("Enemy") && !isHurt)
        {
            isHurt= true;
            collision.gameObject.GetComponent<Health>().takeHit(20);
            Debug.Log("Enemy hit! Remaining health: " + 
                collision.gameObject.GetComponent<Health>().health);

            // Apply knockback force to the player
            float direction = transform.position.x -  collision.transform.position.x;
            direction = Mathf.Sign(direction); // Get the direction of knockback
            rigidbody.linearVelocity = Vector2.zero; // Reset current velocity

            rigidbody.AddForce(new Vector2(direction * knockbackForce, 4f), ForceMode2D.Impulse);
        }
    }

    private bool name(int a, int b)
    {
        return a > b;
    }
}
