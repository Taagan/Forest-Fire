using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashArrowScript : MonoBehaviour
{

    SpriteRenderer sRenderer;
    float positionOffset;


    void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        positionOffset = transform.localPosition.y;

        SetVisible(false);
    }
    
    public void SetVisible(bool visible)
    {
        sRenderer.enabled = visible;
    }

    public void SetPosition(Vector2 pos)
    {
        pos.Normalize();
        transform.localPosition = pos * positionOffset;

        float angle = Vector2.SignedAngle(Vector2.up, pos);

        transform.rotation = Quaternion.Euler(0, 0, angle);

        //om den inte har någon riktning och alltså ligger precis på spelaren så gör den osynlig.
        if (transform.localPosition == Vector3.zero)
            SetVisible(false);
        else
            SetVisible(true);
    }
}
