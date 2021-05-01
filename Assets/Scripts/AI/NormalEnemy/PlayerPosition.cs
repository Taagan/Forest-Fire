using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : Node
{
    public PlayerPosition(Enemy enemy) : base(enemy) { }


    public override NodeState Execute()
    {
        if (enemy.transform.position.x > enemy.Player.transform.position.x)
        {
            enemy.PlayerToTheLeft = true;
        }
        else if (enemy.transform.position.x < enemy.Player.transform.position.x)
        {
            enemy.PlayerToTheLeft = false;
        }
        return NodeState.Success;
    }
}
