using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    [SerializeField] protected WalkerMovementScript movement;
    [SerializeField] protected int hp;
    [SerializeField] protected float speed;
    [SerializeField] protected int atkDamage;
    [SerializeField] protected int aggroRange;
    [SerializeField] protected float stamina;
    [SerializeField] protected float courage;
    [SerializeField] protected bool resting;
    protected bool playerClose;
    protected bool playerToTheLeft;

    public Enemy()
    {

    }

    public virtual GameObject Player { get { return player; } }

    public virtual WalkerMovementScript Movement { get { return movement; } }

    public virtual int Hp { get { return hp; } }

    public virtual float Speed { get { return speed; } }

    public virtual int AtkDamage { get { return atkDamage; } }

    public virtual int AggroRange { get { return aggroRange; } }

    public virtual float Stamina { get { return stamina; } }

    public virtual float Courage { get { return courage; } }

    public virtual bool Resting { get { return resting; } set { resting = value; } }

    public virtual bool PlayerClose { get { return playerClose; } set { playerClose = value; } }

    public virtual bool PlayerToTheLeft { get { return playerToTheLeft; } set { playerToTheLeft = value; } }

    public virtual void Execute() { }

    public virtual void Attack() { }

    public virtual void Flee() { }

    public virtual void Idle() { }

}
