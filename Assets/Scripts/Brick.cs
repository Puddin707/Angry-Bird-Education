using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private Sprite[] spriteList;
    [SerializeField] public float health;
    [SerializeField] private int destroyScore;
    [SerializeField] private AudioClip[] soundListDestroy;
    [SerializeField] private AudioClip[] soundListCollide;
    [SerializeField] private AudioClip[] soundListDamage;

    [SerializeField] public GameObject dieText;

    [SerializeField] private GameObject woodParticle;

    [SerializeField] private GameObject explosionParticlePrefab;

    [SerializeField] private Color32 color;
    [SerializeField] private Color32 outlineColor;



    [SerializeField] private int materialType;

    [SerializeField] private bool isTnt = false;
    // 0 wood 
    // 1 glass
    // 2 stone

    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]

    private Vector3 velocityBeforePhysicsUpdate;

    Rigidbody2D rb;

    private void FixedUpdate()
    {
        velocityBeforePhysicsUpdate = rb.velocity;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            die();
        }
    }
    public void Start()
    {
        currentHealth = health;

    }
    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();

    }

    public void die()
    {
        if(isTnt)
        {
            tnt(gameObject.transform.position,3f, 500f);
            GameObject bombParticleObject = Instantiate(explosionParticlePrefab, transform.position, transform.rotation);
            AudioPlayer.audio.PlayOneShot(soundListDestroy[0]);
            Destroy(bombParticleObject, 2f);
            Destroy(gameObject);
        }
        else
        {
            AudioPlayer.audio.PlayOneShot(soundListDestroy[0], 0.5f);
            GameObject particleObject = Instantiate(woodParticle, transform.position, transform.rotation);
            particleObject.GetComponent<ParticleSystem>().Play();
            Destroy(particleObject, 5f);
            Destroy(gameObject);
        }

    }
    void tnt(Vector2 center, float radius, float force)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        foreach (var hitCollider in hitColliders)
        {

            if(hitCollider != gameObject)
            {
                float distance = Vector2.Distance(hitCollider.gameObject.transform.position, gameObject.transform.position);

                if (hitCollider.gameObject.tag == "Pig")
                {
                    hitCollider.gameObject.GetComponent<Pig>().currentHealth = hitCollider.gameObject.GetComponent<Pig>().currentHealth - (200 / (distance * 10));
                    Rigidbody2DExtension.AddExplosionForce(hitCollider.GetComponent<Rigidbody2D>(), force, gameObject.transform.position, radius + 5f);
                }
                if (hitCollider.gameObject.tag == "Brick")
                {
                    hitCollider.gameObject.GetComponent<Brick>().currentHealth = hitCollider.gameObject.GetComponent<Brick>().currentHealth - (200 + (distance * 10));
                    Rigidbody2DExtension.AddExplosionForce(hitCollider.GetComponent<Rigidbody2D>(), force, gameObject.transform.position, radius + 5f);
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (GameTime.gameTime == 0) return;

        if (col.gameObject.GetComponent<Rigidbody2D>() == null && col.gameObject.tag != "Ground")
        {
            return;
        }

        float damage = 0;

        Rigidbody2D colRB = col.gameObject.GetComponent<Rigidbody2D>();













        // birdcheck 
        if (col.gameObject.tag == "Bird")
        {
            // check if chuck and wood 

            if (col.gameObject.GetComponent<Bird>().birdType == 1 && materialType == 0)
            {
                damage = colRB.velocity.magnitude * 5 * 2;
            }

            // check if rock 
            else if (materialType == 2)
            {
                damage = colRB.velocity.magnitude * 5;
                damage = damage / 6;
            }
            // check chuck and glass
            else if (col.gameObject.GetComponent<Bird>().birdType == 1 && materialType == 1)
            {
                damage = colRB.velocity.magnitude * 5 * 2;
                damage = damage / 3;
            }

            // check blues and glass
            else if(col.gameObject.GetComponent<Bird>().birdType == 2 && materialType == 1)
            {
                damage = colRB.velocity.magnitude * 5 * 2;
            }
            // /2 damage for blues on glass
            else if (col.gameObject.GetComponent<Bird>().birdType == 2 && materialType == 0)
            {
                damage = colRB.velocity.magnitude * 5 / 2;
            }
            else
            {
                damage = colRB.velocity.magnitude * 5;
            }

            // slow chuck on glass
            if(materialType == 0 || materialType == 2 && col.gameObject.GetComponent<Bird>().birdType == 1)
            {
                colRB.velocity = new Vector2(colRB.velocity.x / 2, colRB.velocity.y / 2);
            }
        }
        else if(col.gameObject.tag != "Ground")
        {
            damage = colRB.velocity.magnitude * 5;
        }

        if (col.gameObject.tag == "Pig")
        {
            if(isTnt)
            {
                damage = velocityBeforePhysicsUpdate.magnitude;
            }
            else
            {
                damage = velocityBeforePhysicsUpdate.magnitude * 5;
            }
        }
        else if (col.gameObject.tag == "Brick")
        {
            if (isTnt)
            {
                damage = velocityBeforePhysicsUpdate.magnitude;
            }
            else
            {
                damage = velocityBeforePhysicsUpdate.magnitude * 5;
            }
        }
        else if (col.gameObject.tag == "Ground")
        {
            if (isTnt)
            {
                damage = velocityBeforePhysicsUpdate.magnitude;
            }
            else
            {
                damage = velocityBeforePhysicsUpdate.magnitude * 5;
            }
        }

        //don't play audio for small damages
        //Debug.Log((damage / health) * 100f);
        if ((damage / health) * 100f >= 40f)
        {

            GetComponent<AudioSource>().PlayOneShot(soundListDamage[Random.Range(0, 3)]);
            GameObject particleObject = Instantiate(woodParticle, transform.position, transform.rotation);
            Destroy(particleObject, 5f);
        }





















        //decrease health according to magnitude of the object that hit us
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            die();
        }

        if(!isTnt)
        {
            if ((currentHealth / health) * 100 <= 20f)  // 20 40 80
            {
                spriteRenderer.sprite = spriteList[3];
            }
            else if ((currentHealth / health) * 100 <= 40f)
            {
                spriteRenderer.sprite = spriteList[2];
            }
            else if ((currentHealth / health) * 100 <= 80f)
            {
                spriteRenderer.sprite = spriteList[1];
            }
        }
    }
}
