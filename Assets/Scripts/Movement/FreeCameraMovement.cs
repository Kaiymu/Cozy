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

    private MainControl _mainControl;

    private Vector2 _mousePos;

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
    
    // Reactif :D.

    private void Update()
    {
        LookAtMouse();

        Vector3 directionForward = new Vector3(1 * _direction.x, 0f, 1 * _direction.y);
        this.transform.Translate(directionForward * Time.deltaTime * _moveSpeed);
    }

    // TODO peut-?tre faire un diff entre current mousPos & newPos. 
    private void LookAtMouse()
    {
        _mousePos.x = MathHelper.Map(0, Screen.width, -1, 1, _lookAt.x);
        // TODO have a way to invert that.
        _mousePos.y = MathHelper.Map(0, Screen.height, -1, 1, _lookAt.y) * -1;

        if (Mathf.Abs(_mousePos.x) < _cameraDeadZone.x && Mathf.Abs(_mousePos.y) < _cameraDeadZone.y)
            return;

        float curveX = _rotationCurveX.Evaluate(Mathf.Abs(_mousePos.x))* Mathf.Sign(_mousePos.x);
        float curveY = _rotationCurveX.Evaluate(Mathf.Abs(_mousePos.y)) * Mathf.Sign(_mousePos.y);

        _yaw += _rotateSpeed * curveX * Time.deltaTime;
        _pitch += _rotateSpeed * curveY * Time.deltaTime;
        transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
    }
}
