using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.AI;
public class Unit : MonoBehaviour
{    
     private float health;

     private Mission mission;
     
     /// <summary>
     /// Unit's current status
     /// 0: No Job, 1:Gathering, 2: Fighter, 3:Sentinel 
     /// </summary>
     private int job;
     /// <summary>
     /// Unit's faction,should be set by the Base when spwan
     /// 0: Blue, 1: Red
     /// </summary>
     public int faction;
     private float holdingCapacity;
     private float gatheredAmount;
     private bool isCarryingResource;

     /// <summary>
     /// The idea is give unit a basic attack range, and base on the different job,
     /// there will be a multiplier to adjust it
     /// </summary>
     private float attackRange;

     /// <summary>
     /// The idea is give unit a basic observation range, and base on the different job,
     /// there will be a multiplier to adjust it
     /// </summary>
     private float observationRange;

     /// <summary>
     /// Base that the unit belongs to, and when unit spwan, it should be set by the Base
     /// Note: No need to set it by BasicStatInit().
     /// </summary>
     public GameObject Base;
     public NavMeshAgent agent;

     public GameObject WEM;

     void BasicStatInit()
     {
          mission = new Mission();
          mission.MissionInit();
          health = 100f;
          job = 3;
          holdingCapacity = 3f;
          gatheredAmount = 0f;
          isCarryingResource = false;
          attackRange = 3f;
          observationRange = 5f;
          agent = GetComponent<NavMeshAgent>();

          WEM = GameObject.Find("WorldEventManager");

          
     }
     void Awake()
     {
          BasicStatInit();
          Debug.Log(Base.name);
     } 

     /// <summary>
     /// Base on the job, unit will do different things
     /// </summary>
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

               case 2:
                    break;

               case 3:
                    Sentinel();
                    break;
               default:
                    break;
          }
     }

     
     /// <summary>
     /// Go to the Base
     /// </summary>
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
     private void Gatherer()
     {

          if(isCarryingResource)
          {
               if(mission.isHaveMission) // --End Mission Trigger--
               {
                    if (Base.GetComponent<Base>().IsUnitInRange(transform.position))
                    {
                         //Deposit Resource
                         bool isDeposited = RequestDepositResource(Base, gatheredAmount);
                         if (isDeposited)
                         {
                              gatheredAmount = 0;
                              isCarryingResource = false;
                              mission.MissionEnd();
                         }
                    }
               }
               else // --Start Mission: Go to Base--
               {
                    mission.MissionStart_Location(Base.GetComponent<Base>().transform.position, "Go to Base");
                    PathFinding(mission.missionLocation);
               }
          }
          else //if not carrying resource
          {
               if(mission.isHaveMission) // --End Mission Trigger--
               {
                    if (mission.missionGameObject.GetComponent<Resource>().IsUnitInRange(transform.position))
                    {
                         //Gather Resource
                         isCarryingResource = ResquestGatherResource(mission.missionGameObject, holdingCapacity);
                         mission.MissionEnd();
                         Debug.Log("requesting to gather resource");
                    }
               }
               else // --Start Mission: Request Resource Location from Base,then go to the resource--
               {
                    GameObject resource = Base.GetComponent<Base>().SearchBestResourceLocation();

                    // if there is no resource available,request to change job

                    if (resource == null)
                    {
                         
                         UnitChangeJob(Base.GetComponent<Base>().ConsiderBestJob());
                         return;
                    }

                    mission.MissionStart_GameObject(resource, "Go to Resource");
                    PathFinding(mission.missionGameObject.transform.position);
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

     /// <summary>
     /// Sentinel Job
     /// Basic Idea:
     /// Patroling around the Territory, if a enemy unit is in
     /// </summary>
     private void Sentinel()
     {
          float attackRangeMultiplier = 1f;
          float observationRangeMultiplier = 1.5f;

          GameObject detectedEnemy = DetectEnemy(observationRange * observationRangeMultiplier);
          
          if(detectedEnemy != null)
          {
               mission.MissionOverride(detectedEnemy, "Disperse Enemy");
               Destroy(GameObject.Find("Hilight_Point(Clone)"));

               float distance = Vector3.Distance(detectedEnemy.transform.position, transform.position);

               if(distance <= attackRange * attackRangeMultiplier)
               {
                    //Attack the enemy
                    Attacking(detectedEnemy, 10f);
               }
               else
               {
                    //try to chase the enemy
                    Chasing(detectedEnemy);
               }
          }
          else
          {
               if(mission.isHaveMission && mission.missionType == "Disperse Enemy")
               {
                    mission.MissionEnd();
               }

               if(mission.isHaveMission) // --End Mission Trigger--
               {
                    if (Distance2DCalulater(transform.position, mission.missionLocation) <= 0.01f)
                    {
                         mission.MissionEnd();
                         Destroy(GameObject.Find("Hilight_Point(Clone)"));
                    }
               }
               else // --Start Mission: Patroling--
               {
                    //Note: Mission Start in Patrolling() function
                    Patrolling();
               }
               
          }
     }


     /// <summary>
     /// Unit will calulate the direction to all enemy units which in Teritory, and return the closest one
     /// </summary>
     /// <param name="ObsRange"></param>
     /// <returns></returns>
     private GameObject DetectEnemy(float ObsRange)
     {
          GameObject closestEnemy = null;
          float closestDistance = float.MaxValue;

          foreach (var unit in WEM.GetComponent<WEM>().GetAllEnemyUnit(faction))
          {
               float distance = Vector3.Distance(transform.position, unit.transform.position);

               if (distance < closestDistance && 
               Vector3.Distance(unit.transform.position, Base.transform.position) <= Base.GetComponent<Base>().RequestTeritoryRadius() &&
               distance <= ObsRange)     
               {
                    closestDistance = distance;
                    closestEnemy = unit;
               }
          }

          return closestEnemy;
     }

     /// <summary>
     /// When chasing, unit will speed up(by 1.5), and then go to the enemy unit
     /// </summary>
     /// <param name="enemy"></param>
     private void Chasing(GameObject enemy)
     {
          agent.speed = agent.speed * 1.5f;
          PathFinding(enemy.transform.position);
          agent.speed = agent.speed / 1.5f;
     }

     /// <summary>
     /// When patroling, unit will move around the territory,
     /// it will be given a random destination to move to
     /// </summary>
     private void Patrolling()
     {
          float patrolRadius = 5f; // Adjust the patrol radius as needed

          Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
          Vector3 patrolDestination = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);

          NavMeshHit hit;
          if (NavMesh.SamplePosition(patrolDestination, out hit, 1f, NavMesh.AllAreas))
          {
               mission.MissionStart_Location(hit.position, "Patrolling to given location");
               PathFinding(hit.position);
               HilightDestination(hit.position);
          }
     }

     /// <summary>
     /// Attack the enemy unit,and send the attack request to the enemy unit
     /// Enemy unit will react on the request
     /// </summary>
     /// <param name="enemy"></param>
     /// <param name="attackDamage"></param>
     public void Attacking(GameObject enemy, float attackDamage)
     {
          bool isAttackSuccess = enemy.GetComponent<Unit>().ReceiveAttackRequest(attackDamage);
          if(isAttackSuccess)
          {
               Debug.Log("Attack Success");
               //Attack Success
          }
          else
          {
               Debug.Log("Attack Fail");
               //Attack Fail
          }
     }

     /// <summary>
     /// Determine whether the attack is success or not,and react on the result
     /// </summary>
     /// <param name="attackDamage"></param>
     /// <returns></returns>
     public bool ReceiveAttackRequest(float attackDamage)
     {
          health -= attackDamage;
          if(health <= 0)
          {
               //Unit is dead
               Destroy(gameObject);
          }
          return true;
     }

     public void PathFinding(Vector3 destination)
     {
          //PathFinding Algorithm
          //Using NavMeshAgent
          agent.SetDestination(destination);

     }

     public void HilightDestination(Vector3 destination)
     {
          GameObject pole = Instantiate(Resources.Load("Hilight_Point"), destination, Quaternion.identity) as GameObject;
          
     }

     public float Distance2DCalulater(Vector3 point1, Vector3 point2)
     {
          return Mathf.Sqrt(Mathf.Pow(point1.x - point2.x, 2) + Mathf.Pow(point1.z - point2.z, 2));
     }
     private class Mission
     {
          public bool isHaveMission;
          public Vector3 missionLocation;

          public GameObject missionGameObject;

          public string missionType;

          public void MissionInit()
          {
               isHaveMission = false;
               missionLocation = new Vector3(0, 0, 0);
               missionGameObject = new GameObject();
               missionType = "Currently no mission";
          }

          public void MissionStart_Location(Vector3 location,string type = "Something about Location")
          {
               isHaveMission = true;
               missionLocation = location;
               missionType = type;
          }

          public void MissionStart_GameObject(GameObject gameObject, string type = "Something about GameObject")
          {
               isHaveMission = true;
               missionGameObject = gameObject;
               missionType = type;
          }

          public void MissionEnd()
          {
               MissionInit();
          }

          public void MissionOverride(GameObject gameObject, string type = "Mission Override")
          {
               MissionInit();
               isHaveMission = true;
               missionGameObject = gameObject;
               missionType = type;
          }

          public void MissionOverride(Vector3 location, string type = "Mission Override")
          {
               MissionInit();
               isHaveMission = true;
               missionLocation = location;
               missionType = type;
          }
     }
}


