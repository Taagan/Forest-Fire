using UnityEngine;
using UnityEngine.UI;

public class NormalEnemy : Enemy
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxStamina;
    [SerializeField] private float restingAmountStamina = 50;
    [SerializeField] private float restingRateStamina = 0.2f;
    [SerializeField] private float attackRateStamina = 0.1f;
    [SerializeField] private float fleeRateStamina = 0.2f;
    [SerializeField] private float idleRateStamina = 0.4f;
    [SerializeField] private Slider staminaSlider;


    Node baseNode;
    public NormalEnemy()
    {
        baseNode = new SelectorNode(this);
        SequenceNode attackMode = new SequenceNode(this);
        SequenceNode fleeMode = new SequenceNode(this);
        SequenceNode idleMode = new SequenceNode(this);

        InverterNode inv = new InverterNode(this);

        PlayerNear playerNear = new PlayerNear(this);
        Afraid afraid = new Afraid(this);
        Stamina stamina = new Stamina(this);

        AttackNode attackNode = new AttackNode(this);
        FleeNode fleeNode = new FleeNode(this);
        IdleNode idleNode = new IdleNode(this);


        baseNode.AddChild(attackMode);
        baseNode.AddChild(fleeMode);
        baseNode.AddChild(idleMode);

        attackMode.AddChild(playerNear);
        attackMode.AddChild(inv);
        attackMode.AddChild(stamina);
        attackMode.AddChild(attackNode);

        inv.AddChild(afraid);

        fleeMode.AddChild(playerNear);
        fleeMode.AddChild(stamina);
        fleeMode.AddChild(fleeNode);

        idleMode.AddChild(idleNode);
    }


    public void FixedUpdate()
    {
        baseNode.Execute();
        staminaSlider.value = stamina / maxStamina;
        //Debug.Log(stamina);
    }

    public override void Attack()
    {
        rb.velocity = Vector3.zero;
        float moveSpeed = speed * Time.deltaTime;

        //Do attack behaviour
        Vector3 targetDir = player.transform.position - transform.position;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, moveSpeed, 0.0f);

        Debug.DrawRay(transform.position, newDir, Color.red);

        transform.rotation = Quaternion.LookRotation(newDir);

        rb.velocity = transform.forward * moveSpeed;

        stamina -= attackRateStamina;
        Debug.Log("Attacking");
    }

    public override void Flee()
    {
        rb.velocity = Vector3.zero;
        float moveSpeed = speed * Time.deltaTime;

        //Do flee behaviour
        Vector3 targetDir = player.transform.position - transform.position;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, -targetDir, moveSpeed, 0.0f);

        Debug.DrawRay(transform.position, newDir, Color.red);

        transform.rotation = Quaternion.LookRotation(newDir);

        rb.velocity = transform.forward * moveSpeed;

        stamina -= fleeRateStamina;


        Debug.Log("Fleeing");
    }

    public override void Idle()
    {
        rb.velocity = Vector3.zero;
        if (resting)
        {
            stamina += restingRateStamina;
            if (stamina >= restingAmountStamina || stamina >= maxStamina)
            {
                resting = false;
            }
        }
        else
        {
            stamina += idleRateStamina;
        }
        if (stamina >= maxStamina)
        {
            stamina = maxStamina;
        }
        else if (stamina <= 0)
        {
            stamina = 0;
        }
        //Do idle behaviour
        Debug.Log("Idle");

    }


    [SerializeField]
    public void UpdateStats(GameObject player, int hp, int speed, int atkDamage, int aggroRange, int stamina, float courage)
    {
        this.player = player;
        this.hp = hp;
        this.speed = speed;
        this.atkDamage = atkDamage;
        this.aggroRange = aggroRange;
        this.stamina = stamina;
        this.courage = courage;
    }
}
