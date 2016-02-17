using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {


	public Transform target;    // The target we are following
    public float zDistance;      // The distance from the target along its Z axis
    public float height;        // the height we want the camera to be above the target
    public float positionDamping;   // how quickly we should get to the target position
    public float rotationDamping;   // how quickly we should get to the target rotation
   	public float rotationSpeed;


    void Awake()
    {
    	target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // LateUpdate is called once per frame
    void LateUpdate ()
	{

		if(Input.GetMouseButton(1))
		{
			transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
		}


      	Vector3 targetPosition = target.position + (target.up * height) - (target.forward * zDistance);
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, positionDamping * Time.deltaTime);

//        Quaternion targetRotation = Quaternion.LookRotation(target.position-transform.position, target.up);
//        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);


    }

	// Use this for public variable initialization
    public void Reset() 
    {
        zDistance = 3;
        height = 1;
        positionDamping = 6;
        rotationDamping = 60;
    }

}
