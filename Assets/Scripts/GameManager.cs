using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector] static public GameManager instance;
    [HideInInspector] public List<Enemy> activeEnemies = new List<Enemy>();
    [HideInInspector] public List<Tower> activeTowers = new List<Tower>();

    private int gameState = 0;
    private const int PREPARE_STAGE = 0;
    private const int BATTLE = 1;
    private const int GAME_OVER = 2;

    [Header("General Varibles")] 
    [SerializeField] private int initialGold;
    private int gold;
    private int currentWaves = 1;
    
    [Header("Enemy Related")] 
    [SerializeField] private Transform enemySpawningLocation;
    [SerializeField] private GameObject[] enemyList;
    [SerializeField] private float[] enemySpawnInterval;
    private float[] enemySpawnIntervalRemain;
    private int totalEnemiesToSpawn;
    private int[] enemiesAwaitsSpawn;
    [SerializeField] private float totalTimeToTakeForAllEnemySpawning;
    [SerializeField] private float totalTimeIncrement;
    [SerializeField] private float prepareTime;
    private float remainingPrepareTime;
    [SerializeField] public GameObject[] navPointsArray;
    
    // [Header("Temp Enemy Spawning")]
    // [SerializeField] private float tempEnemyRefreshInterval;    //In the future, there will be better spawning logics
    // [SerializeField] private float tempEnemyWaveTotalNumber;    //Same here

    [Header("Tower Related")] 
    [SerializeField] private int finalTowerHP;

    int currentSceneIndex = 0;
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] GameObject titleScreenPanel;
    [SerializeField] GameObject buildingPanel;
    [SerializeField] private Text goldText;
    [SerializeField] private Text healthPoolText;
    [SerializeField] private GameObject countDownText;
    private Text countDownTextComponent;
    [SerializeField] private Text waveText;
    [SerializeField] private Slider waveProgressSlider;

    // Might be useful for reload the scene
    
    
    private void Awake()
    {
        gold = initialGold;
        
        instance = this;
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
        GameStateChanging();
        
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

    private void GameStateChanging()
    {
        
        // Check gameState and transfer to corresponding state
        if (gameState == PREPARE_STAGE)
        {
            PrepareUpdate();
        }
        else if (gameState == BATTLE)
        {
            // TODO: Fix Temp Solution
            countDownText.SetActive(false);
            
            
            BattleUpdate();
        } 
        else if (gameState == GAME_OVER)
        {
            gameOverPanel.SetActive(true);
        }
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
        
        // Logic for the 1st kind of enemy 
        /*
         * Creeps:
         * Refresh every wave
         * move slow
         * low Hp
         */
        int enemy1BaseNumber = 3;
        int enemy1WaveFactor = 5;
        int enemy1ToAdd = enemy1BaseNumber + enemy1WaveFactor * currentWaves;
        enemiesAwaitsSpawn[0] = enemy1ToAdd;
        
        // Logic for the 2nd kinds 
        /*
         * Flying Creeps
         * Only Refresh when waves % 2 == 0
         * Moves very slow
         * Low HP
         */
        if (currentWaves % 2 == 0)
        {
            int enemy2BaseNumber = 6;
            int enemy2WaveFactor = 2;
            int enemy2ToAdd = enemy2BaseNumber + enemy2WaveFactor * currentWaves;
            enemiesAwaitsSpawn[1] = enemy2ToAdd;
        }
        
        // Logic for the 3rd kinds 
        /*
         * Boss
         * Only Refresh when waves  % 3 == 0
         * moves slow
         * HP very high
         */
        
        if (currentWaves % 3 == 0)
        {
            int enemy3BaseNumber = 4;
            int enemy3WaveFactor = 2;
            int enemy3ToAdd = enemy3BaseNumber + enemy3WaveFactor * currentWaves;
            enemiesAwaitsSpawn[2] = enemy3ToAdd;
        }
        
        // Initialize totalEnemiesToSpawn
        totalEnemiesToSpawn = CountEnemiesInList();
        
        // Update enemySpawnInterval here
        totalTimeToTakeForAllEnemySpawning = totalTimeToTakeForAllEnemySpawning + totalTimeIncrement * currentWaves;
        
        // Then update enemySpawnIntervalRemain
        for (int i = 0; i < enemyList.Length; i++)
        {
            enemySpawnInterval[i] = totalTimeToTakeForAllEnemySpawning / enemiesAwaitsSpawn[i];  
            enemySpawnIntervalRemain[i] = enemySpawnInterval[i];
        }
        
        // Refresh remainingPrepareTime
        remainingPrepareTime = prepareTime;
        // Set CountDownText as active
        countDownText.SetActive(false);
        
        // CountDowntextComponent Init
        countDownTextComponent = countDownText.GetComponent<Text>();
    }

    private void PrepareUpdate()
    {
        UpdateUIText();
        CountDownTimer();
        
        // Update remaining PrepareTime and prepare for stage Shifting
        remainingPrepareTime -= Time.deltaTime;
            
        // Change gameState to battle;
        if (remainingPrepareTime < 0)
        {
            gameState = BATTLE;
        }
    }
    private void BattleUpdate()
    {
        UpdateUIText();
        
        // Check HP first to see if game is over.
        if (finalTowerHP < 0)
        {
            gameState = GAME_OVER;
        }
        
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

    private int CountEnemiesInList()
    {
        int enemiesCount = 0;
        // Count how many enemies in enemiesAwaitsSpawn list
        for (int i = 0; i < enemiesAwaitsSpawn.Length; i++)
        {
            enemiesCount += enemiesAwaitsSpawn[i];
        }

        return enemiesCount;
    }
    private void CountDownTimer()
    {
        if (countDownText.active)
        {
            int tempSecondText =  (int) remainingPrepareTime;
            countDownTextComponent.text = tempSecondText.ToString();
            if (tempSecondText <= 3)
            {
                countDownTextComponent.fontSize = 300;
            }
            else
            {
                countDownTextComponent.fontSize = 60;
            }
        }
        else
        {
            countDownText.SetActive(true);
        }
    }
    
    private void UpdateUIText()
    {
        goldText.text = new string($"Gold: {gold}");
        healthPoolText.text = new string($"HP: {finalTowerHP}");
        waveText.text = new string($"Wave: {currentWaves}");
        float tempValueBeforeMap = Mathf.InverseLerp(0, totalEnemiesToSpawn, CountEnemiesInList());
        waveProgressSlider.value = Mathf.Lerp(1, 0, tempValueBeforeMap);
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

    public void AddGold(int goldToAdd)
    {
        gold += goldToAdd;
    }

    public void CostGold(int goldToCost)
    {
        gold -= goldToCost;
    }

    public int getGold()
    {
        return gold;
    }

    public void InsufficientGoldWarning()
    {
        StartCoroutine(startWarningCoroutine());
    }
    
    public IEnumerator startWarningCoroutine()
    {
        goldText.color = Color.red;
        yield return new WaitForSeconds(2);
        goldText.color = Color.black;
    }

    public void TowerDamageTaken(int damage)
    {
        finalTowerHP -= damage;
    }
    public void TransitionToNextLevel()
    {
        //TODO:
        currentSceneIndex++;
        if (currentSceneIndex > SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Trying to load{currentSceneIndex}.Aborting!Loading scene 0 instead");
            currentSceneIndex = 0;
        }
        titleScreenPanel.SetActive(currentSceneIndex == 0);
        buildingPanel.SetActive(currentSceneIndex != 0);
        SceneManager.LoadScene(currentSceneIndex);

    }
}
