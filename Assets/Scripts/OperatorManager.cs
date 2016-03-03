using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OperatorManager : MonoBehaviour {

    
	public float cellWidth, cellLength;
	public float ditanceFromPlayer;
	public Camera opCamera;
	public bool placingObstacle;
	public Obstacles selectedObstacle;
	public Vector3 placingPoint;
	public Obstacles[] OperatorHand;
    public float[] coolDownDurations, coolDownCounters;
    public Image[] cardMasks;
    public bool[] allowMove;

	Transform Player;
    //changes by shreyas
    public delegate void num_key_del(int key_pressed);
    public static event num_key_del num_key_pressed;
    //
    // Use this for initialization

    void Start () {

		Player = GameObject.FindGameObjectWithTag("Player").transform;
		opCamera = GameObject.FindGameObjectWithTag("OperatorCamera").GetComponent<Camera>();

        for (int i = 0; i < allowMove.Length - 1; i++)
        {
            allowMove[i] = true;
            coolDownCounters[i] = coolDownDurations[i];
        }

	}
	
	// Update is called once per frame
	void Update ()
	{
        updateCooldown();
        checkNumberKey();

		
		if(placingObstacle)
		{
			setObstacle();
		}
		

		if(Input.GetMouseButtonUp(0))
		{
			if(selectedObstacle != null && placingObstacle)
			{
                if (selectedObstacle.tag == "ImpossibleWall")
                {
                    if ((placingPoint.z - selectedObstacle.get_obst_len_wid().y / 2) - Player.position.z > 2)
                    {
                        placeObstacle();
                    }
                    else
                    {
                        resetObstacle();
                    }
                }
                else
                {
                    if (!collidingWithOtherObjects() && Vector3.Distance(placingPoint, Player.position) > ditanceFromPlayer)
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

        
	}


    void updateCooldown()
    {
        for (int i = 0; i < coolDownDurations.Length; i++)
        {
            if (coolDownCounters[i] < coolDownDurations[i])
            {
                coolDownCounters[i] += Time.deltaTime;
                cardMasks[i].fillAmount = 1 - (coolDownCounters[i] / coolDownDurations[i]);
                allowMove[i] = false;
            }
            else
            {
                allowMove[i] = true;
            }
        }
    }


    public int currentKey;

    void checkNumberKey()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetKeyDown(KeyCode.Alpha4) ||
            Input.GetKeyDown(KeyCode.Alpha5) ||
            Input.GetKeyDown(KeyCode.Alpha6))
        {
            string keyPreseed = Input.inputString;
            int keyNum = System.Convert.ToInt32(keyPreseed);


            // KEYNUM NO OF CARDS IN HAND
            if (keyNum >= 1 && keyNum <= 6)
            {
                if (allowMove[keyNum - 1])
                {

                    if (selectedObstacle != null)
                    {
                        Destroy(selectedObstacle.gameObject);
                    }

                    selectedObstacle = GameObject.Instantiate(OperatorHand[keyNum - 1], Vector3.one, OperatorHand[keyNum - 1].transform.localRotation) as Obstacles;
                    foreach(Transform t in selectedObstacle.GetComponentsInChildren<Transform>())
                    {
                        t.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
                    }
                    foreach (Collider col in selectedObstacle.GetComponentsInChildren<Collider>())
                    {
                        col.enabled = false;
                    }

                    placingObstacle = true;
                    currentKey = keyNum;
                }
              

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
				
				float xPos = 0;
				float zPos = Mathf.Floor(hit.point.z / cellLength) * cellLength;
				placingPoint = new Vector3(xPos,0,zPos);

				selectedObstacle.transform.position = placingPoint;


                if (selectedObstacle.tag == "ImpossibleWall")
                {
                    if ((placingPoint.z - selectedObstacle.get_obst_len_wid().y / 2) - Player.position.z  > 2)
                    {
                        foreach (MeshRenderer t in selectedObstacle.GetComponentsInChildren<MeshRenderer>())
                        {
                            t.material.color = Color.green;
                        }
                    }
                    else
                    {
                        foreach (MeshRenderer t in selectedObstacle.GetComponentsInChildren<MeshRenderer>())
                        {
                            t.material.color = Color.red;
                        }
                    }
                }
                else
                {
                    if (Vector3.Distance(placingPoint, Player.position) > ditanceFromPlayer && !collidingWithOtherObjects())
                    {
                        foreach (MeshRenderer t in selectedObstacle.GetComponentsInChildren<MeshRenderer>())
                        {
                            t.material.color = Color.green;
                        }
                    }
                    else
                    {
                        foreach (MeshRenderer t in selectedObstacle.GetComponentsInChildren<MeshRenderer>())
                        {
                            t.material.color = Color.red;
                        }
                    }
                }
                
			}
		}
	}

   

	void placeObstacle()
	{
		selectedObstacle.transform.position = placingPoint;
        TurnObstacleNormal(selectedObstacle.gameObject);
		foreach(Transform t in selectedObstacle.GetComponentsInChildren<Transform>())
		{
            TurnObstacleNormal(t.gameObject);
		}

        foreach (Collider col in selectedObstacle.GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }

        selectedObstacle.tag = "Obstacle";
	

        if (Random.Range(0, 2) == 0)
        {
            selectedObstacle.transform.localScale = new Vector3(1, 0, 1);
            placingObstacle = false;
            StartCoroutine(applyGrowingAnimation(selectedObstacle.gameObject));
            selectedObstacle = null;

        }
        else
        {
            selectedObstacle.transform.position += (Vector3.up * 10);
            placingObstacle = false;
            StartCoroutine(applyFallingAnimation(selectedObstacle.gameObject, placingPoint));
            selectedObstacle = null;
        }


        allowMove[currentKey - 1] = false;
        coolDownCounters[currentKey - 1] = 0;


        placingPoint = Vector3.zero;
        //begin changes by shreyas
        if (num_key_pressed != null)
        {
            num_key_pressed(999);  //passing invalid value to clear the highlighting 
        }
        //end changes
	}

    void TurnObstacleNormal(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("Default");

        if (go.tag.Contains("ImpossibleWall"))
        {
            if (go.tag == "ImpossibleWallPL")
            {
                go.layer = LayerMask.NameToLayer("IgnoreOperator");
            }
            else if (go.tag == "ImpossibleWallOP")
            {
                go.layer = LayerMask.NameToLayer("IgnorePlayer");
            }
        }
               
        MeshRenderer m = go.GetComponent<MeshRenderer>();

        if (m != null)
        {                            
            if (m.tag == "ImpossibleWallOP")
            {
                m.material.color = new Color(0, 0, 0, .5f);
            }
            else
            {
                m.material.color = Color.white;
            }
             
        }
        
    }

    IEnumerator applyFallingAnimation(GameObject g, Vector3 destinationPoint)
    {

        while (g.transform.position.y > destinationPoint.y)
        {
            g.transform.position -= (Vector3.up * 20f * Time.deltaTime);
            yield return 0;
        }

        g.transform.position = destinationPoint;
        g.GetComponent<Obstacles>().PlacedObstacle();
        GetComponent<SoundBank>().PlaySound("ObstacleLand");
       
    }

     IEnumerator applyGrowingAnimation(GameObject g)
    {
      
        while (g.transform.localScale.y < 1)
        {
            g.transform.localScale += (Vector3.up * 1f * Time.deltaTime);
            yield return 0;

        }

        
        g.transform.localScale = Vector3.one;
        g.GetComponent<Obstacles>().PlacedObstacle();
       
    }

    void resetObstacle()
	{
		selectedObstacle.transform.position = 200 * Vector3.up;
        Destroy(selectedObstacle.gameObject);
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
        int obstacleLayer = LayerMask.NameToLayer("Default");
        int operatorCullLayer = LayerMask.NameToLayer("IgnoreOperator");
        cols = Physics.OverlapBox(new Vector3(selectedObstacle.transform.position.x, 2.5f, selectedObstacle.transform.position.z), obstacleBounds *.49f, Quaternion.identity, (1 << obstacleLayer) | (1 << operatorCullLayer));

        foreach (Collider col in cols)
        {
            if (col.gameObject.tag != "Floor" && !col.gameObject.tag.Contains("ImpossibleWall"))
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
