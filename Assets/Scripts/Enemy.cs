using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    // [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int healthPool;
    [SerializeField] private int damageNumber;
    private int currentHealthPool;
    [SerializeField] private bool isFlying = false;
    private int currentNavIndex;
    private Transform nextNavPointTransform;
    [SerializeField] private float toleranceDistance;
    private float originalSpeed;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize Add data required
        InitializeEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        // Move Method transferred to FixedUpdate()
        // EnemyMove();
        CheckAlive();
    }

    public void ChangeSpeed(float tempSpeed)
    {
        originalSpeed = moveSpeed;
        moveSpeed *= tempSpeed;
    }

    public void ResetSpeed()
    {
        moveSpeed = originalSpeed;
    }
    
    private void FixedUpdate()
    {
        EnemyMove();
    }

    private void InitializeEnemy()
    {
        currentHealthPool = healthPool;
        GameManager.instance.activeEnemies.Add(this);
        
        // Check if enemy is flying, if not, set index to 0
        // If flying, get the final index.
        if (isFlying)
        {
            currentNavIndex = GameManager.instance.navPointsArray.Length - 1;
        }
        else
        {
            currentNavIndex = 0;
        }
        SetCurrentIndexAsGuidePointTransform();
    }
    
    void EnemyMove()
    {
        // TODO: Follow guidance pins
        // 1. check the distance between enemy and next navPoint
        float distanceFromGuidePoint;
        distanceFromGuidePoint = (transform.position - nextNavPointTransform.position).magnitude;
        
        // 2. if distance is too far, move towards next point
        //    if close enough, change nav point to next (if current point is the last, stop)
        if (distanceFromGuidePoint < toleranceDistance)
        {
            if (currentNavIndex == GameManager.instance.navPointsArray.Length - 1)
            {
                // Stop moving and report error
                Debug.LogError("Enemy reached final guide point.");
            }
            else
            {
                currentNavIndex++;
                SetCurrentIndexAsGuidePointTransform();
            }
        }
        
        // 3. Move to NavPoint
        Vector3 targetVec = -(transform.position - nextNavPointTransform.position).normalized;
        gameObject.transform.position += targetVec * (moveSpeed * Time.fixedDeltaTime);
    }
    
    private void SetCurrentIndexAsGuidePointTransform()
    {
        // Set Current index's transform as the next navPoint transform
        nextNavPointTransform = GameManager.instance.navPointsArray[currentNavIndex].transform;
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
