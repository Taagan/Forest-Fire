using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spelarens begränsningar läggs här. Controllern kan se nedräknarvariabler och sådant men den kan kalla alla metoder så mycket den vill också
//om inte spelarens dash cooldown är nedräknad så händer inget.
[RequireComponent(typeof(PlayerScript))]
public class PlayerMovementScript : WalkerMovementScript
{
    public enum MovementState
    {
        none,
        running,
        wall_gliding,
        hanging, //hänger på väggkant
        jumping,
        falling,
        dashing
    }

    public MovementState movementState { get; protected set; } = MovementState.none;
    public int speedLevel { get; protected set; } = 0;

    [HideInInspector] public PlayerScript playerScript;
    SpriteRenderer sRenderer;//antagligen temporär typ, används nu för att ändra färg beroende på movementState

    [Space(10)]
    [Header("               Running/speed")]
    public float[] runSpeed = new float[] { 7, 10, 15 };
    public float groundAcceleration = 100f;
    public float airAcceleration = 50f;
    public float groundDecceleration = 200f;
    public float airDecceleration = 50f;
    
    [Space(10)]
    [Header("               Jump")]
    public float jumpVelocity = 10f;
    public float jumpHoldTime = .15f;//sekunder som man kan hålla nere hoppknappen för att få högre hopphöjd.
    public int airJumps = 1;
    public float jumpFwdBoost = 5;//fart frammåt som man får om man rör vill röra sig medans man gör ett hopp
    public float coyoteTime = .1f;//tid efter man lämnat marken som man fortfarande är grounded.

    [Space(10)]
    [Header("               Dash")]
    public float dashTime = .3f;//Sekunder som man dashar om den inte avbryts.
    public float dashVelocity = 20f;
    public int maxDashes = 1;
    public int dashes { get; protected set; } = 1;
    
    protected const float ignSemisolidsUpTime = .25f;
    protected float ignSemisolidsTimer = 0f;
    protected float duckThroughDownVel = -5f;//velocity applied to more smoothly pass through semisolids

    protected int airJumpsAvailable = 1;
    protected float jumpingTimer = 0;
    protected const float jumpCooldown = .05f;//liten timer för att förhindra omedelbara "accidental" dubbelhopp
    protected float jumpCooldownTimer = jumpCooldown;
    protected float coyoteTimer = 0;

    protected int forgivnessLevel = 0;//level of speed to get back to if forgiven
    protected int forgivnessDir = 1;//riktningen som man kan få tillbaka sin fart åt.
    protected float forgivnessTime = .10f;//tid som man kan låta bli att ge input utan att man förlorar sin speedlevel
    protected float forgivnessTimer = 0;
    protected float loseSpeedThreshold = .4f;//andel av minfarten man måste hålla sig över för att inte förlora farten.
    protected float loseSpeedTime = .2f;//förlora speedlevel om man har stått stilla så här länge.
    protected float loseSpeedTimer = 0;
    protected int maxSpeedLevel;
    protected int moveDirLast = 1;
    protected int moveDir = 1;
    protected float wantedHorizontalSpeed = 0f;

    protected int dashDir = 1;
    protected float dashTimer;


    //Colors, för debug mest kanske. Om det inte blir snyggt nog att ha i spelet
    private Color noneColor = Color.white;
    private Color runningColor = Color.white;
    private Color wall_glidingColor = Color.blue;
    private Color hangingColor = Color.grey; //hänger på väggkant
    private Color jumpingColor = Color.white;
    private Color fallingColor = Color.white;
    private Color dashingColor = Color.cyan;

    private Color speedColorOne = Color.white;
    private Color speedColorTwo = Color.magenta;
    private Color speedColorThree = Color.red;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        playerScript = GetComponent<PlayerScript>();
        sRenderer = GetComponent<SpriteRenderer>();

        maxSpeedLevel = runSpeed.Length;
    }

    // Update is called once per frame
    override protected void Update()
    {
        Timers();

        if (grounded)
        {
            airJumpsAvailable = airJumps;
            dashes = maxDashes;
        }
        
        //Movements
        //None och falling gör inget unikt just nu. De bara ändrar kanske movementState och sedan skickar vidare till running 
        //eftersom där hanteras acceleration redan på ett sätt som inte bör vara neutralt för running
        switch (movementState)
        {
            case MovementState.none:
                sRenderer.color = noneColor;

                if (!grounded)
                    movementState = MovementState.falling;
                else if (moveDir != 0)
                    movementState = MovementState.running;

                StandardMovementUpdate();
                break;

            case MovementState.falling:
                sRenderer.color = fallingColor;

                if (grounded && moveDir == 0)
                    movementState = MovementState.none;
                else if (grounded)
                    movementState = MovementState.running;

                StandardMovementUpdate();
                break;

            case MovementState.running:
                sRenderer.color = runningColor;

                if (!grounded)
                    movementState = MovementState.falling;
                else if (moveDir == 0)
                    movementState = MovementState.none;

                StandardMovementUpdate();
                break;

            case MovementState.wall_gliding:
                sRenderer.color = wall_glidingColor;

                break;

            case MovementState.hanging:
                sRenderer.color = hangingColor;

                break;

            case MovementState.jumping:
                sRenderer.color = jumpingColor;

                StandardMovementUpdate();
                break;

            case MovementState.dashing:
                sRenderer.color = dashingColor;

                //velocity.x = dashDir * dashVelocity;
                sRenderer.color = Color.blue;
                break;
        }
        
        base.Update();

        //resets
        if (collisions.right && velocity.x > 0 || collisions.left && velocity.x < 0)
        {
            velocity.x = 0;
        }
        if (collisions.above && velocity.y > 0)
            velocity.y = 0;

        moveDirLast = moveDir;
        moveDir = 0;
    }

    protected void StandardMovementUpdate()
    {
        if (speedLevel > 0 && Mathf.Abs(velocity.x) < runSpeed[0] * loseSpeedThreshold && loseSpeedTimer <= 0)//om står stilla
            loseSpeedTimer = loseSpeedTime;
        else if (Mathf.Abs(velocity.x) > runSpeed[0] * loseSpeedThreshold)
            loseSpeedTimer = 0;

        //Kolla om vänder eller stannar så ska forgivnessTimern starta och man kan då inom tidsramen kunna få tillbaka sin fart åt det hållet.
        //SpeedLevel resettas till noll om man stannar eller vänder och återges när man ger input i rätt riktning igen.
        if (speedLevel > 0 && forgivnessTimer <= 0)
        {
            if (moveDirLast == moveDir * -1 || moveDir == 0)//om vänder eller stannar starta förlåtelsetiden
            {
                forgivnessTimer = forgivnessTime;
                forgivnessLevel = speedLevel;
                forgivnessDir = moveDirLast;
                speedLevel = 0;
            }
        }
        else if (forgivnessTimer > 0 && moveDir != 0)
        {
            if (moveDir == forgivnessDir)//Forgiven!
            {
                forgivnessTimer = 0;
                speedLevel = forgivnessLevel;
            }
        }

        Accelerate();
    }

    //om den går från sant till falsk så startar den coyoteTimer istället som sen sätter den till falskt efter asså
    protected override void SetGrounded(bool value)
    {
        if (grounded == value)
            return;
        else if (!value && coyoteTimer <= 0)
        {
            coyoteTimer = coyoteTime;
        }
        else if (value)
        {
            grounded = true;
            coyoteTimer = 0;
        }

    }

    protected void Timers()
    {
        float dT = Time.deltaTime;

        if(loseSpeedTimer > 0)
        {
            loseSpeedTimer -= dT;
            if (loseSpeedTimer <= 0)
                speedLevel = 0;
        }

        if (forgivnessTimer > 0)
        {
            forgivnessTimer -= dT;
        }

        if(coyoteTimer > 0)
        {
            coyoteTimer -= dT;
            if (coyoteTimer <= 0)
                grounded = false;
        }

        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= dT;
        }

        if (jumpingTimer > 0)
        {
            jumpingTimer -= dT;
            SetVerticalVelocity(jumpVelocity);
        }

        if (dashTimer > 0)
        {
            dashTimer -= dT;
        }
        if (movementState == MovementState.dashing && dashTimer <= 0)
        {
            StopDash();
        }
        
        if (ignSemisolidsTimer > 0)
        {
            ignoreSemisolid = true;
            ignSemisolidsTimer -= dT;
        }
        else
            ignoreSemisolid = false;
    }
    
    protected void Accelerate()
    {
        if (moveDir != 0)
            wantedHorizontalSpeed = runSpeed[speedLevel] * moveDir;
        else
            wantedHorizontalSpeed = 0;

        //räkna ut om accelererar eller deccelererar.
        //om deccelererar mot att vända så deccelererar man snabbare, kanske inte bör vara så men..
        //farten man accelererar/deccelerar med är ett konstant siffervärde per sekund
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
        else if (wantedDir == currentDir && velocity.x * currentDir > wantedHorizontalSpeed * wantedDir)
        {
            float vel = currentDir * velocity.x;
            float deccel = grounded ? groundDecceleration : airDecceleration;
            vel -= deccel * dT;

            if (vel < wantedHorizontalSpeed * wantedDir)
                vel = wantedHorizontalSpeed * wantedDir;

            velocity.x = vel * wantedDir;
        }


    }
    
    public void StartJump()
    {
        if ((grounded || airJumpsAvailable > 0) && jumpCooldownTimer <= 0 && movementState != MovementState.jumping)
        {    
            jumpingTimer = jumpHoldTime;

            if (movementState == MovementState.dashing)
                StopDash();

            if (!grounded)
                airJumpsAvailable--;

            jumpCooldownTimer = jumpCooldown;
            movementState = MovementState.jumping;

            if (moveDir != 0)
                velocity.x += moveDir * jumpFwdBoost;
        }
    }

    public void StopJump()
    {
        jumpingTimer = 0;
        if(movementState == MovementState.jumping)
            movementState = MovementState.falling;
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
    public void StartDash(int dir)
    {
        if (movementState == MovementState.dashing || dashes < 1)
            return;

        if (movementState == MovementState.jumping)
        {
            StopJump();
        }

        if (dir != 0)
            dir = (int)Mathf.Sign(dir);//ett eller minus ett..
        else
            dir = playerScript.facing;
        
        if (speedLevel < maxSpeedLevel - 1)
            speedLevel++;

        velocity.x = dir * dashVelocity;
        velocity.y = 0;
        affectedByGravity = false;

        dashDir = dir;
        dashes--;
        dashTimer = dashTime;
        movementState = MovementState.dashing;
    }

    //händer automatiskt när dashtimer är slut eller när controllern kör den.
    public void StopDash()
    {
        if(movementState == MovementState.dashing)
            movementState = MovementState.none;
        affectedByGravity = true;
    }

    public void Move(int dir)//dir == -1 = vänster, dir == 1 = höger
    {
        moveDir = dir;
    }

}
