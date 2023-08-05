using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private SnakeMovement snake;
    [SerializeField]
    private ParticleSystem fire;
    Vector3 oldPos = Vector3.zero;
    private bool first = true;
    [SerializeField]
    AudioSource eat;
    private void Awake()
    {
        snake = FindObjectOfType<SnakeMovement>();
    }
    private void Start()
    {
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        if (first)
        {
            first = false;
        }
        else
        {
            fire.transform.position = oldPos;
            fire.Play();

        }
    
        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        int x = Mathf.RoundToInt(Random.Range(-10, 9));
        int y = Mathf.RoundToInt(Random.Range(-12, 7));

        // Prevent the food from spawning on the snake
        while (snake.Occupies(x, y))
        {
            x++;
            if (x > 9)
            {
                x = Mathf.RoundToInt(-10);
                y++;

                if (y > 7)
                {
                    y = Mathf.RoundToInt(-12);
                }
            }
        }


        oldPos = new Vector3(x, 1f, y);
        transform.position = oldPos;
    }


    private void OnTriggerEnter(Collider other)
    {
        RandomizePosition();
        snake.Grow();
        eat.Play();
    }
}
