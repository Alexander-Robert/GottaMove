using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{

    private Animator anim;
    private BaseMovement baseMove;
    private ImprovedMovement improveMove;
    private CelesteMovement celetseMove;
    private Collision coll;
    [HideInInspector]
    public SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<Collision>();
        baseMove = GetComponentInParent<BaseMovement>();
        improveMove = GetComponentInParent<ImprovedMovement>();
        celetseMove = GetComponentInParent<CelesteMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        anim.SetBool("onGround", coll.onGround);
        anim.SetBool("onWall", coll.onWall);
        anim.SetBool("onRightWall", coll.onRightWall);
        if(baseMove.enabled){
            anim.SetBool("wallGrab", baseMove.wallGrab);
            anim.SetBool("wallSlide", baseMove.wallSlide);
            anim.SetBool("canMove", baseMove.canMove);
            anim.SetBool("isDashing", baseMove.isDashing);
        } if(improveMove.enabled) {
            anim.SetBool("wallGrab", improveMove.wallGrab);
            anim.SetBool("wallSlide", improveMove.wallSlide);
            anim.SetBool("canMove", improveMove.canMove);
            anim.SetBool("isDashing", improveMove.isDashing);
        } else {
            anim.SetBool("wallGrab", celetseMove.wallGrab);
            anim.SetBool("wallSlide", celetseMove.wallSlide);
            anim.SetBool("canMove", celetseMove.canMove);
            anim.SetBool("isDashing", celetseMove.isDashing);
        }
    }

    public void SetHorizontalMovement(float x,float y, float yVel)
    {
        anim.SetFloat("HorizontalAxis", x);
        anim.SetFloat("VerticalAxis", y);
        anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void Flip(int side)
    {

        if (baseMove.wallGrab || baseMove.wallSlide || improveMove.wallGrab || improveMove.wallSlide)
        {
            if (side == -1 && sr.flipX)
                return;

            if (side == 1 && !sr.flipX)
            {
                return;
            }
        }

        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }
}
