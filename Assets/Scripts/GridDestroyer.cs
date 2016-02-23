using UnityEngine;
using System.Collections;

public class GridDestroyer : MonoBehaviour {

    public float speed;
    public bool moveAhead;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (moveAhead)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, this.transform.position + Vector3.forward, speed * Time.deltaTime);
        }
	
	}

    void OnTriggerEnter(Collider any)
    {
        if (any.tag == "Floor")
        {
            any.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

}
