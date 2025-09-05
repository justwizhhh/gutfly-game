using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBlock : BaseLevelObject
{
    // ----------------------
    //
    // When destroyed, this object spews out lots of collectibles for the player to collect
    //
    // ----------------------

    [Space(10)]
    public Collectible Collectible;
    public int CollectibleAmount;

    [Space(10)]
    public float CollectibleMinBurstForce;
    public float CollectibleMaxBurstForce;
    public float CollectibleBurstTime;

    [Space(10)]
    public Sprite[] RandomTextures;

    // Object/component references
    private List<Collectible> allCollectibles = new List<Collectible>();

    public override void Start()
    {
        base.Start();
        for (int i = 0; i < CollectibleAmount; i++)
        {
            Collectible newCollectible = Instantiate(Collectible);

            newCollectible.gameObject.SetActive(false);
            newCollectible.transform.position = transform.position;
            newCollectible.MinPlayerRadius = 0;

            allCollectibles.Add(newCollectible);
        }

        if (RandomTextures.Length != 0)
        {
            sr.sprite = RandomTextures[Random.Range(0, RandomTextures.Length - 1)];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(col, collision);

            rb.simulated = false;
            sr.enabled = false;

            foreach (Collectible collectible in allCollectibles)
            {
                collectible.transform.position = transform.position;
                collectible.gameObject.SetActive(true);
                collectible.public_col.enabled = false;

                // Burst force the collectibles in random directions
                Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                collectible.public_rb.AddForce(dir * Random.Range(CollectibleMinBurstForce, CollectibleMaxBurstForce), ForceMode2D.Impulse);
            }

            StartCoroutine(ActivateAllCollectibles());
        }
    }

    private IEnumerator ActivateAllCollectibles()
    {
        yield return new WaitForSeconds(CollectibleBurstTime);

        foreach (Collectible collectible in allCollectibles)
        {
            collectible.ForceFollowPlayer();
            collectible.public_col.enabled = true;
        }
    }
}
