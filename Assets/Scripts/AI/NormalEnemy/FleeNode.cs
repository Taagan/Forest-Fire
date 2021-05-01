using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeNode : Node
{
    public FleeNode(Enemy enemy) : base(enemy) { }


    public override NodeState Execute()
    {
        //Give Extra stuff for flee(Currently no more info needed)
        enemy.Flee();
        return NodeState.Success;
    }
}
