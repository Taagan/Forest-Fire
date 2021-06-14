using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPlayermovementNewScript : WalkerMovementScript
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
    
    public MovementState movementState { get; protected set;}
    


    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {



        base.Update();



    }
}
