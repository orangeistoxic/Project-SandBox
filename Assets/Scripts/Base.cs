using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.AI;

public class Base : MonoBehaviour
{
    /// <summary>
    /// Faction of the base
    /// 0: Blue, 1: Red
    /// </summary>
    private int faction;
    private float storedResource;
    private float unitCost;
    private float reachResourceRange;
    private int numGatherer;
    private int numFighter;
    private int numSentinel;
    private float operationRange;

    private float spawnTimer;

    public GameObject unit;
    private List<GameObject> resourceList ;
    public GameObject dotForRange;

    public void BasicStatInit()
    {
        if(gameObject.layer == 6) //Team Blue
        {
            faction = 0;
        }
        else if(gameObject.layer == 7) //Team Red
        {
            faction = 1;
        }
        else
        {
            faction = 0;
        }
        storedResource = 10f;
        unitCost = 10f;
        reachResourceRange = 30f;
        numGatherer = 0;
        numFighter = 0;
        numSentinel = 0;
        operationRange = 3.3f;
        spawnTimer = 0f;
        resourceList = new List<GameObject>();

    }

    void Awake()
    {
        BasicStatInit();
        LocateReachableResource();
        
        switch (faction)
        {
            case 0:
                unit = Resources.Load<GameObject>("Unit_Blue");
                dotForRange = Resources.Load<GameObject>("BlueDot_forIndicator");
                break;
            case 1:
                unit = Resources.Load<GameObject>("Unit_Red");
                dotForRange = Resources.Load<GameObject>("RedDot_forIndicator");
                break;
            default:
                break;
        }
        ShowRange();
    }

    void Update()
    {
        SpwanUnit();
        spawnTimer -= Time.deltaTime;
        Debug.Log(Time.deltaTime);
        

    }

    void ShowRange(){
        GameObject rangeIndicator = Instantiate(dotForRange,transform.position,Quaternion.identity);
        rangeIndicator.transform.localScale = new Vector3(reachResourceRange,1,reachResourceRange);
    }

    void LocateReachableResource(){
        GameObject[] potentialResource = GameObject.FindGameObjectsWithTag("Resource");
        foreach (var resource in potentialResource)
        {
            if(Vector3.Distance(resource.transform.position,transform.position)<=reachResourceRange)
            {
                resourceList.Add(resource);
            }
        }
    }

    void SpwanUnit(){
        float spwanRange=3.5f;
        if(storedResource>=unitCost && spawnTimer<=0){
            storedResource-=unitCost;
            unit.GetComponent<Unit>().Base = gameObject;
            float x=UnityEngine.Random.Range(-spwanRange,spwanRange);
            float z=Mathf.Sqrt(spwanRange*spwanRange-x*x);
            if(UnityEngine.Random.value>0.5){
                z=-z;
            }
            Instantiate(unit,new Vector3(transform.position.x+x,transform.position.y,transform.position.z+z),Quaternion.identity);
            spawnTimer=1f;

        }
    }

    public GameObject SearchBestResourceLocation()
    {
        GameObject bestResourceLocation = null;
        float bestResourceDistance = float.MaxValue;
        
        
        if(resourceList.Count > 0) //If there is resource available, find the nearest one
        {
            foreach (var resource in resourceList)
            {
                float distance = Vector3.Distance(resource.transform.position, transform.position);
                if (distance < bestResourceDistance)
                {
                    bestResourceDistance = distance;
                    bestResourceLocation = resource;
                }

            }
            return bestResourceLocation;
        }
        else //If not, return null, then and the Unit should call Base to offer other jobs
        {
            return bestResourceLocation;
        }
    }

    /// <summary>
    /// Base on numGatherer,numFighter,numSentinel, current available resource, enemy threat, etc
    /// to determine the job for the unit
    /// 
    /// Note: test version. I only change the problem ones to sentinel, if no problem, Gatherer.
    /// </summary>
    public int ConsiderBestJob()
    {
        if(resourceList.Count == 0)
        {
            return 3;
        }
        else
        {
            return 1;
        }
    }

    public bool DepositResource(float amount)
    {
        if (amount <= 0)
        {
            return false;
        }
        else
        {
            storedResource += amount;
            return true;
        }
    }

    public bool IsUnitInRange(Vector3 unitPisition)
    {
        float distance = Vector3.Distance(unitPisition, transform.position);
        if (distance <= operationRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


