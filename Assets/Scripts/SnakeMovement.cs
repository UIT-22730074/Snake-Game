using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    private Vector3 direction = Vector3.right;
    private SnakeInput snakeInput;
    private void Awake()
    {
        snakeInput = new SnakeInput();
    }
    private void OnEnable()
    {
        snakeInput.Enable();
    }
    private void OnDisable()
    {
        snakeInput.Disable();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (snakeInput.Movement.W.triggered)
        {
            direction = new Vector3(0f, 0f, 1f);
        }
        else if (snakeInput.Movement.A.triggered)
        {
            direction = new Vector3(-1f, 0f, 0f);
        }
        else if (snakeInput.Movement.S.triggered)
        {
            direction = new Vector3(0f, 0f, -1f);
        }
        else if (snakeInput.Movement.D.triggered)
        {
            direction = new Vector3(1f, 0f, 0f);
        }
    }
}
