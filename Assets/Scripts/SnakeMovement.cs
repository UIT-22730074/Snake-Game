using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class SnakeMovement : MonoBehaviour
{
    private Vector3 direction = Vector3.right;
    private SnakeInput snakeInput;
    List<Transform> segments;
    public Transform segmentsPrefab;
    private float nextUpdate;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    private Vector3Int input;
    public bool moveThroughWalls = false;
    bool isZ = true;
    bool isUpdate = false;
    private void Awake()
    {
        snakeInput = new SnakeInput();
        segments = new List<Transform>();
        isUpdate = true;
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
        ResetState();
    }
    void Update()
    {
        if (isUpdate)
        {
            if (!isZ)
            {
                if (snakeInput.Movement.W.triggered)
                {
                    direction = new Vector3(0f, 0f, 1f);
                    transform.rotation = Quaternion.Euler(-90, 90, 0);
                    isZ = true;
                    isUpdate = false;
                }
                else if (snakeInput.Movement.S.triggered)
                {
                    direction = new Vector3(0f, 0f, -1f);
                    transform.rotation = Quaternion.Euler(-90, -90, 0);
                    isZ = true;
                    isUpdate = false;
                }
            }
            else
            {
                if (snakeInput.Movement.A.triggered)
                {
                    direction = new Vector3(-1f, 0f, 0f);
                    transform.rotation = Quaternion.Euler(-90, 0, 0);
                    isZ = false;
                    isUpdate = false;
                }

                else if (snakeInput.Movement.D.triggered)
                {
                    direction = new Vector3(1f, 0f, 0f);

                    transform.rotation = Quaternion.Euler(-90, -180, 0);
                    isZ = false;
                    isUpdate = false;
                }
            }
        }
       
        
       
    }
   
    private void FixedUpdate()
    {
        if (Time.time < nextUpdate)
        {
            return;
        }
        if (input != Vector3Int.zero)
        {
            direction = input;
        }
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }
        float x = Mathf.RoundToInt(transform.position.x) + direction.x;
        float z = Mathf.RoundToInt(transform.position.z) + direction.z;
        transform.position = new Vector3(x, 1f,z);
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
        isUpdate = true;    
    }
    public void Grow()
    {
        Transform segment = Instantiate(segmentsPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }
    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.z) == y)
            {
                return true;
            }
        }

        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body"))
        {
            Debug.Log("Loss");
        }
    }
    public void ResetState()
    {
        direction = new Vector3(0,0f,1f);

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
        }
    }

}
