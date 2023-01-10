using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityReceiver : MonoBehaviour
{
    [Header("General")]
    public float BaseGravityAcceleration = 9.81f;
    public Vector3 FrameVelocity { get; private set; }
    public Vector3 GravityDir { get; private set; }
    
    private float _gravityScale = 1.0f;
    private Rigidbody _RB;

    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out Rigidbody _RB))
        {
            _RB.useGravity = false;
        }
    }

    private void FixedUpdate()
    {
        FrameVelocity = BaseGravityAcceleration * _gravityScale * Time.fixedDeltaTime * GravityDir; // float * float * float * Vector3 gives Vector3
        if(_RB != null)
        {
            _RB.velocity += FrameVelocity;
        }
    }

    public void SetGravity(RaycastHit hit)
    {
        if(hit.collider.gameObject.tag == "GravityField")
        {
            GravityDir = hit.normal * -1;
        }
    }
}
