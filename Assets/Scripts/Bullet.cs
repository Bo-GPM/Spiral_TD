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
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * bulletSpeed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
        //TODO:Damage
        Destroy(gameObject);
    }
}
