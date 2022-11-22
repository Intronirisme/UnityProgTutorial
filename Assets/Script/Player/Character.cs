using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    [Header("Movement")]
    public float Speed = 5f;
    public float TurnRate = .5f;
    public float LookupRate = .5f;
    public float JumpSpeed = 3f;
    public float GroundDamping = .1f;

    [Header("Air control")]
    public float AirControl = .3f;
    public float AirDamping = .01f;
    public float GravitySpeed = 10f;

    public bool isGrounded = false;
    //references vers les autres composants
    private CharacterController _controls;
    private GameObject _cam_root;
    private GameObject _body;
    private GameObject _cam;

    private Vector2 _moveInput; //récupère l'input du joueur
    private Vector3 _playerMove; //input converti en déplacement
    private Vector3 _physicMove; //déplacements causés par la physique

    // Start is called before the first frame update
    void Start()
    {
        //assignations des références vers nos autres composants
        _controls = gameObject.GetComponent<CharacterController>();
        _body = gameObject.FindInChildren("Body");
        _cam_root = gameObject.FindInChildren("CameraRoot");
        _cam = _cam_root.FindInChildren("Camera");

        //verrouillage et masquage du curseur de la souris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsGrounded();
        Vector3 frameMovements = Vector3.zero;

        float damping = isGrounded ? GroundDamping : AirDamping;
        float speed = isGrounded ? Speed : Speed * AirControl;

        //transforme l'input dans le repère du joueur
        Vector2 playerSpaceInput = _moveInput.Rotate(_cam_root.transform.rotation.y);
        
        _playerMove = new Vector3(
            playerSpaceInput.x * speed * Time.deltaTime,
            0,
            playerSpaceInput.y * speed * Time.deltaTime
        );


        // applique tout les movements
        frameMovements = _playerMove + _physicMove;
        _controls.Move(frameMovements);
    }

    public bool IsGrounded()
    {
        RaycastHit groundHit;
        Vector3 rayStart = gameObject.transform.position;

        //récupère le rayon de Body (à l'échelle)
        float radiusUnscaled = _body.GetComponent<CapsuleCollider>().radius;
        float bodyScaleFactor = _body.transform.localScale.x;
        float scaledRadius = radiusUnscaled * bodyScaleFactor;

        //récupère la hauteur du nombril aux pieds de Body (à l'échelle)
        float halfHeightUnscale = _body.GetComponent<CapsuleCollider>().height / 2;
        float bodyScaleFactorY = _body.transform.localScale.y;
        float scaledHalfHeight = halfHeightUnscale * bodyScaleFactorY;

        //marge de détection
        float threshold = 0.05f;

        return Physics.SphereCast(rayStart, scaledRadius, new Vector3(0, -1, 0), out groundHit, scaledHalfHeight+threshold);
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        //lire des input binaire (pressé / relâché)
        bool _iJumping = value.ReadValueAsButton();
        Debug.Log("On Jump ! => " + _iJumping);

        if(value.ReadValueAsButton())
        {
            //si barre espace pressée
        }
    }

        public void OnMove(InputAction.CallbackContext value)
    {
        //value.ReadValue<Vector2>() lire un vecteur
        _moveInput = value.ReadValue<Vector2>();
        Debug.Log("On Move ! => " + _moveInput);
    }
}
