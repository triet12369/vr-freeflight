using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // public references
    public Rigidbody player;
    public GameObject rightHandRayInteractor;
    public GameObject aimLine;
    public bool enableAimLine;

    private PlayerStateController playerState;
    private HandPoseController handPoseController;

    // constants
    public float MovementSpeed = 20.0f;
    public float JumpStrength = 200.0f;
    public float HorizontalSensitivity = 30.0f;
    public float VerticalSensitivity = 30.0f;
    public float FlyingSpeed = 20.0f;
    public float MaxVelocity = 200.0f;


    private float _moveX, _moveY;
    private float _mouseX = 0.0f, _mouseY = 0.0f;
    private Camera _camera;


    private float intrinsicDrag = 0.5f;
    private float dampenDrag = 2.0f;
    private float dragDelta = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // attach controllers
        playerState = GetComponent<PlayerStateController>();
        handPoseController = GetComponent<HandPoseController>();

        // initialize hand ray
        //rightHandRayInteractor.SetActive(false);
        aimLine.SetActive(false);

        // set state change handler
        playerState.OnFlyingForwardStart = OnFlyingForwardStart;
        handPoseController.OnRPointingStart = OnRPointingStart;
        handPoseController.OnRPointingEnd = OnRPointingEnd;
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

        //aimLine.transform.position = rayInteractor.Origin;
        //aimLine.transform.rotation = rayInteractor.Rotation;
        aimLine.transform.position = handPoseController.rightHand.position;
        aimLine.transform.rotation = handPoseController.rightHand.rotation;
    }

    private void FixedUpdate()
    {
        Vector3 movement = transform.right * _moveX + transform.forward * _moveY;
        player.AddForce(movement * MovementSpeed * player.mass);

        if (playerState.CurrentState == PlayerStateController.PLAYER_STATE.FLYING_FORWARD)
        {
            player.drag = Mathf.Clamp(player.drag - dragDelta, intrinsicDrag, dampenDrag);
            Vector3 forward = handPoseController.GetForwardDirection();
            player.AddForce(forward * FlyingSpeed * player.mass);
        }

        if (playerState.CurrentState == PlayerStateController.PLAYER_STATE.HOVERING)
        {
            float hoverForce = - (player.velocity.y + Physics.gravity.y);
            player.AddForce(Vector3.up * hoverForce, ForceMode.Acceleration);
        }

        // clamp velocity
        player.velocity = Vector3.ClampMagnitude(player.velocity, MaxVelocity);
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
        //Vector2 input = inputValue.Get<Vector2>();
        //_mouseX = input.x;
        //_mouseY = input.y;
    }

    public void OnEscape(InputValue inputValue)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnFlyingForwardStart()
    {
        player.drag = dampenDrag;
    }

    private void OnRPointingStart()
    {
        //player.AddForce(new Vector3(0, 1000, 0) * player.mass);
        //rightHandRayInteractor.SetActive(true);
        if (enableAimLine) aimLine.SetActive(true);
    }

    private void OnRPointingEnd()
    {
        //Debug.Log("OnRPointingEnd");
        if (enableAimLine) aimLine.SetActive(false);
    }
}
