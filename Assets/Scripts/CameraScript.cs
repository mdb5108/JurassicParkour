﻿using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {


	public Transform Player;    // The target we are following
	public Transform Clipper; // Empty GO
	public float mapWidth;
    public float distance;      // The distance from the target along its Z axis
    public float height;        // the height we want the camera to be above the target
    public float positionDamping;   // how quickly we should get to the target position
    public float rotationDamping;   // how quickly we should get to the target rotation
   	public float rotationSpeed;
   	public GameObject[] walls;


//   	Vector3 initalDirection;

    void Awake()
    {
    	Player = GameObject.FindGameObjectWithTag("Player").transform;
		Clipper = GameObject.FindGameObjectWithTag("Clipper").transform;
        mapWidth = 20;
		Clipper.transform.position -= (Vector3.forward * mapWidth/2 + Vector3.forward);
		walls = GameObject.FindGameObjectsWithTag("Wall");
		transform.LookAt(Player.position);
//		initalDirection = (Player.position - this.transform.position).normalized;
    }
    // LateUpdate is called once per frame
    void LateUpdate ()
	{
		checkForWalls();

        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(Player.position, Vector3.up, rotationSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
            Clipper.transform.RotateAround(Player.position, Vector3.up, rotationSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
        }
        
        Vector3 targetPosition = Player.position - (transform.forward * distance) + (transform.up * height);
        transform.position = Vector3.Lerp(transform.position, targetPosition, positionDamping * Time.deltaTime);


    }



	void checkForWalls()
	{
		Debug.DrawRay(Clipper.transform.position, (Player.position - Clipper.transform.position).normalized * Vector3.Distance(Player.position, Clipper.transform.position), Color.red);

		RaycastHit hit;

		if(Physics.Raycast(Clipper.transform.position, (Player.position - Clipper.transform.position).normalized, out hit, Vector3.Distance(Player.position, Clipper.transform.position)))
		{

		//----------------------- layer hiding

			if(hit.collider.tag == "Wall")
			{
				hit.collider.gameObject.layer = LayerMask.NameToLayer("IgnoreOperator");
					
				foreach(GameObject g in walls)
				{
					if(g != hit.collider.gameObject)
					{
//						Debug.Log(g);
						g.gameObject.layer = LayerMask.NameToLayer("Default"); 	
					}
				}
			}

		}
	}

	// Use this for public variable initialization
    void Reset() 
    {
        distance = 0;
        height = 2;
        positionDamping = 50;
        rotationDamping = 0;
        rotationSpeed = 100;
    }

}
