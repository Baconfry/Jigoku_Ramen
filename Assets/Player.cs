using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rig;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float speedMultiplier;
    private Vector3 facingDirection = -Vector3.up;
    private Sprite[] idleSprites;
    [SerializeField] private Sprite[] downSprites;
    [SerializeField] private Sprite[] rightSprites;
    [SerializeField] private Sprite[] leftSprites;
    [SerializeField] private Sprite[] upSprites;

    [SerializeField] private GameObject ramenBowl;
    [SerializeField] private Sprite deadSprite;
    private GameObject ramenBowlInstance;
    private int heldRamenSpiceLevel = 0;
    private int health;
    private Text healthText;

    private Text scoreText;
    private int score;

    private AudioSource audioSource;
    [SerializeField] private AudioClip myLeg;

    // Start is called before the first frame update
    void Start()
    {
        health = 3;
        score = 0;
        rig = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        scoreText = GameObject.Find("scoreText").GetComponent<Text>();
        healthText = GameObject.Find("healthText").GetComponent<Text>();
        //ramenBowl = transform.Find("ramenBowl").gameObject;
        idleSprites = downSprites;
        StartCoroutine(IdleAnimation());
        StartCoroutine(DecayScore());
    }

    public void UpdateScore(int amount)
    {
        score += amount;
        if (score < 0) score = 0;
        scoreText.text = "Score: " + score;
    }

    IEnumerator DecayScore()
    {
        while (health > 0)
        {
            if (score > 0)
            {
                yield return new WaitForSeconds(1);
                UpdateScore(-1);
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            rig.velocity = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f) * speedMultiplier;

            if (rig.velocity.y != 0f) facingDirection = Input.GetAxisRaw("Vertical") * Vector3.up;
            if (rig.velocity.x != 0f) facingDirection = Input.GetAxisRaw("Horizontal") * Vector3.right;

            if (facingDirection == Vector3.up)
            {
                if (idleSprites != upSprites) spriteRenderer.sprite = upSprites[0];
                idleSprites = upSprites;
            }
            else if (facingDirection == -Vector3.up)
            {
                if (idleSprites != downSprites) spriteRenderer.sprite = downSprites[0];
                idleSprites = downSprites;
            }
            else if (facingDirection == Vector3.right)
            {
                if (idleSprites != rightSprites) spriteRenderer.sprite = rightSprites[0];
                idleSprites = rightSprites;
            }
            else if (facingDirection == -Vector3.right)
            {
                if (idleSprites != leftSprites) spriteRenderer.sprite = leftSprites[0];
                idleSprites = leftSprites;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                Collider2D[] overlappedColliders = Physics2D.OverlapCircleAll(transform.position + facingDirection, 0.01f);
                for (int i = 0; i < overlappedColliders.Length; i++)
                {
                    //Debug.Log(overlappedColliders[i]);
                    if (overlappedColliders[i].GetComponent<Table>() != null && ramenBowlInstance != null && !overlappedColliders[i].GetComponent<Table>().finishedOrder)
                    {
                        overlappedColliders[i].GetComponent<Table>().GiveOrder(heldRamenSpiceLevel, ramenBowlInstance);
                        ramenBowlInstance = null;
                    }
                    else if (overlappedColliders[i].gameObject.tag == "SpiceStation")
                    {
                        if (ramenBowlInstance != null && heldRamenSpiceLevel < 2)
                        {
                            heldRamenSpiceLevel++;
                            Color spriteColor = Color.white;
                            if (heldRamenSpiceLevel == 1) spriteColor = new Color(1f, 0.7f, 0f, 1f);
                            if (heldRamenSpiceLevel == 2) spriteColor = Color.red;
                            ramenBowlInstance.GetComponent<SpriteRenderer>().color = spriteColor;
                        }
                    }
                    else if (overlappedColliders[i].gameObject.tag == "RamenStation")
                    {
                        if (ramenBowlInstance == null)
                        {
                            ramenBowlInstance = Instantiate(ramenBowl, transform.position, Quaternion.identity);
                            ramenBowlInstance.transform.parent = this.transform;
                            heldRamenSpiceLevel = 0;
                        }

                    }
                }
            }
        }
        else
        {
            rig.velocity = Vector3.zero;
            spriteRenderer.sprite = deadSprite;
        }
        
    }

    IEnumerator IdleAnimation()
    {
        while (health > 0)
        {
            for (int i = 0; i < idleSprites.Length; i++)
            {
                while (rig.velocity.x == 0f && rig.velocity.y == 0f && health > 0)
                {
                    spriteRenderer.sprite = idleSprites[0];
                    yield return null;
                }
                spriteRenderer.sprite = health > 0 ? idleSprites[i] : deadSprite;
                yield return new WaitForSeconds(0.12f);
            }
            
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (health > 0)
        {
            Destroy(other.gameObject);
            health--;
            healthText.text = "Health: " + health;
            audioSource.clip = myLeg;
            audioSource.Play();
            if (health <= 0)
            {
                StartCoroutine(GameOver());
            }
        }


    }
}
