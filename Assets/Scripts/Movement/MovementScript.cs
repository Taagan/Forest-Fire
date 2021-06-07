using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementScript : MonoBehaviour
{
    protected LayerMask collisionMask;
    
    protected int horizontalRays = 5;
    protected int verticalRays = 5;
    protected float bottomRayHeight = .2f;//höjd upp ifrån botten + skinwidth som den första horisontalla rayen castas från. För att underlätta backklättring

    protected float skinDepth = .15f;//hur djupt inne i hitboxen som raycast-raysen börjar.

    [HideInInspector]
    public bool ignoreSemisolid = false;//låter en falla genom semisolider, används när man ska duck-hoppa genom en semisolid plattform, per platformingstandard

    protected RayOrigins rayOrigins;
    public CollisionInfo collisions;
    new protected BoxCollider2D collider;
    
    protected virtual void Start()
    {
        collisionMask = LayerMask.GetMask("Ground");
        collider = GetComponent<BoxCollider2D>();
    }
    

    /// <summary>
    /// Moves entity with raycast collisionChecks and updates collisions
    /// </summary>
    /// <param name="moveBy"></param>
    protected virtual Vector2 Move(Vector2 moveBy)
    {
        collisions.reset();

        UpdateRayOrigins();
        HorizontalMove(ref moveBy);
        transform.Translate(moveBy.x, 0, 0, Space.World);

        UpdateRayOrigins();//ooptimiserat
        VerticalMove(ref moveBy);
        transform.Translate(0, moveBy.y, 0, Space.World);
        
        return moveBy;
    }
    
    protected virtual void HorizontalMove(ref Vector2 moveBy)//kollar kollision och kapar moveBy ifall kollision upptäcks. Sparar saker i collisions också
    {                           //criteria kanske e lite överflödig, men gör så jag kan ignorera en rays resultat beroende på vilket kriterie som jag vill, används i WalkerMoveScript t. ex.
        sbyte dir = (sbyte)Mathf.Sign(moveBy.x);
        Vector2 rStart = dir == 1 ? rayOrigins.bottomRight : rayOrigins.bottomLeft;
        rStart.y += bottomRayHeight;
        float rayLength = Mathf.Abs(moveBy.x) + skinDepth;

        for (int i = 0; i < horizontalRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rStart, Vector2.right * dir, rayLength, collisionMask);
            Debug.DrawRay(rStart, Vector2.right * dir * rayLength, Color.red);
            if (hit)
            {
                if (!hit.transform.CompareTag("Semisolid"))
                {
                    moveBy.x = (hit.distance - skinDepth) * dir;
                    rayLength = hit.distance;
                    if (dir >= 1)
                        collisions.right = true;
                    else
                        collisions.left = true;
                }
            }
            rStart += rayOrigins.horizontalSpacing;
        }
    }

    protected virtual void VerticalMove(ref Vector2 moveBy)
    {
        sbyte dir = (sbyte)Mathf.Sign(moveBy.y);
        Vector2 rStart = dir == 1 ? rayOrigins.topLeft : rayOrigins.bottomLeft + Vector2.up * bottomRayHeight;

        float currentSkinDepth = skinDepth;
        if (dir == -1)
            currentSkinDepth += bottomRayHeight;

        float rayLength = Mathf.Abs(moveBy.y) + currentSkinDepth;

        for (int i = 0; i < verticalRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rStart, Vector2.up * dir, rayLength, collisionMask);
            Debug.DrawRay(rStart, Vector2.up * dir * rayLength, Color.red);
            if (hit)
            {
                if(!(hit.transform.CompareTag("Semisolid") && (dir > 0 || ignoreSemisolid)))//om den är semisolid, ignorera den i vissa fall
                {
                    moveBy.y = (hit.distance - currentSkinDepth) * dir;
                    rayLength = hit.distance;
                    if (dir <= 0)
                    {
                        collisions.below = true;
                        if (Vector2.Angle(hit.normal, Vector2.up) != 0)
                            collisions.standingOnSlope = true;
                    }
                    else
                        collisions.above = true;
                }
            }
            rStart += rayOrigins.verticalSpacing;
        }
    }

    protected void UpdateRayOrigins()
    {
        Bounds bounds = collider.bounds;
        
        bounds.Expand(skinDepth * -2);

        rayOrigins.bottomLeft = bounds.min;
        rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rayOrigins.topRight = bounds.max;
        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);

        rayOrigins.horizontalSpacing = new Vector2(0, (bounds.size.y - bottomRayHeight) / (horizontalRays - 1));
        rayOrigins.verticalSpacing = new Vector2(bounds.size.x / (verticalRays - 1), 0);
    }

    protected struct RayOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
        public Vector2 horizontalSpacing, verticalSpacing;
    }

    public struct CollisionInfo
    {
        public bool above, below, left, right;

        public bool standingOnSlope;

        public void reset()
        {
            above = below = left = right = false;
            standingOnSlope = false;
        }
    }
}
