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
    [SerializeField] private LayerMask upgradeBtnMask;

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
        // Gold Costing here
        if (pendingTower != null) GameManager.instance.CostGold(pendingTower.GetComponent<Tower>().GetTowerWorth());
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
        
        // Check for UpgradeButton is clicked
        RaycastHit2D hitBtn = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, upgradeBtnMask);
        if (hitBtn.collider != null)
        {
            // Debug.LogError($"Upgrade BTN Hit!, the name is {hitBtn.collider.transform.parent.gameObject.name}");
            if (Input.GetMouseButtonDown(0))
            {
                // Debug.LogError("Successfully Clicked");
                hitBtn.collider.transform.parent.gameObject.GetComponent<Tower>().TowerUpgrade();
            }
        }
    }
    /// <summary>
    /// call in UI click Instatntiate Tower
    /// </summary>
    /// <param name="index"></param>
    public void SelectObject(int index)
    {
        // Check if gold is enough
        if (towersPrefab[index].GetComponent<Tower>().GetTowerWorth() <= GameManager.instance.getGold())
        {
            pendingTower = Instantiate(towersPrefab[index], pos, Quaternion.identity);
            childTransform = pendingTower.transform.Find("Root");
            
        }
        else
        {
            GameManager.instance.InsufficientGoldWarning();
        }
    }
}
