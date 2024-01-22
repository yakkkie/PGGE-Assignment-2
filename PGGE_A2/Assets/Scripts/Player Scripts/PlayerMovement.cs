using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGGE;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public CharacterController mCharacterController;
    public Animator mAnimator;

    public float mWalkSpeed = 1.5f;
    public float mRotationSpeed = 50.0f;
    public bool mFollowCameraForward = false;
    public float mTurnRate = 10.0f;

#if UNITY_ANDROID
    public FixedJoystick mJoystick;
#endif

    private float hInput;
    private float vInput;
    private float speed;
    private float lerpValue;
    private bool jump = false;
    private bool emote1 = false;
    private bool emote2 = false;
    private bool emote3 = false;
    private bool crouch = false;
    public float mGravity = -30.0f;
    public float mJumpHeight = 1.0f;

    private Vector3 mVelocity = new Vector3(0.0f, 0.0f, 0.0f);

    void Start()
    {
        mCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //HandleInputs();
        //Move();

        
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    public void HandleInputs()
    {
        // We shall handle our inputs here.
    #if UNITY_STANDALONE
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
    #endif

    #if UNITY_ANDROID
        hInput = 2.0f * mJoystick.Horizontal;
        vInput = 2.0f * mJoystick.Vertical;
    #endif

        speed = mWalkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {

            speed = mWalkSpeed * 2  ;
            lerpValue = Mathf.Lerp(lerpValue, speed, 0.05f);
        }
        else
        {
            lerpValue = Mathf.Lerp(lerpValue, mWalkSpeed, 0.05f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jump = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            crouch = !crouch;
            Crouch();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            emote1 = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            emote2 = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            emote3 = true;
        }
    }

    public void Move()
    {
        if (crouch) return;

        // We shall apply movement to the game object here.
        if (mAnimator == null) return;
        if (mFollowCameraForward)
        {
            RotatePlayerToCameraForward();
        }
        else
        {
            transform.Rotate(0.0f, hInput * mRotationSpeed * Time.deltaTime, 0.0f);
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
        forward.y = 0.0f;

        mCharacterController.Move(forward * vInput * speed * Time.deltaTime);
        mAnimator.SetFloat("PosX", 0);

        mAnimator.SetFloat("PosZ", vInput * lerpValue / (2.0f * mWalkSpeed));


        if(jump)
        {
            Jump();
            jump = false;
        }

        if (emote1)
        {
            Emote(1);
            emote1 = false;
        }
        if(emote2)
        {
            Emote(2);
            emote2 = false;
        }
        if (emote3)
        {
            Emote(3);
            emote3= false;
        }
    }

    void Jump()
    {
        mAnimator.SetTrigger("Jump");
        mVelocity.y += Mathf.Sqrt(mJumpHeight * -2f * mGravity);
    }

    void Emote(float i)
    {
        string emote = "emote";
        string emoted = emote + i;
        mAnimator.SetTrigger(emoted);

    }

    private Vector3 HalfHeight;
    private Vector3 tempHeight;
    void Crouch()
    {
        mAnimator.SetBool("Crouch", crouch);
        if(crouch)
        {
            //make the character crouch
            SetCrouchHeight();
        }
        else
        {
            //reset the character crouch
            CameraConstants.CameraPositionOffset = tempHeight;
        }
    }

    void ApplyGravity()
    {
        // apply gravity.
        mVelocity.x = 0f;
        mVelocity.z = 0f;

       CalculateGravity();
    }

    void CalculateGravity()
    {
        mVelocity.y += mGravity * Time.deltaTime;
        if (mCharacterController.isGrounded && mVelocity.y < 0)
            mVelocity.y = 0f;
    }

    void SetCrouchHeight()
    {
        //set the crouch height to be half of the original height
        tempHeight = CameraConstants.CameraPositionOffset;
        HalfHeight = tempHeight;
        HalfHeight.y *= 0.5f;
        CameraConstants.CameraPositionOffset = HalfHeight;
    }

    void RotatePlayerToCameraForward()
    {
        // rotate Player towards the camera forward.
        Vector3 eu = Camera.main.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0.0f, eu.y, 0.0f),
            mTurnRate * Time.deltaTime);
    }

}
