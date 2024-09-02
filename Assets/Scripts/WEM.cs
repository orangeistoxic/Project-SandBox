using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// WEM stands for World Event Manager
/// </summary>
public class WEM : MonoBehaviour
{

    private List<GameObject> allRedUnit;
    private List<GameObject> allBlueUnit;
    private List<GameObject> allResources;
    private List<GameObject> allRedBase;
    private List<GameObject> allBlueBase;
    
    /// <summary>
    /// When the game starts, find all GameObjects in the scene, and then classify them
    /// </summary>
    void Start()
    {
        allRedUnit = new List<GameObject>();
        allBlueUnit = new List<GameObject>();
        allResources = new List<GameObject>();
        allRedBase = new List<GameObject>();
        allBlueBase = new List<GameObject>();

        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject gameObject in allGameObjects)
        {
            if (gameObject.tag[0] == 'R')
            {
                if (gameObject.CompareTag("Resource"))  //Special case for Resources
                {
                    // Increase the number of Resources
                    allResources.Add(gameObject);
                }
                else if (gameObject.CompareTag("R_Unit"))
                {
                    // Increase the number of Red Team Units
                    allRedUnit.Add(gameObject);
                }
                else if (gameObject.CompareTag("R_Base"))
                {
                    // Increase the number of Red Team Bases
                    allRedBase.Add(gameObject);
                }
                
            }
            else if (gameObject.tag[0] == 'B')
            {
                if (gameObject.CompareTag("B_Unit"))
                {
                    // Increase the number of Blue Team Units
                    allBlueUnit.Add(gameObject);
                }
                else if (gameObject.CompareTag("B_Base"))
                {
                    // Increase the number of Blue Team Bases
                    allBlueBase.Add(gameObject);
                }
            }

        }
            
        
    }

    /// <summary>
    /// Keep tracking the number of GameObjects int the scene
    /// </summary>
    /// <param name="gameObject"> the gameobject add or remove</param>
    /// <param name="addOrRemove"> 1: Add, 0: Remove</param>
    public void GameObjectsNumChange(GameObject gameObject,bool addOrRemove)
    {
        if (addOrRemove) //Add
        {
            if (gameObject.tag[0] == 'R')
            {
                if (gameObject.CompareTag("Resource"))  //Special case for Resources
                {
                    // Increase the number of Resources
                    allResources.Add(gameObject);
                }
                else if (gameObject.CompareTag("R_Unit"))
                {
                    // Increase the number of Red Team Units
                    allRedUnit.Add(gameObject);
                }
                else if (gameObject.CompareTag("R_Base"))
                {
                    // Increase the number of Red Team Bases
                    allRedBase.Add(gameObject);
                }

            }
            else if (gameObject.tag[0] == 'B')
            {
                if (gameObject.CompareTag("B_Unit"))
                {
                    // Increase the number of Blue Team Units
                    allBlueUnit.Add(gameObject);
                }
                else if (gameObject.CompareTag("B_Base"))
                {
                    // Increase the number of Blue Team Bases
                    allBlueBase.Add(gameObject);
                }
            }
        }
        else  //Remove
        {
            if (gameObject.tag[0] == 'R')
            {
                if (gameObject.CompareTag("Resource"))  //Special case for Resources
                {
                    // Remove this Resource from the list
                    allResources.Remove(gameObject);
                }
                else if (gameObject.CompareTag("R_Unit"))
                {
                    // Remove this Unit from the list
                    allRedUnit.Remove(gameObject);
                }
                else if (gameObject.CompareTag("R_Base"))
                {
                    // Remove this Base from the list
                    allRedBase.Remove(gameObject);
                }

            }
            else if (gameObject.tag[0] == 'B')
            {
                if (gameObject.CompareTag("B_Unit"))
                {
                    // Remove this Unit from the list
                    allBlueUnit.Remove(gameObject);
                }
                else if (gameObject.CompareTag("B_Base"))
                {
                    // Remove this Base from the list
                    allBlueBase.Remove(gameObject);
                }
            }
        }
    }

    public List<GameObject> GetAllRedUnit()
    {
        return allRedUnit;
    }

    public List<GameObject> GetAllBlueUnit()
    {
        return allBlueUnit;
    }

    public List<GameObject> GetAllResources()
    {
        return allResources;
    }

    public List<GameObject> GetAllRedBase()
    {
        return allRedBase;
    }

    public List<GameObject> GetAllBlueBase()
    {
        return allBlueBase;
    }

    public List<GameObject> GetAllEnemyUnit(int faction)
    {
        if (faction == 0) //0: Blue
        {
            return allRedUnit;
        }
        else if (faction == 1) //1: Red
        {
            return allBlueUnit;
        }
        else
        {
            return null;
        }
    }
}
