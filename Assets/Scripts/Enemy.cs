using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int healthPool;
    [SerializeField] private int damageNumber;
    private int currentHealthPool;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize Add data required
        InitializeEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        // use this move method later
        EnemyMove();
        CheckAlive();
    }

    private void InitializeEnemy()
    {
        currentHealthPool = healthPool;
        GameManager.instance.activeEnemies.Add(this);
    }
    void EnemyMove()
    {
        // TODO: Follow guidance pins
        
        // Really simple moves
        gameObject.transform.position += Vector3.right * moveSpeed;
    }

    public void TakeDamage(int tempDamageNumber)
    {
        // Call this for taking damage
        currentHealthPool -= tempDamageNumber;
    }

    public void CheckAlive()
    {
        // Check destination first, then check HP
        if (currentHealthPool <= 0)
        {
            GameManager.instance.activeEnemies.Remove(this);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag($"Exit"))
        {
            GameManager.instance.TowerDamageTaken(damageNumber);
            GameManager.instance.activeEnemies.Remove(this);
            Destroy(gameObject);
        }
    }
}
