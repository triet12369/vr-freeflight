using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody player;
    // constants
    public float MovementSpeed = 20.0f;
    public float JumpStrength = 200.0f;
    public float HorizontalSensitivity = 30.0f;
    public float VerticalSensitivity = 30.0f;


    private float _moveX, _moveY;
    private float _mouseX = 0.0f, _mouseY = 0.0f;
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        float rotationX = HorizontalSensitivity * _mouseX * Time.deltaTime;
        float rotationY = VerticalSensitivity * _mouseY * Time.deltaTime;
        Vector3 camRot = _camera.transform.rotation.eulerAngles;
        gameObject.transform.Rotate(new Vector3(0f, rotationX, 0f), Space.World);
        gameObject.transform.Rotate(new Vector3(-rotationY, 0f, 0f), Space.Self);
    }

    private void FixedUpdate()
    {
        Vector3 movement = transform.right * _moveX + transform.forward * _moveY;
        player.AddForce(movement * MovementSpeed * player.mass);
    }

    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        _moveX = inputVec.x;
        _moveY = inputVec.y;
    }

    public void OnJump(InputValue input)
    {
        Vector3 movement = new Vector3(0, JumpStrength, 0);
        player.AddForce(movement * player.mass);
    }

    public void OnLook(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();
        _mouseX = input.x;
        _mouseY = input.y;
    }

    public void OnEscape(InputValue inputValue)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
