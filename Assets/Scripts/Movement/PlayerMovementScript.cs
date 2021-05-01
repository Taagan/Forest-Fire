using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spelarens begränsningar läggs här. Controllern kan se nedräknarvariabler och sådant men den kan kalla alla metoder så mycket den vill också
//om inte spelarens dash cooldown är nedräknad så händer inget.
[RequireComponent(typeof(PlayerScript))]
public class PlayerMovementScript : WalkerMovementScript
{
    public PlayerScript playerScript;

    public float groundFriction = 10f;//ny fart = fart du har + (fart du vill ha - fart du har) * (ground||air)Friction * Time.deltaTime
    public float airFriction = 2f;

    public float runSpeed = 10f;
    public float jumpVelocity = 10f;
    public float jumpHoldTime = .2f;//sekunder som man kan hålla nere hoppknappen för att få högre hopphöjd.
    public int airJumps = 1;

    public float dashTime = .3f;//Sekunder som man dashar om den inte avbryts.
    public float dashVelocity = 40f;

    public float jumpFwdBoost = 5;//fart frammåt som man får om man rör vill röra sig medans man gör ett hopp

    public bool bounceActive = false;
    public float bounceMultiplier = 2f;

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
    protected Vector2 dashDirection = Vector2.zero;
    protected float dashTimer;
    protected int dashes = 1;
    
    protected float postDashFriction = 15f;
    protected float postDashTime = .4f;
    protected float postDashTimer = 0;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        playerScript = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    override protected void Update()
    {
        Timers();
        
        if (dashing)
        {
            affectedByGravity = false;
            velocity = dashDirection * dashVelocity;

            bounceActive = true;//BEHÖVER FINSLIPAS!
        }
        else if (!dashing)
        {
            affectedByGravity = true;
            bounceActive = false;

            if (grounded)
            {
                airJumpsAvailable = airJumps;
                dashes = 1;
            }

            if (moveDir != 0)
                wantedHorizontalSpeed = runSpeed * moveDir;
            else
                wantedHorizontalSpeed = 0;
            
            Accelerate();
        }

        base.Update();

        //resets & bounces
        if (collisions.right && velocity.x > 0)
        {
            if (bounceActive)
            {
                float bounceBack = (velocity.x * Time.deltaTime - latestMovement.x) * -1 * bounceMultiplier;
                transform.Translate(bounceBack, 0, 0);

                if (dashing)
                {
                    velocity.x *= -1;
                    dashDirection.x *= -1;
                    dashTimer = dashTime;
                }
                else
                    velocity.x = -1 * bounceMultiplier * velocity.x;
            }
            else
                velocity.x = 0;
        }
        else if (collisions.left && velocity.x < 0)
        {
            if (bounceActive)
            {
                float bounceBack = (velocity.x * Time.deltaTime - latestMovement.x) * -1 * bounceMultiplier;
                transform.Translate(bounceBack, 0, 0);
                
                if (dashing)
                {
                    velocity.x *= -1;
                    dashDirection.x *= -1;
                    dashTimer = dashTime;
                }
                else
                    velocity.x = -1 * bounceMultiplier * velocity.x;
            }    
            else
                velocity.x = 0;
        }

        if (collisions.above && velocity.y > 0)
            velocity.y = 0;

        moveDir = 0;
        jumping = false;
    }

    protected void Timers()
    {
        if (jumpCooldownTimer > 0)
            jumpCooldownTimer -= Time.deltaTime;

        if (jumpingTimer > 0)
        {
            jumpingTimer -= Time.deltaTime;
            SetVerticalVelocity(jumpVelocity);
        }

        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;
        if (dashing && dashTimer <= 0)
            StopDash();

        if (postDashTimer > 0)
            postDashTimer -= Time.deltaTime;

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
        float currentFriction;

        if (postDashTimer > 0)
            currentFriction = postDashFriction;
        else
            currentFriction = grounded ? groundFriction : airFriction;

        float deltaVel = (wantedHorizontalSpeed - velocity.x) * currentFriction * Time.deltaTime;
        velocity.x += deltaVel;
    }//BRa värden för denna är 10groundfriction och typ 2 airfriction

    //i denna agerar friction som en konstant acceleration, står du på marken och vill framåt så accelererar du framåt med groundFriction/s
    //kontrollerar så den inte accelererar över den gränsen. Dock så vill jag att man ska kunna ta sig över gränsen med andra abilities
    
    
    public void StartJump()
    {
        if ((grounded || airJumpsAvailable > 0) && jumpCooldownTimer <= 0 && !jumping)
        {
            if (dashing)
                StopDash();

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

    //konstant snabb fart, studsbar, timer, ska gå att avsluta när som??
    public void StartDash(Vector2 dir)
    {
        if (dashing || dashes < 1)
            return;

        dashes--;

        dir.x = (dir.x == 0) ? 0 : Mathf.Sign(dir.x);
        dir.y = (dir.y == 0) ? 0 : Mathf.Sign(dir.y);

        if (dir.y < 0)
            DuckThroughSemisolid();

        if (dir.x == 0 && dir.y == 0)
            dir.x = playerScript.facing;
        
        dashDirection = dir.normalized;

        dashTimer = dashTime;
        dashing = true;
    }

    //händer automatiskt när dashtimer är slut eller när controllern kör den.
    public void StopDash()
    {
        dashing = false;
        postDashTimer = postDashTime;
        velocity.y *= .3f;
    }

    public void Move(sbyte dir)//dir == -1 = vänster, dir == 1 = höger
    {
        moveDir = dir;
    }

}
