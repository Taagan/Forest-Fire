using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klass med Move-metod som då flyttar en entity och kollar kollisioner med sin collisionmask på vägen.
/// </summary>


[RequireComponent(typeof(BoxCollider2D))]
public class MoveScript : MonoBehaviour
{
    public LayerMask collisionMask;

    [Range(3,20)]
    public int horizontalRays = 5;
    [Range(3, 20)]
    public int verticalRays = 5;
    float skinDepth = .15f;//hur djupt inne i hitboxen som raycast-raysen börjar.

    [HideInInspector]
    public bool ignoreSemisolid = false;//låter en falla genom semisolider, används när man ska duck-hoppa genom en semisolid plattform, per platformingstandard

    protected RayStruct rayStruct;
    protected CollisionInfo collisions;
    new protected BoxCollider2D collider;
    
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        //Debugkod som låter en direktkontrollera saken.
        Vector2 velocity = new Vector2();
        
        velocity.y = -20;
        velocity.x = Input.GetAxisRaw("Horizontal") * 10;

        velocity.y += Input.GetAxisRaw("Vertical") * 30;

        if (Input.GetKey(KeyCode.S))
            ignoreSemisolid = true;
        else
            ignoreSemisolid = false;

        Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Moves entity with raycast collisionChecks and updates collisions
    /// </summary>
    /// <param name="moveBy"></param>
    public virtual void Move(Vector2 moveBy)
    {
        collisions.reset();

        HorizontalMove(moveBy.x);
        VerticalMove(moveBy.y);
    }
    
    protected void HorizontalMove(float moveBy)
    {
        UpdateRayStruct();//någorlunda ooptimiserat men är så liten operation att det inte lär göra mycket
        
        sbyte dir = (sbyte)Mathf.Sign(moveBy);
        Vector2 rStart = dir == 1 ? rayStruct.bottomRight : rayStruct.bottomLeft;
        float rayLength = Mathf.Abs(moveBy) + skinDepth;

        for (int i = 0; i < horizontalRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rStart, Vector2.right * dir, rayLength, collisionMask);
            Debug.DrawRay(rStart, Vector2.right * dir * rayLength, Color.red);
            if (hit)
            {
                if (!hit.transform.CompareTag("Semisolid"))
                {
                    moveBy = (hit.distance - skinDepth) * dir;
                    rayLength = hit.distance;
                    if (dir >= 1)
                        collisions.right = true;
                    else
                        collisions.left = true;
                }
            }
            rStart += rayStruct.horizontalSpacing;
        }

        transform.Translate(moveBy, 0, 0, Space.World);
    }

    protected void VerticalMove(float moveBy)
    {
        UpdateRayStruct();

        sbyte dir = (sbyte)Mathf.Sign(moveBy);
        Vector2 rStart = dir == 1 ? rayStruct.topLeft : rayStruct.bottomLeft;
        float rayLength = Mathf.Abs(moveBy) + skinDepth;

        for (int i = 0; i < verticalRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rStart, Vector2.up * dir, rayLength, collisionMask);
            Debug.DrawRay(rStart, Vector2.up * dir * rayLength, Color.red);
            if (hit)
            {
                if(!(hit.transform.CompareTag("Semisolid") && (dir > 0 || ignoreSemisolid)))//om den är semisolid, ignorera den i vissa fall
                {
                    moveBy = (hit.distance - skinDepth) * dir;
                    rayLength = hit.distance;
                    if (dir <= 0)
                        collisions.below = true;
                    else
                        collisions.above = true;
                }

            }

            rStart += rayStruct.verticalSpacing;
        }

        transform.Translate(0, moveBy, 0, Space.World);
    }

    private void UpdateRayStruct()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinDepth * -2);

        rayStruct.bottomLeft = bounds.min;
        rayStruct.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rayStruct.topRight = bounds.max;
        rayStruct.topLeft = new Vector2(bounds.min.x, bounds.max.y);

        rayStruct.horizontalSpacing = new Vector2(0, bounds.size.y / (horizontalRays - 1));
        rayStruct.verticalSpacing = new Vector2(bounds.size.x / (verticalRays - 1), 0);
    }

    protected struct RayStruct
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
        public Vector2 horizontalSpacing, verticalSpacing;
    }

    protected struct CollisionInfo
    {
        public bool above, below, left, right;

        public void reset()
        {
            above = below = left = right = false;
        }
    }
}
