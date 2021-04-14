using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Funkar bäst när den påverkas av gravitation men allt sådant får man göra själv i controllerklasser.
public class WalkerMovementScript : MovementScript
{
    [HideInInspector]
    public Vector2 velocity = Vector2.zero;
    [HideInInspector]
    public bool grounded;
    
    public bool affectedByGravity = true;
    public float gravityConstant = 9.82f;
    public float maxFallSpeed = 40f;
    
    protected bool verticalSpeedSet = false; //hindrar gravitations påverkan en uppdatering efter att SetVerticalVelocity kallats
    protected float groundCheckRange = .01f;

    override protected void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        GroundCheck();

        if (affectedByGravity && !collisions.below && !verticalSpeedSet)
        {
            velocity.y -= gravityConstant * Time.deltaTime;
            if (velocity.y < -maxFallSpeed)
                velocity.y = -maxFallSpeed;
        }
        else if (affectedByGravity && collisions.below && !verticalSpeedSet)
        {
            velocity.y = 0;
        }

        Move(velocity * Time.deltaTime);
        verticalSpeedSet = false;
    }

    /// <summary>
    /// negativ speed = vänster
    /// </summary>
    /// <param name="speed"></param>
    public void SetHorizontalVelocity(float speed)//negativt = vänster
    {
        velocity.x = speed;
    }
    
    /// <summary>
    /// negativ speed = neråt
    /// </summary>
    /// <param name="speed"></param>
    public void SetVerticalVelocity(float speed)//negativt = neråt
    {
        velocity.y = speed;
        verticalSpeedSet = true;
    }

    //FUNKAR
    protected void GroundCheck()
    {
        grounded = false;
        
        RaycastHit2D leftHit = Physics2D.Raycast(rayOrigins.absBottomLeft + Vector2.up * 2 * skinDepth + Vector2.right * .01f, Vector2.down, groundCheckRange + 2 * skinDepth, collisionMask);
        RaycastHit2D rightHit = Physics2D.Raycast(rayOrigins.absBottomRight + Vector2.up * 2 * skinDepth - Vector2.right * .01f, Vector2.down, groundCheckRange + 2 * skinDepth, collisionMask);

        //Debug.DrawRay(rayOrigins.absBottomRight + Vector2.up * 2 * skinDepth - Vector2.right * .01f, Vector2.down * (groundCheckRange + 2 * skinDepth), Color.blue);
        //Debug.DrawRay(rayOrigins.absBottomLeft + Vector2.up * 2 * skinDepth + Vector2.right * .01f, Vector2.down * (groundCheckRange + 2 * skinDepth), Color.blue);

        grounded = leftHit || rightHit;
    }
}
