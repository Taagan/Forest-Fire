using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : Node
{

    public SelectorNode(Enemy enemy) : base(enemy) { }


    public override NodeState Execute()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].Execute() == NodeState.Success)
                return NodeState.Success;
        }

        return NodeState.Failure;
    }
}
