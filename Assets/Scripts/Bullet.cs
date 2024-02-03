using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Transform target;
    [Header("References")]
    [SerializeField] Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float destroyTimer = 3f;
    [Header("BulletType")]
    [SerializeField] bool isNormalBullet;
    [SerializeField] bool isShotGunBullet;
    Vector2 direction = Vector2.zero;
    private void Start()
    {
        Destroy(gameObject,destroyTimer);
    }
    public void SetTarget(Transform givenEnemy)
    {
        //set target inside Tower Script
        target = givenEnemy;
    }
    private void FixedUpdate()
    {
        
        //bullet can follow the target
        if (!target)
        {
            return;
        }
        if (isNormalBullet)
        {
            direction = (target.position - transform.position).normalized;
        }
        if (isShotGunBullet)
        {
            direction = target.position.normalized;
        }
        rb.velocity = direction * bulletSpeed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
        //TODO:Damage
        Destroy(gameObject);

    }
    public void BulletUpgrade() 
    {
        //TODO:MoneyCondition
        bulletDamage += 1;
        bulletSpeed += 1;
    }
}
