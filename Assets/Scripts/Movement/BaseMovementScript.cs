using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BaseMovementScript : MonoBehaviour
{
    //protected
    new protected BoxCollider2D collider;
    protected float skinDepth = .05f;

    //public
    public LayerMask collisionMask;
    [Range(3,10)]
    public int horizontalRays = 5;
    [Range(3, 10)]
    public int verticalRays = 5;
    [HideInInspector]
    public bool ignoreSemisolids = false;

    protected virtual void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Move(ref Vector2 moveBy)
    {

    }

    protected virtual void HorizontalCollisions(ref Vector2 moveBy)
    {

    }

    protected virtual void VerticalCollisions(ref Vector2 moveBy)
    {

    }
    
}
