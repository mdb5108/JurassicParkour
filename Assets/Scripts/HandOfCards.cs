using UnityEngine;
using System.Collections;

public class HandOfCards : MonoBehaviour {
    
    private int card_count;
    private RectTransform first_rect;
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
        card_count = transform.childCount;
        Debug.Log("child count is");
        Debug.Log(card_count);

        Transform child;
        child = transform.GetChild(0);
        //Debug.Log(child.name);
        first_rect = child.GetComponent<RectTransform>();
        
        //Debug.Log(first_rect.TransformPoint(first_rect.rect.position));
    }
	    

	// Update is called once per frame
	void Update () {
	
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

            Debug.Log(rect.TransformPoint(rect.position));
            //Debug.Log(first_rect.position);
            if (rect.position.y > first_rect.position.y)
            {
                //rect.Translate(0,-2,0);
                //Debug.Log(child.name); 
                //child.Translate(0, -5, 0,Space.Self);
            }
        }
        if (key_pressed >= 1 && key_pressed <= card_count)
        {
            child = transform.GetChild(key_pressed - 1);
            rect = child.GetComponent<RectTransform>();
            //rect.Translate(0, 2, 0);
            canvas = child.GetComponent<CanvasRenderer>();
            Color light_green = new Color();
            ColorUtility.TryParseHtmlString("#49CC7BFF", out light_green);
            canvas.SetColor(light_green);
        }
    }
}
