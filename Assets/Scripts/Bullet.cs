using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Transform target;
    Vector2 tempPos;
    [Header("References")]
    [SerializeField] Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float destroyTimer = 3f;
    [Header("BulletType")]
    [SerializeField] bool isNormalBullet;
    [SerializeField] bool isShotGunBullet;
    [SerializeField] bool isMainTowerBullet;
    Vector2 direction = Vector2.zero;
    [SerializeField] private GameObject hitEffect;
    private void Start()
    {
        //destroy after destroyTimer(incase not touch the enemy)
        Destroy(gameObject,destroyTimer);
    }
    public void SetTarget(Transform givenEnemy)
    {
        //set target inside Tower Script
        target = givenEnemy;
    }
    public void SetPosition(Vector2 givenVector2)
    {
        //set position
        tempPos = givenVector2;
    }
    private void FixedUpdate()
    {
        //incase error
        if ((!target&&(isNormalBullet||isShotGunBullet))||(tempPos==null))
        {
            return;
        }
        //switch different bullet type
        if (isNormalBullet)
        {
            direction = (target.position - transform.position).normalized;
        }
        if (isShotGunBullet)
        {
            direction = target.position.normalized;
        }
        if (isMainTowerBullet)
        {
            direction = tempPos.normalized;
        }
        rb.velocity = direction * bulletSpeed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //cause damage
        collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
        // Particle Effect
        Instantiate(hitEffect, transform.position, quaternion.identity);
        Destroy(gameObject);

    }
    public void BulletUpgrade() 
    {
        bulletDamage += 1;
        bulletSpeed += 1;
    }
}
