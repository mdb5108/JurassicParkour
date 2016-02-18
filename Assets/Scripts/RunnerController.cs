using UnityEngine;
using System.Collections;

using UnityStandardAssets.CrossPlatformInput;

public class RunnerController : MonoBehaviour
{
    private float radiusOfInteraction;
    private readonly static float INTERACTION_LENGTH = 5f;
    // Use this for initialization
    void Start ()
    {
        radiusOfInteraction = GetComponent<Collider>().bounds.extents.x;
    }

    // Update is called once per frame
    void Update ()
    {
        HandleObstacleInteraction();
    }

    void HandleObstacleInteraction()
    {
        bool jumpUp   = CrossPlatformInputManager.GetAxis("JUMP_UP") > .5;
        bool jumpDown = CrossPlatformInputManager.GetAxis("JUMP_DOWN") > .5;

        if(jumpUp || jumpDown)
        {
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, radiusOfInteraction, transform.forward, INTERACTION_LENGTH);
            for(int i = 0; i < hits.Length; i++)
            {
                if(hits[i].collider.tag == "Obstacle")
                {
                    Debug.Log(hits[i].collider.gameObject);
                    break;
                }
            }
        }
    }
}
