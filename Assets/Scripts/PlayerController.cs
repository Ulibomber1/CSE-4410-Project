using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{ 
    public float jumpHeight = 0;
    public float haltingDrag;
    public float accelerationForce;
    public float maxVelocity;

    //private bool isGrounded = false;
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    private Vector3 movementVector;
    private Quaternion rotationResult;
    private bool jumping = false;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;

    void OnJump()
    {
        jumping = true;
    }


    void JumpCheck()
    {
        if (GroundCheck() && jumping == true)
        {
            Debug.Log("Boing");
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
            jumping = false;
        }
        else
        {
            jumping = false;
        }
    }

    void OnFire()
    {
        Debug.Log("Pew"); // Projectile-based shooting
        // Disable projectile when they hit something
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 readVector = movementValue.Get<Vector2>();
        Vector3 toConvert = new Vector3(readVector.x, 0, readVector.y);
        movementVector = VectorLocalToRelative(toConvert);
        VectorToQuaternion(movementVector);
    }

    void VectorToQuaternion(Vector3 movementVector)
    {
        Vector3 relative = (transform.position + movementVector) - transform.position;
        rotationResult = Quaternion.LookRotation(relative, Vector3.up);
    }

    void MovePlayerRelativeToCamera()
    {
        if (movementVector.magnitude == 0.0f)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationY |
                             RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationZ;
            return;
        }
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        rb.AddForce(movementVector.normalized * accelerationForce);
        VectorToQuaternion(movementVector);
        transform.rotation = rotationResult;

        if (rb.velocity.sqrMagnitude > maxVelocity * maxVelocity) // Using sqrMagnitude for efficiency
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    private Vector3 VectorLocalToRelative(Vector3 vector)
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeVerticalInput = vector.normalized.z * forward;
        Vector3 rightRelativeHorizontalInput = vector.normalized.x * right;

        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;
        return cameraRelativeMovement;
    }

    bool GroundCheck()
    {
        if (Physics.BoxCast(transform.position, boxSize, -transform.up, transform.rotation, maxDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        movementVector = VectorLocalToRelative(movementVector);
        MovePlayerRelativeToCamera();
        
        JumpCheck();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Void"))
        {
            transform.position = new Vector3(0f, 0.5f, 0f);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }
}
