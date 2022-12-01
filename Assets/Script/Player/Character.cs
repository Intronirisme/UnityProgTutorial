using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    [Header("Walking")]
    public float Speed = 5f;
    public float TurnRate = .5f;
    public float LookupRate = .5f;
    public float GroundDamping = 2f;

    [Header("Jumping")]
    public float JumpSpeed = 3f;
    public float JumpDuration = .8f;
    public float MaxFallingSpeed = 4f;
    public float GravityAcceleration = 9.81f;

    [Header("Air control")]
    public float AirControl = .3f;
    public float AirDamping = .5f;


    //references vers les autres composants
    private CharacterController _controls;
    private GameObject _body;
    private CapsuleCollider _caps;
    private GameObject _cam_root;
    private GameObject _cam;

    //inputs & déplacements
    private Vector2 _moveInput; //récupère l'input du joueur
    private Vector2 _lookInput;
    private Vector3 _playerMove; //input converti en déplacement
    private float _iJumping = 0.0f;
    private Vector3 _physicMove; //déplacements causés par la physique

    //booleen ayant pour but d'éviter l'input bouncing
    private bool _iInteracting = false;

    // Start is called before the first frame update
    void Start()
    {
        //assignations des références vers nos autres composants
        _controls = gameObject.GetComponent<CharacterController>();
        _body = gameObject.FindInChildren("Body");
        _caps = _body.GetComponent<CapsuleCollider>();
        _cam_root = gameObject.FindInChildren("CameraRoot");
        _cam = _cam_root.FindInChildren("Camera");

        //verrouillage et masquage du curseur de la souris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float damping = _controls.isGrounded ? GroundDamping : AirDamping;

        GetPlayerMove();
        UpdateCamera();
        GetPhysicMove();

        Vector3 frameMovements = _playerMove + _physicMove;
        frameMovements -= frameMovements * damping;
        // applique tout les movements
        _controls.Move(frameMovements);
    }

    private void GetPlayerMove()
    {
        float moveSpeed = _controls.isGrounded ? Speed : Speed * AirControl;
        //transforme l'input dans le repère du joueur
        Vector2 playerSpaceInput = _moveInput.Rotate(_cam_root.transform.rotation.y);

        _playerMove = new Vector3
        (
            playerSpaceInput.x * moveSpeed * Time.deltaTime,
            0,
            playerSpaceInput.y * moveSpeed * Time.deltaTime
        );
    }

    private void GetPhysicMove()
    {
        float gravityMove = _controls.isGrounded ? 0.0f : -GravityAcceleration * Time.deltaTime;
        float jumpMove = _iJumping > 0.0f ? JumpSpeed * Time.deltaTime : 0.0f;

        _physicMove += new Vector3(0, _iJumping > 0.0f ? jumpMove : gravityMove, 0);
        _iJumping = Mathf.Clamp(_iJumping - Time.deltaTime, 0.0f, JumpDuration);
    }

    private void UpdateCamera()
    {
        baseRotation = new Quaternion
        (
            _cam_root.transform.rotation.x,
            _cam_root.transform.rotation.y,
            _cam_root.transform.rotation.z
        );

        //baseRotation 
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        //lire des input binaire (pressé / relâché) en évitant le rebond

        if(value.ReadValueAsButton() && _controls.isGrounded)
        {
            Debug.Log("Jump");
        }
        else if(!value.ReadValueAsButton())
        {
            _iJumping = 0.0f;
            Debug.Log("No Jump");
        }
    }

    public void OnInteract(InputAction.CallbackContext value)
    {
        //lire des input binaire (pressé / relâché) en évitant le rebond

        if(value.ReadValueAsButton() && !_iInteracting) {
            _iInteracting = true;
        } else if(!value.ReadValueAsButton() && _iInteracting) {
            _iInteracting = false;
        }
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        //value.ReadValue<Vector2>() lire un vecteur
        _moveInput = value.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        _lookInput = value.ReadValue<Vector2>();
    }
}
