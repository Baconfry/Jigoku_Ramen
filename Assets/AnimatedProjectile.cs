using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedProjectile : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float secondsPerFrame;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(PlayAnimation());
        Destroy(this.gameObject, 8f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlayAnimation()
    {
        while (true)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(secondsPerFrame);
            }
        }
    }
}
