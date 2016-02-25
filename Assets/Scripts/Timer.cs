using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {
    public float time_in_secs;
    // Use this for initialization
    void Start () {
        //Camera cam = transform.GetComponent<Camera>();
        //cam.camera = Camera.main;
        Canvas canv = transform.GetComponent<Canvas>();
        canv.worldCamera = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
        Transform timer = transform.FindChild("Timer");
        Text text_comp = timer.GetComponent<Text>();
        string time_text;

        if (time_in_secs <= 0.0f)
        {
            if (Input.GetKeyDown("r"))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0); 
                
            }
            else
            {
                Time.timeScale = 0;
                time_text = "Game Over:\n Press R to restart";
                text_comp.text = time_text;
                return;
            }
        }
        time_in_secs -= Time.deltaTime;
       
        int time_in_mins = (int)time_in_secs / 60;
        int time_in_secs_int = (int)time_in_secs - time_in_mins * 60;
        
        if (time_in_secs_int >= 10)
            if (time_in_mins >= 10)
                time_text = time_in_mins.ToString() + ":" + time_in_secs_int.ToString();
            else
                time_text = "0" + time_in_mins.ToString() + ":" + time_in_secs_int.ToString();
        else
            if (time_in_mins >= 10)
            time_text = time_in_mins.ToString() + ":0" + time_in_secs_int.ToString();
        else
            time_text = "0" + time_in_mins.ToString() + ":0" + time_in_secs_int.ToString();

        text_comp.text = time_text;
    }
}
