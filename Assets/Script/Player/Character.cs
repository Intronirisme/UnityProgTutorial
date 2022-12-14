using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(GravityReceiver))]
public class Character : MonoBehaviour
{
    [Header("Walking")]
    public float Speed = 5f;
    public float GroundDamping = 2f;
    public float TurnRate = 3f;

    [Header("Jumping")]
    public float JumpSpeed = 3f;
    public float JumpDuration = .4f;
    public float MaxFallingSpeed = 4f;
    public float GravityAcceleration = 9.81f;

    [Header("Air control")]
    public float AirControl = .3f;
    public float AirDamping = .5f;

    [Header("Ground detection")]
    public Vector2 GroundDetectionOffset = new Vector2(0.0f, 0.1f);


    //references vers les autres composants
    private CharacterController _controls;
    private GameObject _body;
    private Material _body_mat;
    private CapsuleCollider _caps;
    private GameObject _cam_root;
    private GameObject _cam;

    //inputs & déplacements
    private Vector2 _moveInput; //récupère l'input du joueur
    private Vector3 _playerMove; //input converti en déplacement
    private Vector3 _physicMove; //déplacements causés par la physique

    //private Vector3 _acceleration;
    private Vector3 _velocity;

    //booleen ayant pour but d'éviter l'input bouncing
    private bool _iInteracting = false;
    private bool _iJumping = false;
    //private float _remainingJump = 0.0f;

    private bool _iGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
        //assignations des références vers nos autres composants
        _controls = gameObject.GetComponent<CharacterController>();
        _body = gameObject.FindInChildren("Body");
        _body_mat = _body.GetComponent<Renderer>().material;
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
        TestGround();
        _body_mat.SetColor("_Color", _iGrounded ? Color.white : Color.red);
        float damping = _iGrounded ? GroundDamping : AirDamping;

        GetPlayerMove();
        GetPhysicMove();

        Vector3 frameMovements = _playerMove + _physicMove;
        //frameMovements -= frameMovements * damping;
        // applique tout les movements
        _controls.Move(frameMovements);
    }

    private void FixedUpdate() {
        TestGround();
    }

    private void GetPlayerMove()
    {
        float moveSpeed = _iGrounded ? Speed : Speed * AirControl;
        //transforme l'input dans le repère du joueur
        Vector2 playerSpaceInput = _moveInput.Rotate(-_cam_root.transform.rotation.eulerAngles.y);

        _playerMove = new Vector3
        (
            playerSpaceInput.x * moveSpeed * Time.deltaTime,
            0,
            playerSpaceInput.y * moveSpeed * Time.deltaTime
        );

            //aligne le corps dans la direction du movement
        Vector3 XZvelocity = new Vector3(_controls.velocity.x, 0, _controls.velocity.z).normalized;
        if(XZvelocity.magnitude > 0.01f)
        {
            //RotateToward prend en paramètre un interval de temps, cette fonction étant appelée à chaque frames,
            //il est de fait possible t'interpoler l'alignement du joueur au cours du temps (effet smooth)
            _body.transform.rotation = Quaternion.RotateTowards(
                _body.transform.rotation,
                Quaternion.LookRotation(XZvelocity),
                TurnRate * Time.deltaTime
            );
        }
    }

    private void GetPhysicMove()
    {
        if(!_controls.isGrounded)
        {
            // Ma vélocité augmente
            _velocity.y -= GravityAcceleration * Time.deltaTime;
            _velocity.y = Mathf.Clamp(_velocity.y, -MaxFallingSpeed, MaxFallingSpeed);
        }
        else
        {
            // Si je touche le sol je ne vais pas vers le bas
            _velocity.y = Mathf.Clamp(_velocity.y, 0.0f, MaxFallingSpeed);
        }
        _physicMove = _velocity * Time.deltaTime;
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        //lire des input binaire (pressé / relâché) en évitant le rebond

        if(value.ReadValueAsButton() && !_iJumping && _iGrounded)
        {
            _iJumping = true;
            _velocity.y += JumpSpeed;
            
            //Debug.Log("Jump");
        }
        else if(!value.ReadValueAsButton() && _iJumping)
        {
            _iJumping = false;
            //Debug.Log("No Jump");
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

    private void TestGround()
    {
        RaycastHit hit;
        float halfHeight = (_controls.height / 2) + GroundDetectionOffset.y;
        float radius = _controls.radius + GroundDetectionOffset.x;
        if(Physics.SphereCast(transform.position, radius, Vector3.down, out hit, halfHeight))
        {
            _iGrounded = hit.normal.AngleBetweenVector(Vector3.up) <= _controls.slopeLimit;
        } else {
            _iGrounded = false;
        }
    }
}
