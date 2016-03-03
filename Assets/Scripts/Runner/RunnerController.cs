using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;

public class RunnerController : MonoBehaviour
{
    private float radiusOfInteraction;
    private readonly static float INTERACTION_LENGTH = 4f;

    private ThirdPersonCharacter thirdCharacter;
    private ThirdPersonUserControl userControl;
    public Animator targetAnimator;
    public Transform animationRoot;
    private Vector3 animationRootOrigin;
    private Quaternion animationRootOriginRot;

    private Rigidbody characterRigidbody;
    private Collider characterCollider;

    private bool movingToVault;
    private Vector3 moveToVaultPosition;

    private Obstacles.Fatigue temp_fatigue;
    [SerializeField]
    private Obstacles.Fatigue fatigue_pool;
    private Obstacles.Fatigue fatigue_pool_start;

    // Use this for initialization
    void Start ()
    {
        targetAnimator.GetBehaviour<StateSoundBehaviour>().soundBank = targetAnimator.GetComponent<SoundBank>();

        radiusOfInteraction = GetComponent<Collider>().bounds.extents.x;
        thirdCharacter = GetComponent<ThirdPersonCharacter>();
        userControl = GetComponent<ThirdPersonUserControl>();
        animationRootOrigin = animationRoot.localPosition;
        animationRootOriginRot = animationRoot.localRotation;
        characterCollider = GetComponent<Collider>();
        characterRigidbody = GetComponent<Rigidbody>();
        fatigue_pool_start = fatigue_pool;


        Transform child = transform.FindChild("Mesh").transform.FindChild("anubis").transform.FindChild("polySurface63");
        SkinnedMeshRenderer skin = transform.FindChild("Mesh").transform.FindChild("anubis").transform.FindChild("polySurface63").GetComponent<SkinnedMeshRenderer>();
        Debug.Log(skin);

        //Material mat  = Resources.Load("Assets/Models/Character/Materials/greenarms", typeof(Material)) as Material; 
        Material mat1 = Resources.Load<Material>("mainbody");
        Material mat2 = Resources.Load<Material>("greencore");
        Material mat3 = Resources.Load<Material>("redarms");
        Material[] mats = { mat1, mat2, mat3 };
        //Debug.Log(mat);

        // skin.sharedMaterials[1] = mat;

        //skin.material = mat;
        skin.sharedMaterials = mats;
        //skin.sharedMaterial = mats[0];


    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void HandleObstacleInteraction(bool jumpUp, bool jumpDown)
    {
        if((jumpUp || jumpDown) && thirdCharacter.GetGrounded())
        {
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, radiusOfInteraction, transform.forward, INTERACTION_LENGTH);
            for(int i = 0; i < hits.Length; i++)
            {
                if(hits[i].collider.tag == "ImpossibleWallPL")
                {
                    break; //If impassible wall was hit first, any likely obstacle is on the other side
                }
                else if(hits[i].collider.tag == "Obstacle")
                {
                    Obstacles obstacle = hits[i].transform.GetComponent<Obstacles>();
                    var interactionType = obstacle.get_possible_interaction();
                    bool matching = ((interactionType == Obstacles.way_of_interaction.PASS_UP)   && jumpUp) ||
                                    ((interactionType == Obstacles.way_of_interaction.PASS_DOWN) && jumpDown);

                    if(matching
                        && obstacle.CheckApproachAngle(transform.position, transform.forward))
                    {
                        //Find nearest point on axis
                        Vector3 axisOfInteraction = obstacle.get_start_axis_offset(transform.position);
                        Vector3 pointOnAxis = obstacle.get_start_axis_point_on_line(transform.position);
                        Vector3 closestPointDelta = Vector3.Project(pointOnAxis - transform.position, axisOfInteraction.normalized);

                        //If the delta is pointing opposite the axis offset we can still
                        //run up
                        if(Vector3.Dot(closestPointDelta, axisOfInteraction) < 0)
                        {
                            userControl.EnableInput(false);
                            StartCoroutine(MoveToVault(transform.position + closestPointDelta, axisOfInteraction, obstacle));
                            break;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator MoveToVault(Vector3 position, Vector3 axisNormal, Obstacles obs)
    {
        moveToVaultPosition = position;
        movingToVault = true;
        while(transform.position != position && Vector3.Dot((position - transform.position), axisNormal) < 0)
        {
            yield return new WaitForFixedUpdate();
        }
        movingToVault = false;
        StartAnimation(obs);
    }

    private void FixedUpdate()
    {
        if(movingToVault)
        {
            Vector3 flatened = moveToVaultPosition - transform.position;
            flatened = flatened+transform.forward;
            flatened.y = flatened.z;
            thirdCharacter.Move(flatened, false, false);
        }
    }

    private void StartAnimation(Obstacles obs)
    {
        if(obs.get_to_lock_orientation())
        {
            transform.forward = obs.get_orientation_facing(transform.position);
        }

        string trigger = obs.get_animation_trigger();
        characterRigidbody.isKinematic = true;
        characterCollider.enabled = false;
        targetAnimator.SetTrigger(trigger);
        
        //start of code changes by Shreyas
        temp_fatigue =  obs.get_fatigue();
    }

    public void AnimationFinished()
    {
        transform.position = animationRoot.position - animationRootOrigin;
        animationRoot.localPosition = animationRootOrigin;
        animationRoot.localRotation = animationRootOriginRot;
        userControl.EnableInput(true);
        characterRigidbody.isKinematic = false;
        characterCollider.enabled = true;

        //start of code changes by Shreyas        
        ColorBody();
        

    }

    public void ColorBody()
    {
        fatigue_pool.arms -= temp_fatigue.arms;
        fatigue_pool.core -= temp_fatigue.core;
        fatigue_pool.legs -= temp_fatigue.legs;


        if (fatigue_pool.arms <= 20 / 100 * fatigue_pool_start.arms)
        {
            SetBodyColor("arms","red");
        }
        else if (fatigue_pool.arms <= 50 / 100 * fatigue_pool_start.arms)
        {
            SetBodyColor("arms", "yellow");
        }


        if (fatigue_pool.core <= 20 / 100 * fatigue_pool_start.core)
        {
            SetBodyColor("core", "red");
        }
        else if(fatigue_pool.core <= 50 / 100 * fatigue_pool_start.core)
        {
            SetBodyColor("core", "yellow");
        }


        if (fatigue_pool.legs <= 20 / 100 * fatigue_pool_start.legs)
        {
            SetBodyColor("legs", "red");
        }
        else if(fatigue_pool.legs <= 50 / 100 * fatigue_pool_start.legs)
        {
            SetBodyColor("legs", "yellow");
        }
        

    }   

    void SetBodyColor(string i_body_part,string i_color)
    {
        switch (i_body_part)
        {
            case "core":
                {
                    switch (i_color)
                    {
                        case "yellow":
                            MeshRenderer MeshRend =  transform.FindChild("polySurface63").GetComponent<MeshRenderer>();
                            Debug.Log(MeshRend);
                            break;

                        case "red":
                            break;

                        default:
                            break;
                    }
                }
                break;

            case "arms":
                {
                    switch (i_color)
                    {
                        case "yellow":
                            break;

                        case "red":
                            break;

                        default:
                            break;
                    }
                }
                break;

            case "legs":
                {
                    switch (i_color)
                    {
                        case "yellow":
                            break;

                        case "red":
                            break;

                        default:
                            break;
                    }
                }
                break;

            default:
                break;
        }

    }

}
