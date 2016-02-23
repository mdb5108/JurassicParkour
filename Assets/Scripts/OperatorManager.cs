using UnityEngine;
using System.Collections;

public class OperatorManager : MonoBehaviour {


	public float cellWidth, cellLength;
	public float ditanceFromPlayer;
	public Camera opCamera;
	public bool placingObstacle;
	public Obstacles selectedObstacle;
	public Vector3 placingPoint;
	public Obstacles[] OperatorHand;

	Transform Player;
    //changes by shreyas
    public delegate void num_key_del(int key_pressed);
    public static event num_key_del num_key_pressed;
    //
    // Use this for initialization

    void Start () {

		Player = GameObject.FindGameObjectWithTag("Player").transform;
		opCamera = GameObject.FindGameObjectWithTag("OperatorCamera").GetComponent<Camera>();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        checkNumberKey();

		
		if(placingObstacle)
		{
			setObstacle();
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

			//DEBUG

        
	}


    void checkNumberKey()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetKeyDown(KeyCode.Alpha4) ||
            Input.GetKeyDown(KeyCode.Alpha5) )
        {
            string keyPreseed = Input.inputString;
            int keyNum = System.Convert.ToInt32(keyPreseed);


            // KEYNUM NO OF CARDS IN HAND
            if (keyNum >= 1 && keyNum <= 5)
            {
                if (selectedObstacle)
                {
                    Destroy(selectedObstacle.gameObject);
                }
                selectedObstacle = GameObject.Instantiate(OperatorHand[keyNum - 1], Vector3.one, Quaternion.identity) as Obstacles;
                selectedObstacle.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
                selectedObstacle.GetComponentInChildren<Collider>().enabled = false;
                placingObstacle = true;

                // code changes by Shreyas

                if (num_key_pressed != null)
                {
                    num_key_pressed(keyNum);
                }

                //end of changes       
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
			if(hit.collider.tag == "Floor")
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
        TurnObstacleNormal(selectedObstacle.gameObject);
		foreach(Transform t in selectedObstacle.transform)
		{
            TurnObstacleNormal(t.gameObject);
		}
        selectedObstacle.GetComponentInChildren<Collider>().enabled = true;
        selectedObstacle.tag = "Obstacle";
		placingObstacle = false;
		selectedObstacle = null;
		placingPoint = Vector3.zero;
        //begin changes by shreyas
        if (num_key_pressed != null)
        {
            num_key_pressed(999);  //passing invalid value to clear the highlighting 
        }
        //end changes
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

   public Vector3 obstacleBounds;

    public Collider[] cols;
	bool collidingWithOtherObjects()
	{
        obstacleBounds = new Vector3(selectedObstacle.get_obst_len_wid().x, 5, selectedObstacle.get_obst_len_wid().y);
        cols = Physics.OverlapBox(new Vector3(selectedObstacle.transform.position.x, 2.5f, selectedObstacle.transform.position.z), obstacleBounds *.49f, Quaternion.identity);

        foreach (Collider col in cols)
        {
            if (col.gameObject.tag != "Floor" && col.gameObject.tag != "InnerObstacle")
            {
                return true;
            }
        }
         
    	return false;
	}
    // UNCOMMENT IF DEBUGGING OBSTACLE SIZE
    //void OnDrawGizmos()
    //{
    //    if (selectedObstacle && placingObstacle)
    //    {
    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawCube(new Vector3(selectedObstacle.transform.position.x, 2.5f, selectedObstacle.transform.position.z), obstacleBounds);
    //    }
       
    //}

}
