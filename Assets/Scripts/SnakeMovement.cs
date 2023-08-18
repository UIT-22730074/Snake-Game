using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class SnakeMovement : MonoBehaviour
{
    // Snake movement direction
    private Vector3 direction = Vector3.right;

    // Input handling
    private SnakeInput snakeInput;

    // List of snake body segments
    List<Transform> segments;

    // Prefab for snake body segments
    public Transform segmentsPrefab;

    // Timing and speed variables
    private float nextUpdate;
    public float speed = 20f;
    public float speedMultiplier = 1f;

    // Initial size of the snake
    public int initialSize = 4;

    // Input direction vector
    private Vector3Int input;

    // Flags to track movement in 2D plane
    bool isZ = true;
    bool isUpdate = false;

    // References to various game elements
    [SerializeField] ParticleSystem dead;
    [SerializeField] Material deadMaterial;
    [SerializeField] AudioSource lose;

    // Possible states of the snake
    public enum SnakeState
    {
        Normal, Dead, Lose, Start
    }
    SnakeState currentState = SnakeState.Start;

    // Audio sources for gameplay and lobby
    [SerializeField] AudioSource playgame;
    [SerializeField] AudioSource lobby;

    // Array to store previous positions of snake segments
    Vector3[,] previousPositions;

    // UI elements
    [SerializeField] GameObject startUI;
    [SerializeField] GameObject playUI;
    [SerializeField] GameObject LoseUI;

    // Current point count
    private int point = -2;

    // Text fields to display points
    [SerializeField] TextMeshProUGUI textPoint;
    [SerializeField] TextMeshProUGUI textPointL;

    // Initial position of the snake
    public Vector3 startpos = Vector3.one;

    // Method to start the game
    public void PlayGame()
    {
        ResetState();
        SwitchState(SnakeState.Normal);
    }

    // Method to return to the start screen
    public void Ok()
    {
        SwitchState(SnakeState.Start);
    }

    // Method to switch between different game states
    public void SwitchState(SnakeState state)
    {
        switch (state)
        {
            case SnakeState.Normal:
                {
                    playUI.SetActive(true);
                    startUI.SetActive(false);
                    lobby.Stop();
                    playgame.Play();
                    Time.timeScale = 1;
                    break;
                }
            case SnakeState.Dead:
                {
                    playUI.SetActive(false);

                    // Restore previous positions of segments
                    for (int i = 0; i < segments.Count; i++)
                    {
                        try
                        {
                            segments[i].position = previousPositions[0, i];
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Error restoring position: " + e.Message);
                        }
                    }

                    // Reset snake's position
                    transform.position = oldPosSnake;

                    // Play death effects and sounds
                    dead.Play();
                    playgame.Stop();
                    lose.Play();

                    // Trigger dead state animation
                    StartCoroutine(SetDead());
                    break;
                }
            case SnakeState.Lose:
                {
                    LoseUI.SetActive(true);
                    Time.timeScale = 0;
                    break;
                }
            case SnakeState.Start:
                {
                    LoseUI.SetActive(false);
                    startUI.SetActive(true);
                    lobby.Play();
                    break;
                }
        }
        currentState = state;
    }

    // Animation parameter
    float amount = 1f;

    // List to store previous positions for animation
    List<Vector3> lastPos = new List<Vector3>();

    // Coroutine to animate death
    IEnumerator SetDead()
    {
        textPointL.text = point.ToString();
        while (amount >= 0)
        {
            amount -= Time.deltaTime;
            deadMaterial.SetFloat("_Amount", amount);
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1f);
        SwitchState(SnakeState.Lose);
    }
    private void Awake()
    {
        startpos = transform.position;
        snakeInput = new SnakeInput();
        segments = new List<Transform>();
        isUpdate = true;
        dead.gameObject.SetActive(true);
        SwitchState(SnakeState.Start);
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
        switch (currentState)
        {
            case SnakeState.Normal:
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
                    break;
                }
            default: break;
            
        }


    }
    Vector3 oldPosSnake= Vector3.zero;
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case SnakeState.Normal:
                {
                    if (Time.time < nextUpdate)
                    {
                        return;
                    }
                    if (input != Vector3Int.zero)
                    {
                        direction = input;
                    }
                    for (int i = 0; i < segments.Count; i++)
                    {
                        previousPositions[1, i] = previousPositions[0, i];
                        previousPositions[0, i] = segments[i].position;
                    }
                    for (int i = segments.Count - 1; i > 0; i--)
                    {
                        segments[i].position = segments[i - 1].position;
                    }
                    oldPosSnake = transform.position;
                    float x = Mathf.RoundToInt(transform.position.x) + direction.x;
                    float z = Mathf.RoundToInt(transform.position.z) + direction.z;
                    transform.position = new Vector3(x, 1f, z);
                    nextUpdate = Time.time + (1f / (speed * speedMultiplier));
                    isUpdate = true;
                    break;
                }
            default: break;
        }

    }
    public void Grow()
    {
        point++;
        textPoint.text = point.ToString();
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
        if (currentState.Equals(SnakeState.Normal))
        {
            if (other.CompareTag("Body"))
            {

                SwitchState(SnakeState.Dead);
            }

        }
      
    }
    public void ResetState()
    {
        transform.position = startpos;
        isUpdate = true;
        point = -2;
        textPoint.text = point.ToString();
         isZ = true;
        transform.rotation = Quaternion.Euler(-90, 90, 0);
        previousPositions = new Vector3[2, 5000];
        for (int i = 0; i < segments.Count; i++)
        {
            previousPositions[0, i] = segments[i].position;
            previousPositions[1, i] = segments[i].position;
        }
        amount = 1f;
        deadMaterial.SetFloat("_Amount", 1f);
        direction = new Vector3(0, 0f, 1f);

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
