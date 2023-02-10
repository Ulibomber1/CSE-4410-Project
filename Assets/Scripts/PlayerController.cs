using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    
    public float speed = 0;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI deathCountText;
    public float jumpHeight = 0;

    private bool isGrounded = false;
    private Rigidbody rb;
    private int count;
    private int deathcount;
    private float movementX;
    private float movementY;
    public GameObject winTextObject;



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        count = 0;
        deathcount = 0;

        SetCountText();
        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x; //left right
        movementY = movementVector.y; //up down
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        deathCountText.text = "Deaths: " + deathcount.ToString();

        if (count >= 6)
        {
            winTextObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

/*        if(movementX != 0 || movementY != 0 && movementY > 0) //this code sucks but its better than nothing
        {
            transform.rotation = Quaternion.LookRotation(forward); //controls rotation, feels a bit jank
        }
        else if (movementX != 0 || movementY != 0 && movementY < 0)
        {
            transform.rotation = Quaternion.LookRotation(forward * -1); //makes the player face backwards
        }
        */

        Vector3 forwardRelativeVerticalInput = movementY * forward;
        Vector3 rightRelativeHorizontalInput = movementX * right;

        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;
        //this.transform.Translate(cameraRelativeMovement, Space.World);
        rb.AddForce(cameraRelativeMovement * speed);

        transform.rotation = Quaternion.LookRotation(cameraRelativeMovement); //a little bit better way of rotating player

        /* Vector3 movement = new Vector3(movementX, 0.0f, movementY);
         rb.AddForce(movement * speed);*/

        if (rb.velocity.y == 0) //dumb way of doing isGrounded, will fix
        {
            isGrounded = true;
        }
        else
            isGrounded = false;

    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    void OnFire()
    {
        Debug.Log("Pew");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;

            SetCountText();
        }

        if (other.gameObject.CompareTag("Void"))
        {
            transform.position = new Vector3(0f, 0.5f, 0f);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            deathcount = deathcount + 1;
            SetCountText();
        }
    }
}
