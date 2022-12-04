using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public float HorizontalSensitivity = 30.0f;
    public float VerticalSensitivity = 30.0f;
    private float _mouseX = 0.0f, _mouseY = 0.0f;
    public Camera _camera;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotationX = HorizontalSensitivity * _mouseX * Time.deltaTime;
        float rotationY = VerticalSensitivity * _mouseY * Time.deltaTime;
        Vector3 camRot = _camera.transform.rotation.eulerAngles;
        camRot.x += rotationX;
        camRot.y += rotationY;
        _camera.transform.rotation = Quaternion.Euler(camRot);
    }

    public void OnLook(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();
        _mouseX = input.x;
        _mouseY = input.y;
    }
}
