using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Funkar bäst när den påverkas av gravitation men allt sådant får man göra själv i controllerklasser.
public class WalkerMovementScript : MovementScript
{
    [Range(1, 89)]
    public float maxWalkableAngle = 50;//degrees from the ground up
    [HideInInspector]
    public bool grounded = false;//sann om mark upptäcks med groundcheck, bör användas för om hopp är tillåtet, mer förlåtande än collisions.below
    [HideInInspector]
    public Vector2 velocity = Vector2.zero;

    public bool gravityTypeAcceleration = false;//accelererar den med -gravityConstant/s eller är dess velocity neråt bara konstant -gravityConstant
    public bool affectedByGravity = true;
    public float gravityConstant = 9.82f;
    public float maxFallSpeed = 40f;
    
    protected float groundCheckRange = .02f;
    protected float groundAngle = 0.0f;//i vinklar, unitys egna använder vinklar för det mesta, andra mattebibliotek använder radianer..
    protected bool verticalSpeedSet = false; //hindrar gravitations påverkan en uppdatering efter att SetVerticalVelocity kallats

    override protected void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        if (affectedByGravity && (!collisions.below && !collisions.ascendingSlope) && !verticalSpeedSet)
        {
            if (gravityTypeAcceleration)
            {
                velocity.y -= gravityConstant * Time.deltaTime;
                if (velocity.y < -maxFallSpeed)
                    velocity.y = -maxFallSpeed;
            }
            else
                velocity.y = -gravityConstant;
        }
        else if (affectedByGravity && (collisions.below||collisions.ascendingSlope) && !verticalSpeedSet)
            velocity.y = 0;

        Move(velocity * Time.deltaTime);
        verticalSpeedSet = false;
    }

    public void SetHorizontalVelocity(float speed)//negativt = vänster
    {
        velocity.x = speed;
    }
    
    public void SetVerticalVelocity(float speed)//negativt = neråt
    {
        velocity.y = speed;
        verticalSpeedSet = true;
    }

    protected override void Move(Vector2 moveBy)
    {
        UpdateRayOrigins();
        GroundCheck();
        collisions.reset();

        if(grounded && groundAngle != 0 && moveBy.y <= 0 && moveBy.x != 0)
        {
            if (Mathf.Sign(moveBy.x) != Mathf.Sign(groundAngle))
                AscendSlope(ref moveBy, Mathf.Abs(groundAngle));
            else if (Mathf.Sign(moveBy.x) == Mathf.Sign(groundAngle))
                DescendSlope(ref moveBy, Mathf.Abs(groundAngle));
        }
        
        HorizontalMove(ref moveBy);
        VerticalMove(ref moveBy);
        

        transform.Translate(moveBy, Space.World);
    }

    protected override void HorizontalMove(ref Vector2 moveBy)
    {
        sbyte dir = (sbyte)Mathf.Sign(moveBy.x);
        Vector2 rStart = dir == 1 ? rayOrigins.bottomRight : rayOrigins.bottomLeft;
        float rayLength = Mathf.Abs(moveBy.x) + skinDepth;

        for (int i = 0; i < horizontalRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rStart, Vector2.right * dir, rayLength, collisionMask);
            Debug.DrawRay(rStart, Vector2.right * dir * rayLength, Color.red);
            if (hit)
            {
                if (!hit.transform.CompareTag("Semisolid"))
                {
                    float hitAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if(i == 0 && hitAngle <= maxWalkableAngle && !collisions.ascendingSlope)// && moveBy.y <= 0)
                    {
                        float yBefore = moveBy.y;
                        float hitDist = hit.distance - skinDepth;
                        moveBy.x -= hitDist * dir;
                        AscendSlope(ref moveBy, hitAngle);
                        moveBy.x += hitDist * dir;
                        if (yBefore > moveBy.y)
                            moveBy.y = yBefore;
                    }
                    else if (hitAngle > maxWalkableAngle)
                    {
                        moveBy.x = (hit.distance - skinDepth) * dir;
                        rayLength = hit.distance;

                        //kolla om går uppför, isåfall justera moveBy.y så den inte liksom hoppar vid väggen.
                        if (collisions.ascendingSlope)
                        {
                            moveBy.y = Mathf.Abs(Mathf.Tan(Mathf.Abs(groundAngle) * Mathf.Deg2Rad) * moveBy.x);
                        }


                        if (dir >= 1)
                            collisions.right = true;
                        else
                            collisions.left = true;
                    }
                }
            }
            rStart += rayOrigins.horizontalSpacing;
        }
    }

    protected override void VerticalMove(ref Vector2 moveBy)
    {
        sbyte dir = (sbyte)Mathf.Sign(moveBy.y);
        Vector2 rStart = dir == 1 ? rayOrigins.topLeft : rayOrigins.bottomLeft;
        float rayLength = Mathf.Abs(moveBy.y) + skinDepth;

        for (int i = 0; i < verticalRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rStart, Vector2.up * dir, rayLength, collisionMask);
            Debug.DrawRay(rStart, Vector2.up * dir * rayLength, Color.red);
            if (hit)
            {
                if (!(hit.transform.CompareTag("Semisolid") && (dir > 0 || ignoreSemisolid)))//om den är semisolid, ignorera den i vissa fall
                {
                    float hitAngle = Vector2.SignedAngle(hit.normal, Vector2.up);
                    
                    //om första/sista stråle beroende på rörelseriktning och rör sig neråt
                    if((i == 0 && Mathf.Sign(moveBy.x) == 1 || (i == verticalRays - 1 && Mathf.Sign(moveBy.x) == -1)) && dir == -1 && collisions.descendingSlope && hitAngle == groundAngle)
                    {
                        //gör inget...... lite onödigt kanske men men

                    }
                    else
                    {
                        moveBy.y = (hit.distance - skinDepth) * dir;
                        rayLength = hit.distance;

                        if (collisions.ascendingSlope)
                        {
                            moveBy.x = moveBy.y/Mathf.Tan(Mathf.Abs(groundAngle) * Mathf.Deg2Rad) * Mathf.Sign(moveBy.x);
                        }
                    }
                    if (dir <= 0)
                    {
                        collisions.below = true;
                    }
                    else
                        collisions.above = true;
                }
            }
            rStart += rayOrigins.verticalSpacing;
        }
    }
    
    protected void AscendSlope(ref Vector2 moveBy, float angle)
    {
        collisions.ascendingSlope = true;
        angle = Mathf.Abs(angle) * Mathf.Deg2Rad;
        
        sbyte xDir = (sbyte) Mathf.Sign(moveBy.x);

        //ersätt mvoeBy.y med nytt, om man redan har positiv y-fart borde de filtreras före det kommer hit.
        moveBy.y = Mathf.Sin(angle) * Mathf.Abs(moveBy.x);
        moveBy.x = Mathf.Cos(angle) * moveBy.x;
        
    }

    protected void DescendSlope(ref Vector2 moveBy, float angle)//tar moveBy.x och lutar det avståndet längs kurvan som skas nerför.
    {
        //Debug.Log("Descending from slope");

        collisions.descendingSlope = true;
        angle = Mathf.Abs(angle) * Mathf.Deg2Rad;
        sbyte xDir = (sbyte)Mathf.Sign(moveBy.x);

        float moveDistance = Mathf.Abs(moveBy.x);

        moveBy.y = Mathf.Sin(angle) * moveDistance * -1;
        moveBy.x = Mathf.Cos(angle) * moveBy.x;

        //specialraycasts som ser till att man inte förlorar fart vid ett byte från att gå ner för backe till att va på platt mark.
        Vector2 rayOrigin = xDir == 1 ? rayOrigins.bottomRight : rayOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * xDir, Mathf.Abs(moveBy.x) + skinDepth, collisionMask);//kolla om träffar något i fothöjd innan man träffar marken
        Debug.DrawRay(rayOrigin, Vector2.right * xDir * ((Mathf.Abs(moveBy.x) + skinDepth)), Color.white);

        //ignorerar inte semisolider, möjlighet till pytteliten bugg som gör att man rör sig liite för kort i en frame om semisolid finns nära mark till backe och man rör sig skitsnabbt horisontellt, nog inget att oroa sig för iofs
        //inte helt säker dock...
        if (!hit)//om inte träffas, gör den andra raycasten som kollar om vi landar på mark me vår bakända
        {
            rayOrigin = xDir == 1 ? rayOrigins.bottomLeft : rayOrigins.bottomRight;
            rayOrigin.x += moveBy.x;

            hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Abs(moveBy.y) + skinDepth, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * xDir * (Mathf.Abs(moveBy.y) + skinDepth), Color.white);

            if (hit)//det är här specialfallet faktiskt händer, lägger till lite i moveBy.x bara.. mycket kod för litet specialfall.....
            {
                float hitDistance = hit.distance - skinDepth;
                moveBy.y = -hitDistance;

                float slopeDistance = hitDistance / Mathf.Sin(angle);
                moveDistance -= slopeDistance;
                moveBy.x = Mathf.Cos(angle) * slopeDistance * xDir;
                moveBy.x += moveDistance * xDir;
            }
        }

        //lägg till kollar i verticalMove:
        //ska göra så den glider ner för en backe den inte kan stå på
    }
    

    //FUNKAR
    protected void GroundCheck()
    {
        grounded = false;
        groundAngle = 0;

        //gör båda för att motverka ett scenario där man står på platt mark men den upptäcker en nerförsbacke som ens ena kant hänger över
        RaycastHit2D leftHit = Physics2D.Raycast(rayOrigins.absBottomLeft + Vector2.up * 2 * skinDepth + Vector2.right * .01f, Vector2.down, groundCheckRange + 2 * skinDepth, collisionMask);
        RaycastHit2D rightHit = Physics2D.Raycast(rayOrigins.absBottomRight + Vector2.up * 2 * skinDepth - Vector2.right * .01f, Vector2.down, groundCheckRange + 2 * skinDepth, collisionMask);

        Debug.DrawRay(rayOrigins.absBottomRight + Vector2.up * 2 * skinDepth - Vector2.right * .01f, Vector2.down * (groundCheckRange + 2 * skinDepth), Color.blue);
        Debug.DrawRay(rayOrigins.absBottomLeft + Vector2.up * 2 * skinDepth + Vector2.right * .01f, Vector2.down * (groundCheckRange + 2 * skinDepth), Color.blue);

        //om båda fast med olika vinklar, mest komplicerade kollen, rätt så specifikt fall iofs men
        if((leftHit && rightHit) && (leftHit.normal != rightHit.normal))
        {
            //En positiv vinkel lutar så att en boll skulle rullat åt höger, en negativ vinkel åt andra hållet!
            float leftAngle = Vector2.SignedAngle(leftHit.normal, Vector2.up);
            float rightAngle = Vector2.SignedAngle(rightHit.normal, Vector2.up);

            bool leftAngleViable = Mathf.Abs(leftAngle) <= maxWalkableAngle;
            bool rightAngleViable = Mathf.Abs(rightAngle) <= maxWalkableAngle;

            if(leftAngleViable && !rightAngleViable)
            {
                grounded = true;
                groundAngle = leftAngle;
            }
            else if(rightAngleViable && !leftAngleViable)
            {
                grounded = true;
                groundAngle = rightAngle;
            }
            else if(rightAngleViable && leftAngleViable)
            {
                grounded = true;
                //om vinkeln är noll säger jag att den inte är viable, för då står man på en backe tänker jag.
                if (rightAngle == 0 && leftAngle > 0)//   \___
                    groundAngle = leftAngle;
                else if (leftAngle == 0 && rightAngle < 0)//  ____/
                    groundAngle = rightAngle;
                else if (rightAngle == 0 && leftAngle < 0)// /---
                    groundAngle = rightAngle;
                else if (leftAngle == 0 && rightAngle > 0)//  ---\
                    groundAngle = leftAngle;
                else
                    groundAngle = leftAngle;            //annars om de båda lutar så väljer vi vänstervinkeln arbiträrt.
            }
        }
        else if (leftHit)
        {
            if(Vector2.Angle(Vector2.up, leftHit.normal) <= maxWalkableAngle)
            {
                grounded = true;
                groundAngle = Vector2.SignedAngle(leftHit.normal, Vector2.up);
            }
        }
        else if (rightHit)
        {
            if (Vector2.Angle(Vector2.up, rightHit.normal) <= maxWalkableAngle)
            {
                grounded = true;
                groundAngle = Vector2.SignedAngle(rightHit.normal, Vector2.up);
            }
        }
    }
}
