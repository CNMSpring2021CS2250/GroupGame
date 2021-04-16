using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPSInput")]

public class FPSInputJump : MonoBehaviour
{
    // Variables for Speed, Gravity
    public float speed = 6.0f;
    public float gravity = -9.8f;

    // Jump Code
    public float jumpSpeed = 15.0f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;

    private float _vertSpeed;

    //public const float baseSpeed = 6.0f;

    // Reference to Character Controller
    private CharacterController _charController;


    // // Messenger code
    // void Awake()
    // {
    //     Messenger<float>.AddListener(GameEvent.SPEED_CHANGED, OnSpeedChanged);
    // }

    // void OnDestroy()
    // {
    //     Messenger<float>.RemoveListener(GameEvent.SPEED_CHANGED, OnSpeedChanged);
    // }

    // private void OnSpeedChanged(float value)
    // {
    //     speed = baseSpeed * value;
    // }


    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();    //_charController is a pointer to a class object (not a reference?)
        _vertSpeed = minFall;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        // Jump Code
        if (_charController.isGrounded)
        {
            if (Input.GetButtonDown("Jump")) // Spacebar
            {
                _vertSpeed = jumpSpeed;
            }
            else
            {
                _vertSpeed = minFall;
            }
        }
        else
        {
            _vertSpeed += gravity * 5 * Time.deltaTime;

            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity; // negative numbers are faster when falling
            }
        }

        //movement.y = gravity;
        movement.y = _vertSpeed; // JUMP CODE CHANGE

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);

        //transform.Translate(0, speed, 0);
        //transform.Translate(deltaX * Time.deltaTime, 0, deltaZ * Time.deltaTime);

        _charController.Move(movement);
    }
}
