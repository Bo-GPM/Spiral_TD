using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MainTower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform firePoint;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform towerRotatePoint;
    [Header("Attribute")]
    [SerializeField] float shootColdDown = 1f;
    
    Vector2 Mousepos;
    Vector3 mouseWorldPos;
    float timeUntilFire;
    // Update is called once per frame
    void Update()
    {
        timeUntilFire += Time.deltaTime;
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RotateTarget();
        if (Input.GetKeyDown(KeyCode.W)&&timeUntilFire >= 1f /shootColdDown)
        {
                Mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Fire(Mousepos);
                timeUntilFire = 0f;
        }
    }
    private void Fire(Vector2 givenVector2)
    {
        GameObject bulletobj = Instantiate(bulletPrefab,transform.position,Quaternion.identity);
        Bullet bulletScript = bulletobj.GetComponent<Bullet>();
        bulletScript.SetPosition(givenVector2);
        //Debug.Log("Shoot");
    }
    void RotateTarget()
    {
        float angle = Mathf.Atan2(mouseWorldPos.y - transform.position.y, mouseWorldPos.x - transform.position.x) * Mathf.Rad2Deg-90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        towerRotatePoint.rotation = targetRotation;
    }
}
