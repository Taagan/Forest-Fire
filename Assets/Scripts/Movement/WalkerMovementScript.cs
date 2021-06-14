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
    public bool grounded = false;
    
    public bool affectedByGravity = true;
    public float gravityConstant = 40f;
    public float maxFallSpeed = 40f;

    protected Vector2 latestMovement = Vector2.zero;//sparar hur långt man flyttade sig efter varje update
    protected bool verticalSpeedSet = false; //hindrar gravitations påverkan en uppdatering efter att SetVerticalVelocity kallats

    override protected void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        if (affectedByGravity && !collisions.below && !verticalSpeedSet)
        {
            velocity.y -= gravityConstant * Time.deltaTime;
            if (velocity.y < -maxFallSpeed)
                velocity.y = -maxFallSpeed;
        }
        else if (affectedByGravity && collisions.below && !verticalSpeedSet)
        {
            if (collisions.standingOnSlope)
            {//Den här raden ökar gravitationsfarten om man redan står på marken och det är en nedförsbacke proportionerligt
                velocity.y = -(Mathf.Sin(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x) + 2);//+2 som felmarginal. 
            }
            else
                velocity.y = -2;
        }

        latestMovement = Move(velocity * Time.deltaTime);
        SetGrounded(collisions.below);
        verticalSpeedSet = false;
    }

    protected virtual void SetGrounded(bool value)
    {
        grounded = value;
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
    

}
