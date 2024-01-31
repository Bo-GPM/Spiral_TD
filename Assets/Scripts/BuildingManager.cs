using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject[] towersPrefab;
    [SerializeField] LayerMask groundMask;
    Vector2 pos;
    GameObject pendingTower;

    private void Update()
    {
        //if you slect a tower
        if (pendingTower != null)
        {
            //change the position by frame
            pendingTower.transform.position = pos;
        }
        if (Input.GetMouseButtonDown(0))
        {
            //place tower
            PlaceTower();
        }
    }
    public void PlaceTower()
    {
        //TODO:COIN     
        pendingTower = null;

    }
    private void FixedUpdate()
    {
        //track mouse position 
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, groundMask);
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
        
    }
}
