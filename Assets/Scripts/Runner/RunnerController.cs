using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

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

    [SerializeField]
    private Obstacles.Fatigue fatigue_pool;
    private Obstacles.Fatigue fatigue_pool_start;

    private Material[] mats = new Material[10];
    private SkinnedMeshRenderer skin;
    //[SerializeField]
    //private float green_precentage;
    [SerializeField]
    private float yellow_precentage;
    [SerializeField]
    private float red_precentage;
    [SerializeField]
    private float rejuvination_time;
    private float start_rejuvination_time;
    [SerializeField]
    private float flash_time;
    private float start_flash_time;
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

        start_rejuvination_time = rejuvination_time;
        start_flash_time = flash_time;
        fatigue_pool_start = fatigue_pool;
        mats[0] = Resources.Load<Material>("mainbody");
        mats[1] = Resources.Load<Material>("greenarms");
        mats[2] = Resources.Load<Material>("greencore");
        mats[3] = Resources.Load<Material>("greenlegs");
        mats[4] = Resources.Load<Material>("yellowarms");
        mats[5] = Resources.Load<Material>("yellowcore");
        mats[6] = Resources.Load<Material>("yellowlegs");
        mats[7] = Resources.Load<Material>("redarms");
        mats[8] = Resources.Load<Material>("redcore");
        mats[9] = Resources.Load<Material>("redlegs");
        skin = transform.FindChild("Mesh").transform.FindChild("anubis").transform.FindChild("polySurface63").GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update ()
    {
        Material[] existing_skin = skin.sharedMaterials;
        fatigue_pool.arms += rejuvination_time * Time.deltaTime;
        fatigue_pool.core += rejuvination_time * Time.deltaTime;
        fatigue_pool.legs += rejuvination_time * Time.deltaTime;


        fatigue_pool.arms = Mathf.Min(fatigue_pool_start.arms, fatigue_pool.arms);
        fatigue_pool.core = Mathf.Min(fatigue_pool_start.core, fatigue_pool.core);
        fatigue_pool.legs = Mathf.Min(fatigue_pool_start.legs, fatigue_pool.legs);


        ColorBody();
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
                    break; //If impassible wall was hit first, any likely obstacle after is on the other side
                }
                else if(hits[i].collider.tag == "Obstacle")
                {
                    Obstacles obstacle = hits[i].transform.GetComponent<Obstacles>();
                    //start of code changes by Shreyas
                    Obstacles.Fatigue obs_fatigue = obstacle.get_fatigue();
                    Material[] existing_skin = skin.sharedMaterials;                    
                    if (   obs_fatigue.arms > 0 && existing_skin[1] == mats[7]
                        || obs_fatigue.core > 0 && existing_skin[2] == mats[8]
                        || obs_fatigue.legs > 0 && existing_skin[3] == mats[9])
                    {
                        break;//don't start animation if body part red and obstacle is taxing
                    }


                    if (!obstacle.get_blocked())
                    {
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

        if (obs.get_to_lock_orientation())
        {
            transform.forward = obs.get_orientation_facing(transform.position);
        }

        Obstacles.Fatigue obs_fatigue = obs.get_fatigue();
        fatigue_pool.arms -= obs_fatigue.arms;
        fatigue_pool.core -= obs_fatigue.core;
        fatigue_pool.legs -= obs_fatigue.legs;
        ColorBody();

        string trigger = obs.get_animation_trigger();
        characterRigidbody.isKinematic = true;
        characterCollider.enabled = false;
        targetAnimator.SetTrigger(trigger);
        

    }

    public void AnimationFinished()
    {
        transform.position = animationRoot.position - animationRootOrigin;
        animationRoot.localPosition = animationRootOrigin;
        animationRoot.localRotation = animationRootOriginRot;
        userControl.EnableInput(true);
        characterRigidbody.isKinematic = false;
        characterCollider.enabled = true;

    }

    public void ColorBody()
    {
        Material[] existing_skin = skin.sharedMaterials;
         //reset body
         if (fatigue_pool.arms >= fatigue_pool_start.arms && fatigue_pool.legs >= fatigue_pool_start.legs && fatigue_pool.core >= fatigue_pool_start.core)
         {
             existing_skin[1] = mats[0];
             existing_skin[2] = mats[0];
             existing_skin[3] = mats[0];            
             skin.sharedMaterials = existing_skin;
         }

        if (fatigue_pool.arms <= red_precentage / 100 * fatigue_pool_start.arms)
        {
            SetBodyColor("arms","red",existing_skin);
        }
        else if (fatigue_pool.arms <= yellow_precentage / 100 * fatigue_pool_start.arms)
        {
            SetBodyColor("arms", "yellow",existing_skin);
        }
        else
        {
            existing_skin[1] = mats[0];
        }


        if (fatigue_pool.core <= red_precentage / 100 * fatigue_pool_start.core)
        {
            SetBodyColor("core", "red",existing_skin);
        }
        else if(fatigue_pool.core <= yellow_precentage / 100 * fatigue_pool_start.core)
        {            
            SetBodyColor("core", "yellow",existing_skin);
        }
        else
        {
            existing_skin[2] = mats[0];
        }


        if (fatigue_pool.legs <= red_precentage / 100 * fatigue_pool_start.legs)
        {
            SetBodyColor("legs", "red",existing_skin);
        }
        else if(fatigue_pool.legs <= yellow_precentage / 100 * fatigue_pool_start.legs)
        {
            SetBodyColor("legs", "yellow",existing_skin);
        }
        else
        {
            existing_skin[3] = mats[0];
        }

        skin.sharedMaterials = existing_skin;

    }   

    void SetBodyColor(string i_body_part,string i_color, Material[] existing_skin)
    {
       
        switch (i_body_part)
        {
            case "core":
                {
                    switch (i_color)
                    {
                        case "yellow":          
                            existing_skin[2] = mats[5];
                            break;

                        case "red":
                            existing_skin[2] = mats[8];
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
                            existing_skin[1] = mats[4];
                            break;

                        case "red":
                            existing_skin[1] = mats[7];
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
                            existing_skin[3] = mats[6];
                            break;

                        case "red":
                            existing_skin[3] = mats[9];
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
