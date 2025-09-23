using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController Controller;
    public Transform Cam;
    public float speed = 12f;
    public float jumpForce = 5;
    public float gravity = -9.81f;

    public Vector3 velocity, move;
    private Vector2 MovementInput;

    public bool isGrounded;

    private Player PlayerInput;

    private Vector3 LastPos = Vector3.zero;

    private bool StopAudio = true;
    public static bool IsPaused = false;
    public static int SprintAdditive;

    public float RayDist;

    private void Awake()
    {
        //Cam = Camera.main.transform;
        Controller = GetComponent<CharacterController>();
        PlayerInput = new();
        IsPaused = false;
    }
    private void Start()
    {

    }

    private void OnEnable()
    {
        PlayerInput.Enable();
    }
    private void OnDisable()
    {
        PlayerInput.Disable();
    }

    void Update()
    {
        if (IsPaused)
            return;

        isGrounded = Controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        /*if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravity);
        }*/

        if (Input.GetKey(KeyCode.LeftShift))
            SprintAdditive = 2;
        else
            SprintAdditive = 1;

        MovementInput = PlayerInput.Main.Move.ReadValue<Vector2>();
        move = Cam.forward * MovementInput.y + Cam.right * MovementInput.x;
        move.y = 0;
        Controller.Move(speed * SprintAdditive * Time.deltaTime * move);

        velocity = AdjustVelocityToSlope(velocity);
        velocity.y += gravity * Time.deltaTime;

        Controller.Move(velocity * Time.deltaTime);

        if (LastPos != gameObject.transform.position)
        {
            if (!StopAudio)
            {
                StopAudio = true;
            }
        }
        else if (StopAudio || Time.timeScale == 0)
        {
            StopAudio = false;
        }

        LastPos = gameObject.transform.position;
    }

    public Vector3 AdjustVelocityToSlope(Vector3 velo)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit rch, RayDist))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, rch.normal);
            var adjustedVelo = slopeRotation * velo;

            if (adjustedVelo.y < 0)
            {
                return adjustedVelo;
            }
        }

        return velo;
    }

    public void ToggleMovement(bool toggle)
    {
        IsPaused = toggle;
    }
}
