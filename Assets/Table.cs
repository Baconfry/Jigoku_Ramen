using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    private enum BreathLevel { none, normal, flamethrower}
    private BreathLevel breathLevel;
    
    [SerializeField] private SpriteRenderer patronSprite;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private int desiredSpiceLevel;
    private int actualSpiceLevel;
    public bool finishedOrder;

    [SerializeField] private GameObject fireball;
    private GameObject ramenBowl;

    private Player player;

    private AudioSource audioSource;
    [SerializeField] private AudioClip tomScream;
    [SerializeField] private AudioClip wrong;
    [SerializeField] private AudioClip cashRegister;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Player").GetComponent<Player>();
        StartCoroutine(SpawnPatrons());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GiveOrder(int providedSpiceLevel, GameObject newRamen)
    {
        ramenBowl = newRamen;
        ramenBowl.transform.parent = this.gameObject.transform;
        ramenBowl.transform.localPosition = Vector3.zero;
        if (providedSpiceLevel > actualSpiceLevel)
        {
            audioSource.clip = tomScream;
            audioSource.Play();
            breathLevel = BreathLevel.flamethrower;
        }
        else if (providedSpiceLevel > 0) //&& providedSpiceLevel == actualSpiceLevel)
        {
            breathLevel = BreathLevel.normal;
        }
        else
        {
            breathLevel = BreathLevel.none;
        }
        AudioSource ramenAudioSource = newRamen.GetComponent<AudioSource>();
        if (providedSpiceLevel == desiredSpiceLevel)// || providedSpiceLevel == actualSpiceLevel)
        {
            player.UpdateScore(100 + (100 * providedSpiceLevel));
            ramenAudioSource.clip = cashRegister;
        }
        else
        {
            player.UpdateScore(-100);
            ramenAudioSource.clip = wrong;
        }
        ramenAudioSource.Play();
        finishedOrder = true;
    }

    IEnumerator SpawnPatrons()
    {
        while (true)
        {
            finishedOrder = false;
            patronSprite.enabled = false;
            boxCollider.enabled = false;
            spriteRenderer.color = Color.white;
            float emptyTableTime = Random.Range(2f, 30f);
            yield return new WaitForSeconds(emptyTableTime);

            patronSprite.enabled = true;
            boxCollider.enabled = true;
            desiredSpiceLevel = Random.Range(0, 3);
            if (desiredSpiceLevel == 0) desiredSpiceLevel = Random.Range(0, 3); //make 0 spice level less likely
            actualSpiceLevel = desiredSpiceLevel;
            if (Random.Range(0, 5) == 0 && desiredSpiceLevel > 0) actualSpiceLevel--;
            
            switch (desiredSpiceLevel)
            {
                case 0:
                    spriteRenderer.color = Color.white;
                    break;
                case 1:
                    spriteRenderer.color = new Color(1f, 0.7f, 0f, 1f);
                    break;
                case 2:
                    spriteRenderer.color = Color.red;
                    break;
                default:
                    break;
            }
            //visual indicator
            while (!finishedOrder)
            {
                yield return null;
            }
            yield return new WaitForSeconds(2f);
            float startTime = Time.time;
            while (Time.time < startTime + 8f)
            {
                if (breathLevel != BreathLevel.none)
                {
                    Vector3 randomizeTrajectory = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), 0f);
                    Instantiate(fireball, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = -patronSprite.gameObject.transform.localPosition + randomizeTrajectory;
                    yield return new WaitForSeconds(breathLevel == BreathLevel.normal ? 2f : 1f);
                }
                yield return null;
            }

            Destroy(ramenBowl.gameObject);

        }



    }

}
