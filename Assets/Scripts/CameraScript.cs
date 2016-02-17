using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {


	public Transform Player;    // The target we are following
	public Transform Clipper; // Empty GO
	public float mapWidth;
    public float zDistance;      // The distance from the target along its Z axis
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
		mapWidth = GameObject.Find("Floor").transform.localScale.x * 10;
		Clipper.transform.position -= (Vector3.forward * mapWidth/2 + Vector3.forward);
		walls = GameObject.FindGameObjectsWithTag("Wall");
		transform.LookAt(Player.position);
//		initalDirection = (Player.position - this.transform.position).normalized;
    }

    // LateUpdate is called once per frame
    void LateUpdate ()
	{
		checkForWalls();

		if(Input.GetMouseButton(1))
		{
			transform.RotateAround(Player.position, Vector3.up, rotationSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
			Clipper.transform.RotateAround(Player.position, Vector3.up, rotationSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));

		}

		// UNCOMMENT IF NOT PLAYER'S CHILD----------------------------------------//

//		Vector3 targetPosition = Player.position - (Player.forward * zDistance) + (Player.up * height);
//		transform.position = Vector3.MoveTowards(transform.position, targetPosition, positionDamping * Time.deltaTime);

		// ---------------------------------//

//        Quaternion targetRotation = Quaternion.LookRotation(target.position-transform.position, target.up);
//        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);


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
        zDistance = 0;
        height = 2;
        positionDamping = 50;
        rotationDamping = 0;
        rotationSpeed = 100;
    }

}
