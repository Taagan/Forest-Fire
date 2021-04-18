using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spelarens begränsningar läggs här. Controllern kan se nedräknarvariabler och sådant men den kan kalla alla metoder så mycket den vill också
//om inte spelarens dash cooldown är nedräknad så händer inget.
public class PlayerMovementScript : WalkerMovementScript
{
    public float groundFriction = 10f;//ny fart = fart du har + (fart du vill ha - fart du har) * (ground||air)Friction * Time.deltaTime
    public float airFriction = 2f;

    public float runSpeed = 10f;
    public float jumpVelocity = 10f;
    public float jumpHoldTime = .2f;//sekunder som man kan hålla nere hoppknappen för att få högre hopphöjd.
    public int airJumps = 1;

    public float jumpFwdBoost = 5;//fart frammåt som man får om man rör vill röra sig medans man gör ett hopp

    protected const float ignSemisolidsUpTime = .25f;
    protected float ignSemisolidsTimer = 0f;
    protected float duckThroughDownVel = -5f;//velocity applied to more smoothly pass through semisolids

    protected float jumpingTimer = 0;
    protected int airJumpsAvailable = 1;
    protected const float jumpCooldown = .05f;//liten timer för att förhindra omedelbara dubbelhopp
    protected float jumpCooldownTimer = jumpCooldown;
    
    protected sbyte moveDir = 1;
    protected float wantedHorizontalSpeed = 0f;
    protected bool jumping = false;
    protected bool dashing = false;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        Timers();

        if (jumpingTimer > 0)
            SetVerticalVelocity(jumpVelocity);

        if (grounded)
            airJumpsAvailable = airJumps;
        
        if (moveDir != 0)
            wantedHorizontalSpeed = runSpeed * moveDir;
        else
            wantedHorizontalSpeed = 0;
        
        Accelerate();

        base.Update();
        moveDir = 0;
        jumping = false;
    }

    protected void Timers()
    {
        if (jumpCooldownTimer > 0)
            jumpCooldownTimer -= Time.deltaTime;

        if (jumpingTimer > 0)
            jumpingTimer -= Time.deltaTime;

        if (ignSemisolidsTimer > 0)
        {
            ignoreSemisolid = true;
            ignSemisolidsTimer -= Time.deltaTime;
        }
        else
            ignoreSemisolid = false;
    }

    //ny fart = fart du har + (fart du vill ha - fart du har) * (ground||air)Friction * Time.deltaTime
    //kanske bra, kanske inte. Skulle kunna köra med en konstant acceleration annars.
    protected void Accelerate()
    {
        if (wantedHorizontalSpeed == velocity.x)
            return;

        float currentFriction = grounded ? groundFriction : airFriction;
        float deltaVel = (wantedHorizontalSpeed - velocity.x) * currentFriction * Time.deltaTime;
        velocity.x += deltaVel;
    }//BRa värden för denna är 10groundfriction och typ 2 airfriction

    //i denna agerar friction som en konstant acceleration, står du på marken och vill framåt så accelererar du framåt med groundFriction/s
    //kontrollerar så den inte accelererar över den gränsen. Dock så vill jag att man ska kunna ta sig över gränsen med andra abilities
    
    
    public void StartJump()
    {
        if ((grounded || airJumpsAvailable > 0) && jumpCooldownTimer <= 0 && !jumping)
        {
            if (!grounded)
                airJumpsAvailable--;
            jumpCooldownTimer = jumpCooldown;
            jumpingTimer = jumpHoldTime;

            if (moveDir != 0)
                velocity.x += moveDir * jumpFwdBoost;
        }
    }

    public void StopJump()
    {
        jumpingTimer = 0;
    }

    public void DuckThroughSemisolid()
    {
        if (!grounded)
            return;

        ignSemisolidsTimer = ignSemisolidsUpTime;
        if (velocity.y > duckThroughDownVel)
            velocity.y = duckThroughDownVel;
    }

    public void Dash()
    {

    }

    public void Move(sbyte dir)//dir == -1 = vänster, dir == 1 = höger
    {
        moveDir = dir;
    }

}
