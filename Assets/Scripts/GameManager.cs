using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public GameManager instance;
    [HideInInspector] public List<Enemy> activeEnemies= new List<Enemy>();
    
    private int gameState = 0;
    private const int PREPARE_STAGE = 0;
    private const int BATTLE = 1;

    [Header("General Varibles")] 
    [SerializeField] private int initialGold;
    private int gold;
    private int currentWaves = 1;
    
    [Header("Enemy Related")] 
    [SerializeField] private Transform enemySpawningLocation;
    [SerializeField] private GameObject[] enemyList;
    [SerializeField] private float[] enemySpawnInterval;
    private float[] enemySpawnIntervalRemain;
    private int[] enemiesAwaitsSpawn;
    [SerializeField] private float prepareTime;
    private float remainingPrepareTime;
    
    // [Header("Temp Enemy Spawning")]
    // [SerializeField] private float tempEnemyRefreshInterval;    //In the future, there will be better spawning logics
    // [SerializeField] private float tempEnemyWaveTotalNumber;    //Same here

    [Header("Tower Related")] 
    [SerializeField] private int finalTowerHP;
    
    // Might be useful for reload the scene
    private void Awake()
    {
        // Singleton
        // if (instance != null)
        // {
        //     instance = this;
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
        // DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeSpawnContent();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == PREPARE_STAGE)
        {
            // Update remaining PrepareTime and prepare for stage Shifting
            remainingPrepareTime -= Time.deltaTime;
            
            // Change gameState to battle;
            if (remainingPrepareTime < 0)
            {
                gameState = BATTLE;
            }
        }
        else if (gameState == BATTLE)
        {
            // Spawning enemies before awaits list is empty
            if (!CheckIntArrayIsAllZero(enemiesAwaitsSpawn))
            {
                SpawnEnemy();
            }
            // If there's no more enemies to spawn
            // it wait until there's no enemies and change the state
            else 
            {
                if (activeEnemies.Count == 0)
                {
                    currentWaves++;
                    gameState = PREPARE_STAGE;
                    InitializeSpawnContent();
                }
            }
        }
    }

    private bool CheckIntArrayIsAllZero(int[] tempArray)
    {
        for (int i = 0; i < tempArray.Length; i++)
        {
            if (tempArray[i] != 0)
            {
                return false;
            }
        }

        return true;
    }
    
    private void InitializeSpawnContent()
    {
        // Call this function once before next wave started
        // 0. Initialize enemy related arrays
        // 1. Update enemySpawnNumber
        // 2. refresh enemySpawnInterval
        // 3. Refresh remainingPrepareTime

        // Initialize those arrays
        enemySpawnIntervalRemain = new float[enemyList.Length];
        enemiesAwaitsSpawn = new int[enemyList.Length];
        
        // Logic for the first kind of enemy (Very sketchy)
        int enemy1BaseNumber = 3;
        int enemy1WaveFactor = 5;
        int enemy1toAdd = enemy1BaseNumber + enemy1WaveFactor * currentWaves;
        enemiesAwaitsSpawn[0] = enemy1toAdd;
        
        
        // Update enemySpawnInterval here
        float intervalDecreaseIncrement = 0.1f;
        // Then update enemySpawnIntervalRemain
        for (int i = 0; i < enemyList.Length; i++)
        {
            enemySpawnInterval[i] -= intervalDecreaseIncrement;  
            enemySpawnIntervalRemain[i] = enemySpawnInterval[i];
        }
        
        // Refresh remainingPrepareTime
        remainingPrepareTime = prepareTime;

    }
    
    private void SpawnEnemy()
    {
        // Check Condition then spawn enemy
        for (int i = 0; i < enemyList.Length; i++)
        {
            if (EnemySpawnCheck(i))
            {
                Instantiate(enemyList[i], enemySpawningLocation.position, quaternion.identity);
                enemiesAwaitsSpawn[i]--;
            }
        }
        
    }

    private bool EnemySpawnCheck(int enemyType)
    {
        // Update and them check if spawn condition is met
        enemySpawnIntervalRemain[enemyType] -= Time.deltaTime;
        if (enemySpawnIntervalRemain[enemyType] < 0)
        {
            if (enemiesAwaitsSpawn[enemyType] > 0)
            {
                enemySpawnIntervalRemain[enemyType] += enemySpawnInterval[enemyType];
                return true;
            }
            
        }

        return false;
    }
    
}
