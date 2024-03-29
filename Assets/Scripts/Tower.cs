using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform towerRotatePoint;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] private GameObject upgradeButton;

    [Header("Attribute")]
    [SerializeField] float rotationSpeed = -10f;
    [SerializeField] float range = 5f;
    [SerializeField] float shootSpeed = 1f;
    [SerializeField] private int towerWorth;
    [SerializeField] private int upgradeCost;
    [SerializeField] private int upgradeIncrement;
    [SerializeField] private int tier;

    [Header("Tower Type")]
    [SerializeField] bool isIceTower;
    [SerializeField] bool isNormalTower;
    [SerializeField] bool isShotGunTower;

    Transform target;
    float timeUntilFire;
    public bool canShoot = false;
    int[] soundNumber = { 2, 7, 8 };
    private int currentUpgradeCost;

    private void Start()
    {
        //Add tower to list
        GameManager.instance.activeTowers.Add(this);
        // Initialize currentUpgradeCost
        currentUpgradeCost = upgradeCost;
    }
    private void Update()
    {
        // Check if tower is able to upgrade
        CheckUpgradeQulification();
        //Switch different tower mode
        if (isNormalTower || isShotGunTower)
        {
            if (target == null)
            {
                FindTarget();
                return;
            }
            RotateTowardsTarget();
            if (!CheckTargetIsInRange())
            {
                target = null;
            }
            else
            {
                //fire timer
                timeUntilFire += Time.deltaTime;
                if (timeUntilFire >= 1f / shootSpeed && canShoot)
                {
                    //normal tower
                    if (isNormalTower)
                    {
                        NormalShoot();
                        timeUntilFire = 0f;
                    }
                    //shot gun tower
                    if (isShotGunTower)
                    {
                        ShotGunShoot();
                        timeUntilFire = 0f;
                    }
                }
            }
        }
        //freeze tower
        else if (isIceTower && canShoot)
        {
            FreezeAllTargetsInRange();
        }
    }
    /// <summary>
    /// NormalTower Attack
    /// </summary>
    private void NormalShoot()
    {
        GameObject bulletobj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bulletobj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
        //play random audio
        AudioManager.audioInstance.PlayAudio(soundNumber[UnityEngine.Random.Range(0, soundNumber.Length)]);
    }
    private void ShotGunShoot()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2)transform.position, 0f, enemyMask);
        if (hits.Length > 0)
        {
            //play audio
            AudioManager.audioInstance.PlayAudio(3);
            //for eachone shoot one bullet;
            for (int i = 0; i < hits.Length; i++)
            {
                target = hits[i].transform;
                GameObject bulletobj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                Bullet bulletScript = bulletobj.GetComponent<Bullet>();
                bulletScript.SetTarget(target);
            }
        }
    }
    private void FindTarget()
    {
        //find enemy
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2)transform.position, 0f, enemyMask);
        if (hits.Length > 0)
        {
            //get enemy's transform
            target = hits[0].transform;
        }

    }

    private void RotateTowardsTarget()
    {
        //Find the rotated angle
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        //Let rotate not quickly
        towerRotatePoint.rotation = Quaternion.RotateTowards(towerRotatePoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(transform.position, target.position) <= range;
    }
    private void FreezeAllTargetsInRange()
    {
        Vector2 towerPosition = transform.position;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2)transform.position, 0f, enemyMask);
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                Vector2 enemyPosition = enemy.transform.position;
                float distance = Vector2.Distance(enemyPosition, towerPosition);
                if (distance <= range)
                {
                    enemy.ChangeSpeed(0.99f);
                }
                else
                {
                    enemy.ResetSpeed();
                }
            }
        }
    }
    /// <summary>
    /// for testing see the range in gizmo
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        //Range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2)transform.position, 0f, enemyMask);
        Gizmos.color = Color.red;
        foreach (RaycastHit2D hit in hits)
        {
            //Hit target
            Gizmos.DrawLine(transform.position, hit.point);
            Gizmos.DrawWireSphere(hit.point, 0.2f);
        }
    }
    /// <summary>
    /// for building in case of overlap
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            BuildingManager.buildingManager.canPlace = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            BuildingManager.buildingManager.canPlace = true;
        }
    }
    public void TowerUpgrade()
    {
        // Some upgrade cost update
        GameManager.instance.CostGold(currentUpgradeCost);
        tier++;
        currentUpgradeCost += currentUpgradeCost + upgradeCost + upgradeIncrement * tier;
        // Actual Attributes Boost
        shootSpeed += 1f;
        range += 0.2f;
        //play audio
        AudioManager.audioInstance.PlayAudio(5);
        // Disable the upgrade btn
        upgradeButton.SetActive(false);
    }

    private void CheckUpgradeQulification()
    {
        if (GameManager.instance.getGold() > currentUpgradeCost)
        {
            upgradeButton.SetActive(true);
        }
        else
        {
            upgradeButton.SetActive(false);
        }
    }

    public int GetTowerWorth()
    {
        return towerWorth;
    }

}
