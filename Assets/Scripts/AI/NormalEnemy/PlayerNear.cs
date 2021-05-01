using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNear : Node
{
    public PlayerNear(Enemy enemy) : base(enemy) { }


    public override NodeState Execute()
    {
        float dist = Vector3.Distance(enemy.Player.transform.position, enemy.transform.position);
        if (dist < enemy.AggroRange)
        {
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
