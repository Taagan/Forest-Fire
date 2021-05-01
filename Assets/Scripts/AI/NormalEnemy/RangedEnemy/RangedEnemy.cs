using UnityEngine;
using UnityEngine.UI;

public class RangedEnemy : Enemy
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float maxStamina;
    [SerializeField] private float maxCourage;
    [SerializeField] private float shotDelay;
    [SerializeField] private float restingAmountStamina = 50;
    [SerializeField] private float restingRateStamina = 0.2f;
    [SerializeField] private float attackRateStamina = 0.1f;
    [SerializeField] private float fleeRateStamina = 0.2f;
    [SerializeField] private float idleRateStamina = 0.4f;
    [SerializeField] private bool canShoot;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite scaredSprite;
    [SerializeField] private Sprite tiredSprite;


    private float timer = 0;
    private SpriteRenderer spriteR;
    enum Skin { Normal, Attack, Scared, Tired};
    Skin currentSkin;

    Node baseNode;
    public RangedEnemy()
    {
        baseNode = new SelectorNode(this);
        SequenceNode attackMode = new SequenceNode(this);
        SequenceNode fleeMode = new SequenceNode(this);
        SequenceNode idleMode = new SequenceNode(this);

        InverterNode inv = new InverterNode(this);

        PlayerNear playerNear = new PlayerNear(this);
        Afraid afraid = new Afraid(this);
        Stamina stamina = new Stamina(this);
        PlayerPosition playerPosition = new PlayerPosition(this);


        AttackNode attackNode = new AttackNode(this);
        FleeNode fleeNode = new FleeNode(this);
        IdleNode idleNode = new IdleNode(this);


        baseNode.AddChild(attackMode);
        baseNode.AddChild(fleeMode);
        baseNode.AddChild(idleMode);

        attackMode.AddChild(playerNear);
        attackMode.AddChild(inv);
        attackMode.AddChild(stamina);
        attackMode.AddChild(playerPosition);
        attackMode.AddChild(attackNode);

        inv.AddChild(afraid);

        fleeMode.AddChild(playerNear);
        fleeMode.AddChild(stamina);
        fleeMode.AddChild(playerPosition);
        fleeMode.AddChild(fleeNode);

        idleMode.AddChild(idleNode);
    }

    private void Start()
    {
        spriteR = GetComponent<SpriteRenderer>();
        currentSkin = Skin.Normal;
    }

    public void FixedUpdate()
    {
        staminaSlider.value = stamina / maxStamina;
        CourageCheck();
        ShotCooldown();
        baseNode.Execute();
        Debug.Log(courage);
    }

    public override void Attack()
    {
        if (currentSkin != Skin.Attack)
        {
            currentSkin = Skin.Attack;
            UpdateSkin();
        }
        if (playerToTheLeft)
            this.transform.rotation = new Quaternion(this.transform.rotation.x, 180, this.transform.rotation.z, 0);

        else if (!playerToTheLeft)
            this.transform.rotation = new Quaternion(this.transform.rotation.x, 0, this.transform.rotation.z, 0);

        else { }
            //Player is on the same x-axis as you


        stamina -= attackRateStamina;
        if (canShoot)
        {
            Instantiate(projectile, this.transform.position, this.transform.rotation);
            canShoot = false;
            Debug.Log("shooting");
        }
    }

    public override void Flee()
    {
        if (currentSkin != Skin.Scared)
        {
            currentSkin = Skin.Scared;
            UpdateSkin();
        }
        if (playerToTheLeft)
        {
            movement.SetHorizontalVelocity(speed);
            this.transform.rotation = new Quaternion(this.transform.rotation.x, 0, this.transform.rotation.z, 0);
        }
        else if (!playerToTheLeft)
        {
            movement.SetHorizontalVelocity(-speed);
            this.transform.rotation = new Quaternion(this.transform.rotation.x, 180, this.transform.rotation.z, 0);
        }
        else
        {
            //Player is on the same x-axis as you
        }
        stamina -= fleeRateStamina;
    }

    public override void Idle()
    {
        if (currentSkin != Skin.Normal)
        {
            currentSkin = Skin.Normal;
            UpdateSkin();
        }
        movement.velocity = Vector2.zero;
        if (resting)
        {
            if (currentSkin != Skin.Tired)
            {
                currentSkin = Skin.Tired;
                UpdateSkin();
            }
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


    private void CourageCheck()
    {
        if (playerClose)
        {
            courage -= 0.5f;
            if (courage < -maxCourage)
                courage = -maxCourage;
        }
        else
        {
            courage += 0.5f;
            if (courage > maxCourage)
                courage = maxCourage;
        }
    }

    private void ShotCooldown()
    {
        Debug.Log(timer);
        if (!canShoot)
        {
            Debug.Log("reloading");
            timer += Time.deltaTime;
            if (timer >= shotDelay)
            {
                timer = 0;
                canShoot = true;
            }
        }
    }

    private void UpdateSkin()
    {
        switch (currentSkin)
        {
            case Skin.Normal:
                spriteR.sprite = normalSprite;
                break;
            case Skin.Attack:
                spriteR.sprite = attackSprite;
                break;
            case Skin.Scared:
                spriteR.sprite = scaredSprite;
                break;
            case Skin.Tired:
                spriteR.sprite = tiredSprite;
                break;
            default:
                break;
        }
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            //collision.gameObject.GetComponent<PlayerScript>().hurt
        }
    }
}

