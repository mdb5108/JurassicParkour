using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class KillZoneScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {

            col.GetComponent<Rigidbody>().useGravity = false;
            col.GetComponent<Rigidbody>().isKinematic = true;

            Invoke("restartLevel", 2f);

            //Make Ragdoll
        }
        else if (col.tag == "Floor")
        {
            Destroy(col.gameObject);
        }
    }

    void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
