using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goombaScript : MonoBehaviour
{
    private WalkerMovementScript Movement;
    public float speed;

    private void Start()
    {
        Movement = GetComponent<WalkerMovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement.SetHorizontalVelocity(-speed);
    }
}
