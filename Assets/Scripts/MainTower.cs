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

    [Header("Attribute")]
    [SerializeField] float shootColdDown = 1f;
    
    Vector2 Mousepos;
    float timeUntilFire;
    // Update is called once per frame
    void Update()
    {
        timeUntilFire += Time.deltaTime;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
}
