using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HandOfCards : MonoBehaviour {

    public float popup_distance;
    public float time_in_secs; 
    private int card_count;
    private Vector2 first_rect_position;
    
	// Use this for initialization

    void OnEnable()
    {
        OperatorManager.num_key_pressed += change_color;
    }

    void OnDisable()
    {
        OperatorManager.num_key_pressed -= change_color;
    }

    void Start () {

        foreach (Transform child in transform)
        {
            if (child.tag == "Card")         
                card_count++;           
        }

        Debug.Log("child count is");
        Debug.Log(card_count);

        Transform first_child;
        first_child = transform.GetChild(0);        
        first_rect_position = first_child.GetComponent<RectTransform>().anchoredPosition;
        
    }
	    

	// Update is called once per frame
	void Update () {
        time_in_secs -= Time.deltaTime;
        Transform timer = transform.FindChild("Timer");
        Text text_comp = timer.GetComponent<Text>();
        int time_in_mins = (int)time_in_secs / 60;
        int time_in_secs_int = (int)time_in_secs - time_in_mins * 60 ;
        string time_text;
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

    void change_color(int key_pressed)
    {
        Transform child;
        CanvasRenderer canvas;
        RectTransform rect;
        
        for (int i = 0; i < card_count; i++)
        {
            child = transform.GetChild(i);
            canvas = child.GetComponent<CanvasRenderer>();
            canvas.SetColor(Color.white);
            rect = child.GetComponent<RectTransform>();
            if (rect.anchoredPosition.y > first_rect_position.y)
            {                
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, first_rect_position.y);             
            }
        }
        if (key_pressed >= 1 && key_pressed <= card_count)
        {
            child = transform.GetChild(key_pressed - 1);
            rect = child.GetComponent<RectTransform>();            
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, first_rect_position.y + popup_distance);          
            canvas = child.GetComponent<CanvasRenderer>();
            Color light_green = new Color();
            ColorUtility.TryParseHtmlString("#49CC7BFF", out light_green);
            canvas.SetColor(light_green);
        }
    }
}
