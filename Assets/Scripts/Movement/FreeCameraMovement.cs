using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Helper;
using UniRx;

public class FreeCameraMovement : MonoBehaviour
{
    [Header("Camera movement")]
    [SerializeField]
    private float _moveSpeed;
    [SerializeField]
    private float _jumpSpeed;

    [Header("Camera rotation")]
    [SerializeField]
    private float _rotateSpeed = 0.005f;
    [SerializeField]
    private Vector2 _cameraDeadZone = new Vector2(0.15f, 0.15f);

    [Header("Rotate curve")]
    [SerializeField]
    private AnimationCurve _rotationCurveX;
    [SerializeField]
    private AnimationCurve _rotationCurveY;

    // Rotation
    private float _yaw = 0f;
    private float _pitch = 0f;

    private Vector3 _direction;
    private Vector3 _lookAt;
    private float _jump;

    private MainControl _mainControl;

    private Vector3 _startingMousePos;

    private void Awake()
    {
        // Locking the cursor to the camera.
        // TODO Input manager
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        /// Creating a new control, serialized from the SimpleControls.inputactions
        _mainControl = new MainControl();

        _mainControl.CameraControls.Movement.started += Movement;
        _mainControl.CameraControls.Movement.performed += Movement;
        _mainControl.CameraControls.Movement.canceled += Movement;

        _mainControl.CameraControls.LookAt.started += LookAt;
        _mainControl.CameraControls.LookAt.performed += LookAt;
        _mainControl.CameraControls.LookAt.canceled += LookAt;

        _mainControl.CameraControls.Jump.started += Jump;
        _mainControl.CameraControls.Jump.performed += Jump;
        _mainControl.CameraControls.Jump.canceled += Jump;

        _mainControl.CameraControls.Click.started += Click;
        _mainControl.CameraControls.Click.performed += Click;
        _mainControl.CameraControls.Click.canceled += Click;

        _mainControl.Enable();
    }

    private void OnDestroy()
    {
        _mainControl.CameraControls.Movement.started -= Movement;
        _mainControl.CameraControls.Movement.performed -= Movement;
        _mainControl.CameraControls.Movement.canceled -= Movement;

        _mainControl.CameraControls.LookAt.started -= LookAt;
        _mainControl.CameraControls.LookAt.performed -= LookAt;
        _mainControl.CameraControls.LookAt.canceled -= LookAt;

        _mainControl.CameraControls.Jump.started -= Jump;
        _mainControl.CameraControls.Jump.performed -= Jump;
        _mainControl.CameraControls.Jump.canceled -= Jump;

        _mainControl.CameraControls.Click.started -= Click;
        _mainControl.CameraControls.Click.performed -= Click;
        _mainControl.CameraControls.Click.canceled -= Click;
    }

    // Add spacebar to "jump" :).
    private void Movement(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed)
        {
            _direction = context.ReadValue<Vector2>();
        }
        else
        {
            _direction = Vector2.zero;
        }
    }

    private void LookAt(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed)
        {
            _lookAt = context.ReadValue<Vector2>();
            _lookAt.z = Camera.main.nearClipPlane;
        }
        else
        {
            _lookAt = Vector2.zero;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed)
        {
            _jump = context.ReadValue<float>();
        }
        else
        {
            _jump = 0;
        }
    }

    
    private void Click(InputAction.CallbackContext context)
    {
        // Specific to mouse input, but should handle joystick
        if (context.phase == InputActionPhase.Started)
        {
            _startingMousePos = Mouse.current.position.ReadValue();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            _startingMousePos = Vector3.zero;
        }
    }

    private void Update()
    {
        LookAtMouse();
    }

    private void FixedUpdate()
    {
        float jump = _jump * _jumpSpeed * Time.deltaTime;

        Vector3 directionForward = new Vector3(1 * _direction.x, jump, 1 * _direction.y);
        transform.Translate(directionForward * Time.deltaTime * _moveSpeed);
    }

    // TOdO gérer la différence 
    private void LookAtMouse()
    {
        if (_startingMousePos == Vector3.zero)
            return;

        // TODO faire 
        Vector3 mouseDirection = _lookAt - _startingMousePos;
        Vector2 movement = Vector2.zero;

        // We calculate if a deadzone is crossed, to move or not the camera
        if (Mathf.Abs(mouseDirection.x) > _cameraDeadZone.x)
        {
            movement.x = mouseDirection.x;
        }

        if (Mathf.Abs(mouseDirection.y) > _cameraDeadZone.y)
        {
            movement.y = mouseDirection.y;
        }

        Vector3 mappedPosition = new Vector3();      
        mappedPosition.x = MathHelper.Map(0, Screen.width, 0, 1, movement.x);
        // TODO have a way to invert that via menu
        mappedPosition.y = MathHelper.Map(0, Screen.height, 0, 1, movement.y) * -1;

        float curveX = _rotationCurveX.Evaluate(Mathf.Abs(mappedPosition.x)) * Mathf.Sign(mappedPosition.x);
        float curveY = _rotationCurveY.Evaluate(Mathf.Abs(mappedPosition.y)) * Mathf.Sign(mappedPosition.y);

        _yaw += _rotateSpeed * curveX * Time.deltaTime;
        _pitch += _rotateSpeed * curveY * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -90, 90);

        transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
    }
}
