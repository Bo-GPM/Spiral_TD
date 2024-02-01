using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    static public BuildingManager buildingManager;
    [Header("Reference")]
    [SerializeField] GameObject[] towersPrefab;
    [SerializeField] LayerMask groundMask;

    [Header("Detection")]

    Vector2 pos;
    GameObject pendingTower;
    public bool canPlace=true;
    Transform childTransform;
    private void Awake()
    {
        //Singleton
        if (buildingManager == null)
        {
            buildingManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        UpdateColors();
        //if you slect a tower
        if (pendingTower != null)
        {
            //change the position by frame
            pendingTower.transform.position = pos;
        }
        //trigger detection inside Tower script
        if (Input.GetMouseButtonDown(0)&&canPlace)
        {
            PlaceTower();
        }
    }
    public void PlaceTower()
    {
        //TODO:COIN
        //place tower & toggle shooting function
        if(pendingTower!=null) pendingTower.GetComponent<Tower>().canShoot = true;
        pendingTower = null;
    }

    private void UpdateColors()
    {

        if (canPlace && pendingTower != null)
        {

            childTransform.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (!canPlace && pendingTower != null)
        {
            childTransform.GetComponent<SpriteRenderer>().color = Color.red;

        }

    }
    private void FixedUpdate()
    {
        //track mouse position 
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity,groundMask);
        if (hit.collider != null)
        {
            //save the mouse position
            pos = mouseWorldPos;
        }     
    }
    /// <summary>
    /// call in UI click Instatntiate Tower
    /// </summary>
    /// <param name="index"></param>
    public void SelectObject(int index)
    {
        pendingTower = Instantiate(towersPrefab[index], pos, Quaternion.identity);
        childTransform = pendingTower.transform.Find("Root");
    }
}
