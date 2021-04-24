using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleNode : Node
{
    public IdleNode(Enemy enemy) : base(enemy) { }


    public override NodeState Execute()
    {
        //Give Extra stuff for idle(Currently no more info needed)
        enemy.Idle();
        return NodeState.Success;
    }
}
