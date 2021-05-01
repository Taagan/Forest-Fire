using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class Node
{
    public enum NodeState { Success, Failure, Running };
    protected NodeState currentNodeState { get { return currentNodeState; } }
    protected List<Node> children;
    protected Enemy enemy;


    public Node(Enemy enemy)
    {
        this.enemy = enemy;
        children = new List<Node>();
    }


    public virtual NodeState Execute()
    {
        return currentNodeState;
    }

    public void AddChild(Node child)
    {
        children.Add(child);
    }
}
