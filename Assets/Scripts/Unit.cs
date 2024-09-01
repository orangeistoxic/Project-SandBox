using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.AI;
public class Unit : MonoBehaviour
{
     /// <summary>
     /// Base that the unit belongs to, and when unit spwan, it should be set by the Base
     /// Note: No need to set it by BasicStatInit().
     /// </summary>
     public GameObject Base;
     
     /// <summary>
     /// Unit's current status
     /// 0: No Job, 1:Gathering, 2: Fighter, 3:Sentinel 
     /// </summary>
     private int job;
     private float holdingCapacity;
     private float gatheredAmount;
     private bool isCarryingResource;
     public NavMeshAgent agent;

     void BasicStatInit()
     {
          job = 1;
          holdingCapacity = 3f;
          gatheredAmount = 0f;
          isCarryingResource = false;
          agent = GetComponent<NavMeshAgent>();

          
     }
     void Awake()
     {
          BasicStatInit();
          Debug.Log(Base.name);


     } 
     void Update()
     {
          switch (job)
          {
               case 0:
                    ToBase();
                    break;
               case 1:
                    Gatherer();
                    break;
               default:
                    break;
          }
     }

     

     
     void ToBase()
     {
          PathFinding(Base.transform.position);


          
     }
     /// <summary>
     /// Gatherer Job
     /// Basic Idea:
     /// if you don't have resource, request resource location from Base,and then go to the resource
     /// if you do have resource, go to the Base and request to deposit the resource.
     /// </summary>
     void Gatherer()
     {
          

          if(isCarryingResource)
          {
               if(Base.GetComponent<Base>().IsUnitInRange(transform.position))
               {
                    //Deposit Resource
                    bool isDeposited = RequestDepositResource(Base, gatheredAmount);
                    if(isDeposited)
                    {
                         gatheredAmount = 0;
                         isCarryingResource = false;


                    }
               }
               else
               {
                    //Go to Base
                    ToBase();

               }
          }
          else
          {
               //Request Resource Location from Base
               GameObject resource = Base.GetComponent<Base>().SearchBestResourceLocation();


               //then go to the resource
               // if there is no resource available, Base should offer other jobs for the unit
               //so do nothing

               if (resource == null)
               {
                    //No resource available
                    //Base should offer other jobs for the unit
                    UnitChangeJob(Base.GetComponent<Base>().ConsiderBestJob());
                    return;
               }

               Vector3 resourceLocation = resource.transform.position;
               PathFinding(resourceLocation);

               float distance = Vector3.Distance(resourceLocation, transform.position);
               
               if(resource.GetComponent<Resource>().IsUnitInRange(transform.position))
               {
                    //Gather Resource
                    isCarryingResource = ResquestGatherResource(resource, holdingCapacity);
                    Debug.Log("requesting to gather resource");
               }
               else
               {
                    //Debug.Log("Distance: " + distance );
               }

          }
     }

     public bool ResquestGatherResource(GameObject resource, float amount)
     {
          gatheredAmount += resource.GetComponent<Resource>().GatherResource(amount);

          if(gatheredAmount > 0)
          {
               return true;
          }
          else
          {
               return false;
          }
     }

     public  void UnitChangeJob(int newJob)
     {
          job = newJob;
     }
     public bool RequestDepositResource(GameObject baseObject, float amount)
     {
          return baseObject.GetComponent<Base>().DepositResource(amount);

     }
     
     public void PathFinding(Vector3 destination)
     {
          //PathFinding Algorithm
          //Using NavMeshAgent


          agent.SetDestination(destination);

     }

    
}
