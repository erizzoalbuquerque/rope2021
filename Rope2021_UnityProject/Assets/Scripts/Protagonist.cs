using UnityEngine;
using System.Collections;

public class Protagonist : MonoBehaviour {

	public enum PhysicsState {ON_GROUND,ON_AIR,USING_ROPE, ON_WAll};

	public PhysicsState physicsState = PhysicsState.ON_GROUND; 

	public Rope rope;

	public float maxSpeedX, maxSpeedY;
	public float groundAccel, airAccel;
	public float jumpSpeed;

    public bool canDoubleJump = true;
    bool doubleJumped = false;

	AudioSource audioSource;
	public AudioClip jumpClip, doubleJumpClip, hitSolidClip;

	public float oldSpeed = 0f;

    public bool cLeft;
	public bool cRight;
	public bool onGround;
	bool onGroundTemp;

	public float timeForUnground;
	//float ungroundedTimer;


    public bool canStick;
    public bool sticking;
    public int clingAlarm = 0;
    public int nClingFrames = 8;
	
	public LayerMask whatIsSolid;
	public float solidCheckRadius = 0.01f;
	
	public Transform groundCheck,leftCheck,rightCheck;
	Collider2D groundCheckCol,leftCheckCol,rightCheckCol;
	
	bool kLeft, kRight, kUp, kDown, kJumpDown, kJumpRelease;
	
	public Rigidbody2D rigidBody2d;
	
	float axisHorizontal, axisVertical;

    public float highSpeedGroundDivider = 10;
    public float highSpeedAirDivider = 3;

    public Animator animator;


    // Use this for initialization
    void Start () {

		rope = GetComponent<Rope> ();

		canStick = true;
		sticking = false;
		
		onGround = false;
		cLeft = false;
		cRight = false;
		
		clingAlarm = 0;
		
		rigidBody2d = this.gameObject.GetComponent<Rigidbody2D>();
		
		groundCheckCol = groundCheck.GetComponent<BoxCollider2D>();
		leftCheckCol = leftCheck.GetComponent<BoxCollider2D>();
		rightCheckCol = rightCheck.GetComponent<BoxCollider2D>();

		audioSource = GetComponent<AudioSource> ();

		//ungroundedTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		axisHorizontal = Input.GetAxis ("Horizontal");
		axisVertical = Input.GetAxis ("Vertical");
		
		kLeft = (axisHorizontal < 0f) ? true : false;
		kRight = (axisHorizontal > 0f) ? true : false;
		kUp = (axisVertical > 0f) ? true : false;
		kDown = (axisVertical < 0f) ? true : false;
		
		kJumpDown = Input.GetButtonDown("Fire1");
		kJumpRelease = Input.GetButtonUp("Fire1");

		onGround = Physics2D.IsTouchingLayers( groundCheckCol,whatIsSolid);
		cLeft = Physics2D.IsTouchingLayers( leftCheckCol,whatIsSolid);
		cRight = Physics2D.IsTouchingLayers( rightCheckCol,whatIsSolid);

        //-----------CHECKING FOR STICKING CONDITIONS
        if ((cRight || cLeft))
        {
            sticking = true;
            //canStick = false;
        }

        // Reset wall cling
        if ((!cRight && !cLeft) || onGround)
        {
            //canStick = true;
            sticking = false;
        }

        // Cling to wall
        if (((kRight && cLeft) || (kLeft && cRight)) && sticking)
        {
            clingAlarm -= 1;
            if (clingAlarm <= 0)
                sticking = false;
        }
        else
        {
            clingAlarm = nClingFrames;
        }

        //-----------CHECKING FOR STICKING CONDITIONS------END

        ChoosePhysicsState();
		
		switch (physicsState) 
		{
		case PhysicsState.ON_GROUND:
			ApplyPhysicsOnGround();
			break;
			
		case PhysicsState.ON_AIR:
			ApplyPhysicsOnAir();
			break;
		
        case PhysicsState.ON_WAll:
			ApplyPhysicsOnWall();
			break;	

		case PhysicsState.USING_ROPE:
			ApplyPhysicsOnRope();
			break;			
		}

        HandleAnimation();
    }


	void ChoosePhysicsState()
	{		
		if (onGround == true && rope.ropeIsActive == false) {
			physicsState = PhysicsState.ON_GROUND;
		}
        else if (onGround == false && sticking == false && rope.ropeIsActive == false)
        {
			physicsState = PhysicsState.ON_AIR;
		}
        else if (onGround == false && sticking == true && rope.ropeIsActive == false)
        {
            physicsState = PhysicsState.ON_WAll;
        }
        else {
			physicsState = PhysicsState.USING_ROPE;
		}
	}


	void ApplyPhysicsOnGround()
	{
        doubleJumped = false;

		float currentSpeedY;

		currentSpeedY = rigidBody2d.velocity.y;
		
		float targetSpeedX;
		float currentSpeedX;
		currentSpeedX = rigidBody2d.velocity.x;
		targetSpeedX = maxSpeedX * axisHorizontal;

        if (Mathf.Abs(currentSpeedX) < maxSpeedX)
            currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, groundAccel);
        else if (currentSpeedX >= maxSpeedX && targetSpeedX < 0)
			currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, groundAccel / highSpeedGroundDivider );
        else if (currentSpeedX <= -maxSpeedX && targetSpeedX > 0)
			currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, groundAccel / highSpeedGroundDivider );
        else
            currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, 0.03f * groundAccel);
		
		// Jump 
		if (kJumpDown) 
		{ 
			currentSpeedY += jumpSpeed;
			audioSource.PlayOneShot (jumpClip);				
		}
		
		rigidBody2d.velocity = new Vector2 (currentSpeedX, currentSpeedY);
	}

	void ApplyPhysicsOnAir()
	{
		float currentSpeedY;
		
		currentSpeedY = rigidBody2d.velocity.y;

		if (currentSpeedY>maxSpeedY)
			currentSpeedY = Common.ApproachWeighted( maxSpeedY,currentSpeedY, airAccel);
		else if (currentSpeedY<-maxSpeedY)
			currentSpeedY = Common.ApproachWeighted( -maxSpeedY,currentSpeedY, airAccel);
		
		float targetSpeedX;
		float currentSpeedX;
		currentSpeedX = rigidBody2d.velocity.x;
		targetSpeedX = maxSpeedX * axisHorizontal;

        if (Mathf.Abs(currentSpeedX) < maxSpeedX)
            currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, airAccel);
        else if (currentSpeedX >= maxSpeedX && targetSpeedX < 0)
            currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, airAccel / highSpeedAirDivider);//TODO
        else if (currentSpeedX <= -maxSpeedX && targetSpeedX > 0)
            currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, airAccel / highSpeedAirDivider);//TODO
        //if (Mathf.Abs(axisHorizontal) > 0.01)
        //currentSpeedX = Common.ApproachWeighted( targetSpeedX,currentSpeedX, accel);

        if (kJumpDown && !doubleJumped && canDoubleJump) 
		{
            float doubleJumpSpeed = 0.7f * jumpSpeed;

            if (currentSpeedY > 0.0f)
                currentSpeedY = doubleJumpSpeed;
            else
                currentSpeedY = doubleJumpSpeed;

            doubleJumped = true;
			audioSource.PlayOneShot (doubleJumpClip);
        }
		else if (kJumpRelease) {
			if (currentSpeedY > 0) {
				currentSpeedY *= 0.25f;
			}

		}

        rigidBody2d.velocity = new Vector2 (currentSpeedX, currentSpeedY);
	}

	void ApplyPhysicsOnRope()
	{
        doubleJumped = false;

        //When using rope and touching ground
        if(onGround)
        {
            float currentSpeedY;

            currentSpeedY = rigidBody2d.velocity.y;

            float targetSpeedX;
            float currentSpeedX;
            currentSpeedX = rigidBody2d.velocity.x;
            targetSpeedX = maxSpeedX * axisHorizontal;

            if (Mathf.Abs(currentSpeedX) < maxSpeedX)
                currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, airAccel);
            else if (currentSpeedX >= maxSpeedX && targetSpeedX < 0)
                currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, airAccel);
            else if (currentSpeedX <= -maxSpeedX && targetSpeedX > 0)
                currentSpeedX = Common.ApproachWeighted(targetSpeedX, currentSpeedX, airAccel); ;

            // Jump 
            if (kJumpDown)
            {
                currentSpeedY += jumpSpeed;
				audioSource.PlayOneShot (jumpClip);
            }

            rigidBody2d.velocity = new Vector2(currentSpeedX, currentSpeedY);
        }
        else
        {

        }
    }


    void ApplyPhysicsOnWall()
    {
       

        float currentSpeedX = rigidBody2d.velocity.x;
        float currentSpeedY = rigidBody2d.velocity.y;

        if (currentSpeedY < 0)
        {
            //Apply damping
            currentSpeedY -= currentSpeedY * 0.05f;
        }

        // Wall jump
        if (kJumpDown && cLeft && !onGround)
        {
            if (kLeft)
            {
                currentSpeedY = jumpSpeed * 0.9f; //0.85
                currentSpeedX = jumpSpeed * 0.7f; //0.8
            }
            else {
                currentSpeedY = jumpSpeed * 0.75f;
                currentSpeedX = maxSpeedX;
            }
			audioSource.PlayOneShot (jumpClip);
        }
        if (kJumpDown && cRight && !onGround)
        {
            if (kRight)
            {
                currentSpeedY = jumpSpeed * 0.9f;//0.85
				currentSpeedX = - jumpSpeed * 0.7f; //0.8
            }
            else {
                currentSpeedY = jumpSpeed * 0.75f;
                currentSpeedX = -maxSpeedX;
            }
			audioSource.PlayOneShot (jumpClip);
        }

        rigidBody2d.velocity = new Vector2(currentSpeedX, currentSpeedY);
    }


    void HandleAnimation()
    {
		if (physicsState == PhysicsState.ON_WAll) 
		{
			animator.SetBool ("onWall", true);

			if (cRight)
			{
				animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;				
			} 
			else if (cLeft)
			{
				animator.gameObject.GetComponent<SpriteRenderer>().flipX = true;
			}
		} 
		else 
		{
			animator.SetBool ("onWall", false);

			animator.SetFloat("player_vel_x", Mathf.Abs(rigidBody2d.velocity.x));
			animator.SetFloat("player_vel_y", rigidBody2d.velocity.y);

			animator.SetBool("isGrounded", onGround);

			if (axisHorizontal > 0f)
			{
				animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
			}
			else if (axisHorizontal < 0f)
			{
				animator.gameObject.GetComponent<SpriteRenderer>().flipX = true;
			}   
		}
    }


}

