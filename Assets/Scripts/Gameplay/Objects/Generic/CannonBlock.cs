using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBlock : BaseLevelObject
{
    // ----------------------
    //
    // This object shoots out harmful projectiles based on a timer
    //
    // ----------------------

    [Space(10)]
    public CannonProjectile Projectile;
    public int ProjectilePoolAmount;

    [Space(10)]
    public Transform ShootPosition;
    public float ShootForce;
    public float ShootTime;
    public float ShootDelay;

    private List<Rigidbody2D> pooledProjectiles = new List<Rigidbody2D>();

    private void OnEnable()
    {
        LevelController.EnableObjects.AddListener(Activate);
        LevelController.DisableObjects.AddListener(Deactivate);
    }

    private void OnDisable()
    {
        LevelController.EnableObjects.RemoveListener(Activate);
        LevelController.DisableObjects.RemoveListener(Deactivate);
    }

    public override void Start()
    {
        base.Start();
        if (ProjectilePoolAmount > 0)
        {
            for (int i = 0; i < ProjectilePoolAmount; i++)
            {
                CannonProjectile newProjectile = Instantiate(Projectile);
                newProjectile.CannonCol = col;
                newProjectile.transform.position = transform.position;
                newProjectile.transform.localScale = transform.localScale;
                newProjectile.gameObject.SetActive(false);

                pooledProjectiles.Add(newProjectile.GetComponent<Rigidbody2D>());
            }
        }
    }

    private void Activate()
    {
        StartCoroutine(DelayShooting());
    }

    private void Deactivate()
    {
        StopAllCoroutines();
    }

    public override void Update()
    {
        
    }

    private IEnumerator DelayShooting()
    {
        yield return new WaitForSeconds(ShootDelay);
        StartCoroutine(ShootProjectile());
    }

    private IEnumerator ShootProjectile()
    {
        Rigidbody2D shotProjectile = pooledProjectiles.Find(x => !x.gameObject.activeSelf);

        if (shotProjectile != null)
        {
            shotProjectile.transform.position = transform.position + (transform.right * (transform.localScale.x + 0.1f));
            shotProjectile.velocity = Vector2.zero;
            shotProjectile.gameObject.SetActive(true);
            shotProjectile.AddForce(transform.right * ShootForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(ShootTime);

        StopCoroutine(ShootProjectile());
        StartCoroutine(ShootProjectile());
    }
}
