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

    private Rigidbody characterRigidbody;
    private Collider characterCollider;

    // Use this for initialization
    void Start ()
    {
        radiusOfInteraction = GetComponent<Collider>().bounds.extents.x;
        userControl = GetComponent<ThirdPersonUserControl>();
        legacyAnim = GetComponent<Animation>();
        meshRoot = transform.Find("Mesh");
        Assert.IsTrue( meshRoot != null );
        characterCollider = GetComponent<Collider>();
        characterRigidbody = GetComponent<Rigidbody>();
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
                    Obstacles obstacle = hits[i].transform.GetComponent<Obstacles>();
                    var interactionType = obstacle.get_possible_interaction();
                    bool matching = ((interactionType == Obstacles.way_of_interaction.PASS_UP)   && jumpUp) ||
                                    ((interactionType == Obstacles.way_of_interaction.PASS_DOWN) && jumpDown);
                    if(matching
                        && obstacle.CheckApproachAngle(transform.position, transform.forward))
                    {
                        Animation anim = obstacle.GetComponent<Animation>();
                        userControl.EnableInput(false);
                        characterRigidbody.useGravity = false;
                        characterCollider.enabled = false;
                        legacyAnim.clip = anim.clip;
                        legacyAnim.AddClip(anim.clip, anim.clip.name);
                        legacyAnim.Play();
                        break;
                    }
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
      characterRigidbody.useGravity = true;
      characterCollider.enabled = true;
    }
}
