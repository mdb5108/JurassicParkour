using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;

public class RunnerController : MonoBehaviour
{
    private float radiusOfInteraction;
    private readonly static float INTERACTION_LENGTH = 5f;

    private ThirdPersonUserControl userControl;
    private Animation legacyAnim;
    private Transform meshRoot;

    // Use this for initialization
    void Start ()
    {
        radiusOfInteraction = GetComponent<Collider>().bounds.extents.x;
        userControl = GetComponent<ThirdPersonUserControl>();
        legacyAnim = GetComponent<Animation>();
        meshRoot = transform.Find("Mesh");
        Assert.IsTrue( meshRoot != null );
    }

    // Update is called once per frame
    void Update ()
    {
    }

    public void HandleObstacleInteraction(bool jumpUp, bool jumpDown)
    {
        if(jumpUp || jumpDown)
        {
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, radiusOfInteraction, transform.forward, INTERACTION_LENGTH);
            for(int i = 0; i < hits.Length; i++)
            {
                if(hits[i].collider.tag == "Obstacle")
                {
                    Animation anim = hits[i].transform.GetComponent<Animation>();
                    userControl.EnableInput(false);
                    legacyAnim.clip = anim.clip;
                    legacyAnim.AddClip(anim.clip, anim.clip.name);
                    legacyAnim.Play();
                    break;
                }
            }
        }
    }

    public void AnimationFinished()
    {
      transform.position = meshRoot.position;
      meshRoot.position = transform.TransformPoint(Vector3.zero);
      legacyAnim.RemoveClip(legacyAnim.clip);
      userControl.EnableInput(true);
    }
}
