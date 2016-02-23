using UnityEngine;
using System.Collections;

public class HandOfCards : MonoBehaviour {
    
    private int card_count;
	// Use this for initialization
	void Start () {
        card_count = transform.childCount - 1;
        Debug.Log("child count is");
        Debug.Log(card_count);

        Transform pivot = transform.FindChild("Pivot");        

        for (int i = 0; i < card_count; i++)
        {
            Transform child = transform.GetChild(i);
            child.RotateAround(pivot.position,Vector3.back,-60 + 120 /card_count * i);
            Debug.Log(child.name);
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
