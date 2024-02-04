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
    void Update()
    {
        //Timer
        timeUntilFire += Time.deltaTime;
        //get mouse position
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RotateTarget();
        //Press W To shoot
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
        //play audio
        AudioManager.audioInstance.PlayAudio(1);
        bulletScript.SetPosition(givenVector2);
    }
    void RotateTarget()
    {
        //get the rotation indicate mouse position
        float angle = Mathf.Atan2(mouseWorldPos.y - transform.position.y, mouseWorldPos.x - transform.position.x) * Mathf.Rad2Deg-90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        towerRotatePoint.rotation = targetRotation;
    }
}
