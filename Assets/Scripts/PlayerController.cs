using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Move
    public float maxSpeed = 10f;
    float currentSpeed;
 
    //Jump
    public float maxJumpVelocity = 10f;
    float startJumpVelocity;
    float currentJumpVelocity;
    public float rotationSpeed = 0.1f;
    public GameObject jumpPoint;
    public GameObject arrow;
    public float fallGravityMultiplier = 2.5f;
    public float jumpForceIncrease = 0.1f;
    public float enlargement = 3f;

    public float minY = 0;
    public float maxY = 1.5f;

    bool isJumping = false;
    bool isAirbone = false;
    float rotationAngle;
    
    Vector2 initialPos;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        startJumpVelocity = maxJumpVelocity / 2;
        currentSpeed = maxSpeed;
        currentJumpVelocity = startJumpVelocity;
        initialPos = jumpPoint.transform.localPosition;
        arrow.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
      
        MoveHorizontally();
    }

    private void FixedUpdate()
    {
        
        QuickerFall();
        Jump();
    }

    void MoveHorizontally()
    {
        if (!isJumping)
        {
            float xValue = Input.GetAxis("Horizontal");
            if (true)
                rb.velocity = new Vector2(xValue * currentSpeed, rb.velocity.y);
        }
    }

    void Jump()
    {
        JumpApply();
        JumpControl();      
        QuickerFall();
    }

    void JumpApply()
    {
        if (Input.GetButtonUp("Jump") && isJumping && !isAirbone)  
        {
            arrow.SetActive(false);
            Vector3 jumpDir = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * Vector3.right;
            rb.velocity = jumpDir * new Vector2(currentSpeed, currentJumpVelocity);
            isAirbone = true;
            
        }
    }
    void QuickerFall()
    {
        //Increase gravity to have faster fall
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;
    }
    void JumpControl()
    {
        if (isJumping && !isAirbone)
        {

            float xValue = Input.GetAxis("Horizontal");
            rotationAngle = ArcTangent(jumpPoint.transform.position, transform.position);
            if (rotationAngle <= 70 && xValue < 0)
            {
                jumpPoint.transform.position = RotateByRadians(transform.position, jumpPoint.transform.position, -rotationSpeed * Time.fixedDeltaTime);
                Debug.Log(rotationAngle);
                arrow.transform.rotation = Quaternion.Euler(new Vector3 (0,0, rotationAngle -90));
            }
            if (rotationAngle >= 30 && xValue > 0)
            {
                jumpPoint.transform.position = RotateByRadians(transform.position, jumpPoint.transform.position, rotationSpeed * Time.fixedDeltaTime);
                arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle - 90));
            }
        }

        if (Input.GetButton("Jump") && isJumping && !isAirbone && currentJumpVelocity < maxJumpVelocity)
        {
            arrow.SetActive(true);
            float proportionalConstant = minY + (maxY - minY) * ((currentJumpVelocity - startJumpVelocity) / maxJumpVelocity);
            Debug.Log(proportionalConstant);
            if (currentJumpVelocity + jumpForceIncrease != maxJumpVelocity)
            {
                arrow.transform.localScale = new Vector3 (proportionalConstant, proportionalConstant, 1);

                currentJumpVelocity += jumpForceIncrease;
            }
            else
            {
                currentJumpVelocity = maxJumpVelocity;
            }
            
        }
        if(rb.velocity.y == 0 && isAirbone)
        {
            isAirbone = false;
            isJumping = false;
            currentJumpVelocity = startJumpVelocity;
            jumpPoint.transform.localPosition = initialPos;
            rb.velocity = Vector2.zero;

        }
        if (Input.GetButton("Jump") && !isJumping && !isAirbone)
        {
            Debug.Log("jump");
            isJumping = true;
            rb.velocity = Vector2.zero;
        }


        
    }

    float ArcTangent(Vector2 A, Vector2 B)
    {
        Vector2 difference = A - B;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        return rotZ;
    }

    public Vector2 RotateByRadians(Vector2 Center, Vector2 A, float angle)
    {
        //Move calculation to 0,0
        Vector2 v = A - Center;

        //rotate x and y
        float x = v.x * Mathf.Cos(angle) + v.y * Mathf.Sin(angle);
        float y = v.y * Mathf.Cos(angle) - v.x * Mathf.Sin(angle);

        //move back to center
        Vector2 B = new Vector2(x, y) + Center;

        return B;
    }
}
