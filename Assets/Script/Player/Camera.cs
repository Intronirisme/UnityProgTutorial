using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    //public CameraControls CamControls { get; private set; }
    public Vector2 MinMaxDistance = new Vector2(4.0f, 8.0f);
    public Vector2 TurnSpeed = new Vector2(220, 140);
    public float LookUpLimit = 80.0f;
    //public float WallDistanceDetection = 0.0f; //enabled if > 0.0f

    private GameObject _cam_root;
    private Vector2 _lookInput;
    // Start is called before the first frame update
    void Start()
    {
        _cam_root = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camRotation = _cam_root.transform.rotation.eulerAngles;
        camRotation += new Vector3
        (
            TurnSpeed.y * Time.deltaTime * _lookInput.y,
            TurnSpeed.x * Time.deltaTime * _lookInput.x,
            0
        );
        camRotation.x = Mathf.Clamp(camRotation.x, 0, LookUpLimit);
        _cam_root.transform.rotation = Quaternion.Euler(camRotation);
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        _lookInput = value.ReadValue<Vector2>();
        Debug.Log(_lookInput);
    }
}
