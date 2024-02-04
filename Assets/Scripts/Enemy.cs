using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    // [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int healthPool;
    [SerializeField] private int damageNumber;
    [SerializeField] private int goldWorth;
    private int currentHealthPool;
    [SerializeField] private int hpIncrementPerWaves;
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
        originalSpeed = moveSpeed;
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
        //moveSpeed *= tempSpeed;
        if (moveSpeed>= (originalSpeed*0.7))
        {
            moveSpeed *= tempSpeed;
        }

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
        // Upgrade HP and moveSpeed as waves goes
        currentHealthPool = healthPool + hpIncrementPerWaves * GameManager.instance.getWaves();
        moveSpeed *= 1 + 0.1f * GameManager.instance.getWaves();
        
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
        
        // 4. Change rotation, make it always facing the next checkpoint
        if (currentNavIndex <= GameManager.instance.navPointsArray.Length - 1)
        {
            float angle = Mathf.Atan2(targetVec.y, targetVec.x) * Mathf.Rad2Deg;
            transform.rotation = UnityEngine.Quaternion.Euler(0, 0, angle);
        }
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
        AudioManager.audioInstance.PlayAudio(0);
    }

    public void CheckAlive()
    {
        // Check destination first, then check HP, if HP < 0, die and give gold
        if (currentHealthPool <= 0)
        {
            GameManager.instance.activeEnemies.Remove(this);
            GameManager.instance.AddGold(goldWorth);
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
