using UnityEngine;
using System.Collections;

public class PitScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Floor")
        {
            col.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
