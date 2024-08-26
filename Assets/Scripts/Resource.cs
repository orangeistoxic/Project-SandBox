using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private float operationRange = 3.7f;
    private float resourceAmount = 1000f;
    
    public float GatherResource(float amount)
    {
        if (resourceAmount >= amount)
        {
            resourceAmount -= amount;
            return amount;
        }
        else if(resourceAmount > 0)
        {
            float temp = resourceAmount;
            resourceAmount = 0;
            return temp;
        }
        else
        {
            return 0;
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
