using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {


	public Transform Player;    // The target we are following
	public float mapWidth;
    public float zDistance;
    public float distance;      // The distance from the target along its Z axis
    public float height;        // the height we want the camera to be above the target
    public float positionDamping;   // how quickly we should get to the target position
    public float rotationDamping;   // how quickly we should get to the target rotation
   	public float rotationSpeed;
   	public GameObject[] walls;


//   	Vector3 initalDirection;

    void Awake()
    {
        if(Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        mapWidth = 20;
		walls = GameObject.FindGameObjectsWithTag("Wall");
		transform.LookAt(Player.position);
    }
    // LateUpdate is called once per frame
    void LateUpdate ()
	{
		checkForWalls();

        //ROTATING DISABLED
        //if (Input.GetMouseButton(1))
        //{
        //    transform.RotateAround(Player.position, Vector3.up, rotationSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
        //}
        
        Vector3 targetPosition = Player.position - (transform.forward * distance) + (transform.up * height) + (Vector3.forward * zDistance);
        transform.position = Vector3.Lerp(transform.position, targetPosition, positionDamping * Time.deltaTime);


    }



	void checkForWalls()
	{
		Debug.DrawRay(transform.position, (Player.position - transform.position).normalized * Vector3.Distance(Player.position, transform.position), Color.red);

		RaycastHit hit;

		if(Physics.Raycast(transform.position, (Player.position - transform.position).normalized, out hit, Vector3.Distance(Player.position, transform.position)))
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
