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

    [Header("Attribute")]
    [SerializeField] float range = 5f;
    [SerializeField] float shootSpeed = 1f;
    [SerializeField] private int towerWorth;

    [Header("Tower Type")]
    [SerializeField] bool isIceTower;
    [SerializeField] bool isNormalTower;
    [SerializeField] bool isShotGunTower;

    Transform target;
    float timeUntilFire;
    public bool canShoot = false;
    private void Start()
    {
        //Add tower to list
        GameManager.instance.activeTowers.Add(this);
    }
    private void Update()
    {
        //Switch different tower mode
        if (isNormalTower|| isShotGunTower)
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
                timeUntilFire += Time.deltaTime;
                if (timeUntilFire >= 1f / shootSpeed && canShoot)
                {
                    if (isNormalTower)
                    {
                        NormalShoot();
                        timeUntilFire = 0f;
                    }
                    if (isShotGunTower)
                    {
                        ShotGunShoot();
                        timeUntilFire = 0f;
                    }
                }
            }
        }
        else if (isIceTower && canShoot)
        {
            FreezeAllTargetsInRange();
        }
    }

    private void NormalShoot()
    {
        GameObject bulletobj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bulletobj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
        //Debug.Log("Shoot");
    }
    private void ShotGunShoot()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2)transform.position, 0f, enemyMask);
        if (hits.Length > 0)
        {
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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2)transform.position, 0f, enemyMask);
        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }

    }
    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg;
        towerRotatePoint.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
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
                //StartCoroutine(StopSlowEnemy(enemy));
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
        //TODO:Money condition
        shootSpeed += 1f;
        range += 0.2f;
    }

    public int GetTowerWorth()
    {
        return towerWorth;
    }
}
