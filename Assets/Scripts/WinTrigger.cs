using UnityEngine;
using System.Collections;

public class WinTrigger : MonoBehaviour {

    public GameObject GameWinText;

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
          
            GameWinText.SetActive(true);
            StartCoroutine(winFunc()); 
        }
    }

    IEnumerator winFunc()
    {

        while(Camera.main.rect.height < 1)
        {
            Camera.main.rect = new Rect(0, 0, 1, (Camera.main.rect.height + .01f));
            yield return null;
        }

        Camera.main.rect = new Rect(0, 0, 1, 1);
        
    }

}
