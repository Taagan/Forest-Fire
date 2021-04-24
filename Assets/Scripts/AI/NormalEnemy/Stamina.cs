using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : Node
{
    public Stamina(Enemy enemy) : base(enemy) { }


    public override NodeState Execute()
    {
        if (enemy.Stamina > 0 && !enemy.Resting)
        {
            Debug.Log("not resting");
            return NodeState.Success;
        }
        else if (!enemy.Resting)
        {
            Debug.Log("go rest");
            enemy.Resting = true;
            Debug.Log(enemy.Resting);
            return NodeState.Failure;
        }
        Debug.Log("resting");
        return NodeState.Failure;
    }
}
