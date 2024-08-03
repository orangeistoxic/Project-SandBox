using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Unit : MonoBehaviour
{
     new private Rigidbody rigidbody; 
     
     
     void Start()
     {
         rigidbody = GetComponent<Rigidbody>();
         
     }

     void Update()
     {
          Vector3 velocity = Vector3.zero;

          if (Input.GetKey(KeyCode.D))
          {
               velocity.x = 5;
          }
          if (Input.GetKey(KeyCode.A))
          {
               velocity.x = -5;
          }
          if (Input.GetKey(KeyCode.W))
          {
               velocity.z = 5;
          }
          if (Input.GetKey(KeyCode.S))
          {
               velocity.z = -5;
          }

          rigidbody.velocity = velocity;
     }
}
