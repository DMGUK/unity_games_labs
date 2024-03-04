using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private float lookX;
    private float lookY;
    public Camera mainCamera;
    public float lookSpeed = 2f;
    public InputActions input;
    private InputAction move;
    private InputAction look;
    private int count;
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    private bool gameStopped = false;
    private GameObject endPickup;
    private GameObject deadlyPickup;
    private GameObject[] pickup;

    private void Awake()
    {
        input = new InputActions();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        move = input.Player.Move;
        move.Enable();
        move.performed += OnMove;
        move.canceled += OnMove;
        look = input.Player.Look;
        look.Enable();
        look.performed += OnLook;
        move.canceled += OnMove;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        move.Disable();
        move.performed -= OnMove;
        move.canceled -= OnMove;
        look.Disable();
        look.performed -= OnLook;
        look.canceled -= OnLook;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        endPickup = GameObject.FindGameObjectWithTag("EndPickUp");
        endPickup.SetActive(false);
        deadlyPickup = GameObject.FindGameObjectWithTag("DeadlyPickUp");
        pickup = GameObject.FindGameObjectsWithTag("PickUp");
        foreach (GameObject p in pickup)
        {
            p.SetActive(true);
        }
        deadlyPickup.SetActive(true);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Vector2 movementVector = context.ReadValue<Vector2>();

            movementX = movementVector.x;
            movementY = movementVector.y;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            movementX = 0;
            movementY = 0;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookVector = context.ReadValue<Vector2>();

        lookX = lookVector.x;
        lookY = lookVector.y;

        float rotationX = mainCamera.transform.localEulerAngles.y + lookX * lookSpeed;
        float rotationY = mainCamera.transform.localEulerAngles.x - lookY * lookSpeed;
        mainCamera.transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
    }

    private void FixedUpdate()
    {
        if (!gameStopped)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 movement = cameraForward * movementY + cameraRight * movementX;
            movement.Normalize();
            rb.AddForce(movement * speed);
            rb.AddForce(Physics.gravity * rb.mass);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!gameStopped)
        {
            if (other.gameObject.CompareTag("PickUp"))
            {
                other.gameObject.SetActive(false);
                count++;

                if (count == 4)
                {
                    if (endPickup != null)
                    {
                        endPickup.SetActive(true);
                    }
                }
            }
            else if (other.gameObject.CompareTag("DeadlyPickUp"))
            {
                other.gameObject.SetActive(false);
                loseTextObject.SetActive(true);
                StopGame();
            }
            else if (other.gameObject.CompareTag("EndPickUp"))
            {
                other.gameObject.SetActive(false);
                winTextObject.SetActive(true);
                StopGame();
            }
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }

    void StopGame()
    {
        gameStopped = true;
        move.Disable();
        look.Disable();
        Time.timeScale = 0f;
    }

    void RestartGame()
    {
        gameStopped = false;
        Time.timeScale = 1f;
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        count = 0;
        SetCountText();
        move.Enable();
        look.Enable();
        transform.position = new Vector3(-8.5f, 0.5f, 6.5f);
        foreach (GameObject p in pickup)
        {
            p.SetActive(true);
        }
        deadlyPickup.SetActive(true);
    }

    void Update()
    {
        if (gameStopped && Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }
    }
}
