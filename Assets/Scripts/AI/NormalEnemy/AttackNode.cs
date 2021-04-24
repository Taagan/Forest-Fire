using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNode : Node
{
    public AttackNode(Enemy enemy) : base(enemy) { }




    public override NodeState Execute()
    {
        //Give Extra stuff for attack(Currently no more info needed)
        enemy.Attack();
        return NodeState.Success;
    }
}
