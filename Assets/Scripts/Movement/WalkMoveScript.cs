using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkMoveScript : MoveScript
{
    public float groundCheckRange = .15f;//längden på de strålar som skickas neråt för att kolla om man står på marken och hämta information som lutning därifrån

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
