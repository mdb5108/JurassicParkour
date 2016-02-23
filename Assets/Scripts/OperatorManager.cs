using UnityEngine;
using System.Collections;

public class OperatorManager : MonoBehaviour {


	public float cellWidth, cellLength;
	public float ditanceFromPlayer;
	public Camera opCamera;
	public bool placingObstacle;
	public GameObject selectedObstacle;
	public Vector3 placingPoint;
	public GameObject[] OperatorHand;

	Transform Player;
	// Use this for initialization

	void Start () {

		Player = GameObject.FindGameObjectWithTag("Player").transform;
		opCamera = GameObject.FindGameObjectWithTag("OperatorCamera").GetComponent<Camera>();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        checkNumberKey();

		if(Input.GetMouseButtonUp(0))
		{
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


    void checkNumberKey()
    {
        if (Input.anyKeyDown)
        {
            string keyPreseed = Input.inputString;
            int keyNum = int.Parse(keyPreseed);
            // KEYNUM NO OF CARDS IN HAND
            if (keyNum >= 1 && keyNum <= 5)
            {
                if (selectedObstacle)
                {
                    Destroy(selectedObstacle);
                }
                selectedObstacle = GameObject.Instantiate(OperatorHand[keyNum - 1], Vector3.one, Quaternion.identity) as GameObject;
                selectedObstacle.layer = LayerMask.NameToLayer("IgnorePlayer");
                selectedObstacle.GetComponentInChildren<Collider>().enabled = false;
                placingObstacle = true;
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

    void TurnObstacleNormal(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("Default");
        MeshRenderer m = go.GetComponent<MeshRenderer>();

        if(m != null)
            m.material.color = Color.white;
    }

	void placeObstacle()
	{

		selectedObstacle.transform.position = placingPoint;
        TurnObstacleNormal(selectedObstacle);
		foreach(Transform t in selectedObstacle.transform)
		{
            TurnObstacleNormal(t.gameObject);
		}
        selectedObstacle.GetComponentInChildren<Collider>().enabled = true;
        selectedObstacle.tag = "Untagged";
		placingObstacle = false;
		selectedObstacle = null;
		placingPoint = Vector3.zero;

	}

	void resetObstacle()
	{

		selectedObstacle.transform.position = 200 * Vector3.up;
		placingObstacle = false;
		placingPoint = Vector3.zero;
		foreach(MeshRenderer t in selectedObstacle.GetComponentsInChildren<MeshRenderer>())
		{
			t.material.color = Color.white;
		}
		selectedObstacle = null;

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
