using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNear : Node
{
    public PlayerNear(Enemy enemy) : base(enemy) { }


    public override NodeState Execute()
    {
        float dist = Vector2.Distance(enemy.Player.transform.position, enemy.transform.position);
        if (dist < enemy.AggroRange)
        {
            if (dist < enemy.AggroRange / 2)
                enemy.PlayerClose = true;

            else
                enemy.PlayerClose = false;

            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
