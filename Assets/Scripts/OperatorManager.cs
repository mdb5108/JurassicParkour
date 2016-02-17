using UnityEngine;
using System.Collections;

public class OperatorManager : MonoBehaviour {


	public float cellWidth, cellLength;
	public float ditanceFromPlayer;
	public Camera opCamera;
	public bool placingObstacle;
	public GameObject selectedObstacle;
	public Vector3 placingPoint;
	public GameObject[] obstacleList;

	Transform Player;
	// Use this for initialization

	void Start () {

		Player = GameObject.FindGameObjectWithTag("Player").transform;
		opCamera = GameObject.FindGameObjectWithTag("OperatorCamera").GetComponent<Camera>();
	
	}
	
	// Update is called once per frame
	void Update ()
	{

		if(Input.GetMouseButton(0))
		{
			if(Input.GetKeyDown(KeyCode.LeftShift))
			{
				// ONLY FOR TESTING, CHANGE TO UI INPUT
				selectedObstacle = obstacleList[0];
				selectedObstacle.layer = LayerMask.NameToLayer("IgnorePlayer");
				placingObstacle = true;
			}

			if(placingObstacle)
			{
				setObstacle();
			}
		}

		if(Input.GetMouseButtonUp(0))
		{
			if(selectedObstacle != null && placingObstacle)
			{
				if(!collidingWithOtherObjects() && Vector3.Distance(placingPoint,Player.position) > ditanceFromPlayer )
				{
					placeObstacle();
				}
				else
				{
					resetObstacle();
				}
			}
		}

			
	}


	void setObstacle()
	{
		Ray ray = opCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits  = Physics.RaycastAll(ray, 500);
		Debug.DrawRay(ray.origin, ray.direction * 500, Color.green);

		foreach(RaycastHit hit in hits)
		{
			if(hit.collider.name == "Floor")
			{
//				Debug.Log(Mathf.Floor(hit.transform.position.x / cellWidth));

				
				float xPos = Mathf.Floor(hit.point.x / cellWidth) * cellWidth;
				float zPos = Mathf.Floor(hit.point.z / cellLength) * cellLength;
				placingPoint = new Vector3(xPos,0,zPos);

				selectedObstacle.transform.position = placingPoint;

				if(Vector3.Distance(placingPoint,Player.position) > ditanceFromPlayer && !collidingWithOtherObjects())
				{
					foreach(MeshRenderer t in selectedObstacle.GetComponentsInChildren<MeshRenderer>())
					{
						t.material.color = Color.green;
					}
				}
				else
				{
					foreach(MeshRenderer t in selectedObstacle.GetComponentsInChildren<MeshRenderer>())
					{
						t.material.color = Color.red;
					}
				}


			}
		}
	}

	void placeObstacle()
	{
//		if(selectedObstacle != null && placingObstacle)
//			{

		selectedObstacle.transform.position = placingPoint;
//		selectedObstacle.layer = LayerMask.NameToLayer("Default");
		foreach(Transform t in selectedObstacle.transform)
		{
			t.gameObject.layer = LayerMask.NameToLayer("Default");
			t.GetComponent<MeshRenderer>().material.color = Color.white;
		}

		selectedObstacle.tag = "Untagged";
		placingObstacle = false;
		selectedObstacle = null;
		placingPoint = Vector3.zero;

//			}
	}

	void resetObstacle()
	{
//		if(selectedObstacle != null && placingObstacle)
//		{

		selectedObstacle.transform.position = 200 * Vector3.up;
		placingObstacle = false;
		placingPoint = Vector3.zero;
		foreach(MeshRenderer t in selectedObstacle.GetComponentsInChildren<MeshRenderer>())
		{
			t.material.color = Color.white;
		}
		selectedObstacle = null;

//		}
	}



	public Collider[] cols;
	bool collidingWithOtherObjects()
	{
		cols = Physics.OverlapBox(selectedObstacle.transform.position,.49f * selectedObstacle.transform.localScale, Quaternion.identity);
		foreach(Collider col in cols)
		{
			if(col.gameObject.tag != "Floor" && col.gameObject.tag != "Obstacle")
			{
				return true;
			}
		}


		return false;
	}
}