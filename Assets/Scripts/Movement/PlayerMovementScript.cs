using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spelarens begränsningar läggs här. Controllern kan se nedräknarvariabler och sådant men den kan kalla alla metoder så mycket den vill också
//om inte spelarens dash cooldown är nedräknad så händer inget.
[RequireComponent(typeof(PlayerScript))]
public class PlayerMovementScript : WalkerMovementScript
{
    [HideInInspector] public PlayerScript playerScript;
    SpriteRenderer sRenderer;
    
    public float runSpeed = 10f;
    public float jumpVelocity = 10f;
    public float jumpHoldTime = .2f;//sekunder som man kan hålla nere hoppknappen för att få högre hopphöjd.
    public int airJumps = 1;

    //lägre värde ger långsammare acceleration. såklart. Tänk lite i hur många frames i 60 fps accelerationen kommer ta från 0 till runspeed och vice versa.
    public float groundAcceleration = 100f;//200 som standard ger ca 6 frames för att komma upp i runSpeed
    public float airAcceleration = 50f;
    public float groundDecceleration = 200f;
    public float airDecceleration = 50f;
    //om man är över runSpeed och inte vill vända helt utan fortf vill åt samma håll så saktar man ner med dessa värdena.
    public float overSpeedGroundDecceleration = 100f;
    public float overSpeedAirDecceleration = 25f;

    public float dashTime = .3f;//Sekunder som man dashar om den inte avbryts.
    public float dashVelocity = 40f;
    public float bounceBoost = 1.5f;
    public int maxBounces = 5;
    public float jumpFwdBoost = 5;//fart frammåt som man får om man rör vill röra sig medans man gör ett hopp

    public int maxDashes = 1;
    public int dashes { get; protected set; } = 1;
    
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
    
    protected float postDashFriction = 15f;
    protected float postDashTime = .4f;
    protected float postDashTimer = 0;
    protected int bounces = 0;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        playerScript = GetComponent<PlayerScript>();
        sRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    override protected void Update()
    {
        Timers();
        
        if (dashing)
        {
            velocity = dashDirection * dashVelocity;

            //lite sådär temporärt:
            sRenderer.color = Color.blue;
        }
        else if (!dashing)
        {
            sRenderer.color = Color.white;

            if (grounded)
            {
                airJumpsAvailable = airJumps;
                dashes = maxDashes;
                bounces = 0;
            }

            if (moveDir != 0)
                wantedHorizontalSpeed = runSpeed * moveDir;
            else
                wantedHorizontalSpeed = 0;
            
            Accelerate();
        }

        base.Update();

        //resets & bounces
        if (collisions.right && velocity.x > 0 || collisions.left && velocity.x < 0)
        {
            if (dashing && bounces < maxBounces)
            {
                float bounceBack = (velocity.x * Time.deltaTime - latestMovement.x) * -1;
                velocity.x *= -1;
                
                if(bounces == 0)
                {
                    velocity.x *= bounceBoost;
                    velocity.y *= bounceBoost;
                    bounceBack *= bounceBoost;
                }

                transform.Translate(bounceBack, 0, 0);
                dashDirection.x *= -1;
                dashTimer = dashTime;

                dashes = maxDashes;

                bounces++;
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

    protected void Accelerate()
    {
        //räkna ut om accelererar eller deccelererar.
        //om deccelererar mot att stanna eller för att vända så deccelererar man snabbare, annars deccelerar man lite långsammare.
        //om deccelererar kolla om deccelererar à la överfartdecceleration.
        //farten man accelererar/deccelerar med är ett konstant siffervärde
        float dT = Time.deltaTime;

        int wantedDir = wantedHorizontalSpeed != 0 ? (int)Mathf.Sign(wantedHorizontalSpeed) : 0;
        int currentDir = velocity.x != 0 ? (int)Mathf.Sign(velocity.x) : 0;
        
        if (wantedDir == 0 && currentDir == 0)
            return;

        //om siktar på noll
        if(wantedDir == 0)
        {
            //alltid positiv version av velocity.x, räkna på den så och flippa till rätt håll sen bara.
            float vel = velocity.x * currentDir;
            float deccel = grounded ? groundDecceleration : airDecceleration;
            vel -= deccel * dT;

            if (vel < 0)
                vel = 0;//kan orsaka problem kanske då detta gör att man står helt stilla en hel frame när man vänder håll, känn efter.

            velocity.x = vel * currentDir;
        }
        //om ska vända sig så deccelererar man snabbare
        else if(wantedDir == currentDir * -1)
        {
            float vel = velocity.x * currentDir;
            float deccel = grounded ? groundDecceleration + groundAcceleration : airDecceleration + airAcceleration;
            vel -= deccel * dT;

            velocity.x = vel * currentDir;
        }
        //accelerera
        else if (currentDir == 0 || (wantedHorizontalSpeed * wantedDir > velocity.x * currentDir && wantedDir == currentDir))
        {
            float vel = currentDir * velocity.x;
            float accel = grounded ? groundAcceleration : airAcceleration;
            vel += accel * dT;

            if (vel > wantedHorizontalSpeed * wantedDir)
                vel = wantedHorizontalSpeed * wantedDir;

            velocity.x = vel * wantedDir;
        }
        //överfartsdeccelerera
        else if(wantedDir == currentDir && velocity.x * currentDir > wantedHorizontalSpeed * wantedDir)
        {
            float vel = currentDir * velocity.x;
            float deccel = grounded ? overSpeedGroundDecceleration : overSpeedAirDecceleration;
            vel -= deccel * dT;

            if (vel < wantedHorizontalSpeed * wantedDir)
                vel = wantedHorizontalSpeed * wantedDir;

            velocity.x = vel * wantedDir;
        }
        

    }

    
    public void StartJump()
    {
        if ((grounded || airJumpsAvailable > 0) && jumpCooldownTimer <= 0 && !jumping)
        {
            bool dashJumped = false;

            if (dashing && dashDirection.y > 0)
                dashJumped = true;
            else
                jumpingTimer = jumpHoldTime;

            if (dashing)
            {
                StopDash(true);
                if (dashJumped)
                    velocity.y += jumpVelocity;
            }

            if (!grounded)
                airJumpsAvailable--;

            jumpCooldownTimer = jumpCooldown;

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

        velocity = dashDirection * dashVelocity;

        affectedByGravity = false;
        dashTimer = dashTime;
        dashing = true;
    }

    //händer automatiskt när dashtimer är slut eller när controllern kör den.
    public void StopDash(bool postdashFriction = true)
    {
        dashing = false;
        if (postdashFriction)
        {
            postDashTimer = postDashTime;
            velocity.y *= .3f;
        }

        affectedByGravity = true;
    }

    public void Move(sbyte dir)//dir == -1 = vänster, dir == 1 = höger
    {
        moveDir = dir;
    }

}
