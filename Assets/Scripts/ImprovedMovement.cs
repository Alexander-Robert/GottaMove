using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ImprovedMovement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
    public double jumpBuffer = 0;
    public double jumpBufferMax = 0.3;
    public double climbStamina = 5;
    public double climbStaminaMax = 5;
    float xVelocity = 0.0f;
    public float acceleration = 4.6f;
    public float deceleration = 5.2f;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]

    private bool groundTouch;
    private bool hasDashed;
    private bool canJumpFromGround;

    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    private Color red = new Color(255,0,0,255);
    private Color blue = new Color(0,255,255,255);
    private Color white = new Color(255,255,255,255);

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        // Velocity changes based on acceleration and deceleration times
        // Accelerate in the positive x direction
        if (xRaw > 0 && xVelocity < 1) {
            if (xVelocity < 0) {
                xVelocity = 0;
            }
            xVelocity += Time.deltaTime * acceleration;
        // Accelerate in the negative x direction
        } else if(xRaw < 0 && xVelocity > -1) {
            if (xVelocity > 0) {
                xVelocity = 0;
            }
            xVelocity += -1 * (Time.deltaTime * acceleration);
        // Deceleration while going in the positive x direction
        } else if (xRaw == 0 && xVelocity > 0) {
            xVelocity -= Time.deltaTime * deceleration;
            if (xVelocity < 0) {
                xVelocity = 0;
            }
        // Deceleration while going in the negative x direction
        } else if (xRaw == 0 && xVelocity < 0) {
            xVelocity += Time.deltaTime * deceleration;
            if (xVelocity > 0) {
                xVelocity = 0;
            }
        }
        Vector2 dir = new Vector2(xVelocity, y);

        Walk(dir);
        anim.SetHorizontalMovement(xVelocity, y, rb.velocity.y);

        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if(side != coll.wallSide)
                anim.Flip(side*-1);
            
            climbStamina -= 1 * Time.deltaTime;
            wallGrab = true;
            wallSlide = false;
        }

        if(climbStamina <= 0) {
            wallGrab = false;
        }
        if(climbStamina <= 1.5) {
            setPlayerColor(red);
        } 

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {

            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }
        
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(coll.onWall && !coll.onGround && rb.velocity.y <= 0)
        {
            if (x != 0 && !wallGrab)
            {
                //TODO: replace next two lines with wall climb logic and only call these lines once the climbStamina == 0
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround) {
            wallSlide = false;
            climbStamina = climbStaminaMax;
        }
        
        // Coyote Time Delay
        if(coll.onGround){
            canJumpFromGround = true;
        } else {
            StartCoroutine(CoyoteTime());
        }

        if (Input.GetButtonDown("Jump")) {
            jumpBuffer = jumpBufferMax;
        }

        if (jumpBuffer > 0)
        {
            jumpBuffer -= 1 * Time.deltaTime;

            anim.SetTrigger("jump");

            if (canJumpFromGround) {
                jumpBuffer = 0;
                Jump(Vector2.up, false);
            }
            if (coll.onWall && !coll.onGround) {
                jumpBuffer = 0;
                WallJump();
            }
        }

        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);

        if (wallGrab || wallSlide || !canMove)
            return;

        if(x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }

    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();

        setPlayerColor(white);
    }

    private void Dash(float x, float y)
    {
        SoundManagerScript.PlaySound ("dashpulse");
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        anim.SetTrigger("dash");

        setPlayerColor(blue);

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        //adjusted the amount of time player cannot move after a dash
        yield return new WaitForSeconds(.1f);

        dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
        SoundManagerScript.PlaySound ("jump");
    }

    private void WallSlide()
    {
        if(coll.wallSide != side)
         anim.Flip(side * -1);

        if (!canMove)
            return;

        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab) {
            return;
        }

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        SoundManagerScript.PlaySound ("jump");
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    void setPlayerColor(Color c) {
        //set the sprite's color to the given color
        var sprite = anim.GetComponent<SpriteRenderer>();
        sprite.color = c;
    }

    //Coyote Time Delay Function
    IEnumerator CoyoteTime(){
        yield return new WaitForSeconds(4f);
        canJumpFromGround = false;
    }
}
