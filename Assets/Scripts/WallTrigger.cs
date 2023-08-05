using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    SnakeMovement snakeMovement;
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            snakeMovement.SwitchState(SnakeMovement.SnakeState.Dead);
            //Time.timeScale = 0f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
