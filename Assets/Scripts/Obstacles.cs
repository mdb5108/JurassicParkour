using UnityEngine;

using System;
using System.Collections;

public class Obstacles : MonoBehaviour {

    public enum actions
    {
        VAULT = 1,
        LEAP,
        WALL_RUN_SIDE,
        WALL_RUN_UP,
        SLIDE
    };

    public enum way_of_interaction
    {
        PASS_UP = 1,
        PASS_DOWN
    }

    [System.Serializable]
    public struct StartAxis
    {
        public Vector3 origin;
        public float axisNormal;
        public float distanceFromCenter;
        public bool mirrored;
        public bool lockOrientation;
        public float lockedOrientation;
    }


    [SerializeField]
    private string animation_trigger;
    [SerializeField]
    private actions[] allowed_actions;
    [SerializeField]
    private float angle_approach;
    [SerializeField]
    private float angle_threshold;
    [SerializeField]
    private way_of_interaction possible_interaction;
    [SerializeField]
    private Vector2 obst_len_wid;
    [SerializeField]
    private StartAxis start_axis;

    [System.Serializable]
    public struct Fatigue
    {        
        public float arms;
        public float core;
        public float legs;
    }

    [SerializeField]
    private Fatigue obst_end;


    // Use this for initialization
    void Start()
    {


	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
      DrawAxisOfInteraction();
#endif
	}

  void DrawAxisOfInteraction()
  {
      var player = GameObject.FindWithTag("Player");
      Vector3 axisOffset = get_start_axis_offset(player.transform.position);
      Vector3 lineOfAxis = Vector3.Cross(axisOffset, Vector3.up).normalized;
      Vector3 pointOnAxis = get_start_axis_point_on_line(player.transform.position);
      float lineLength = 10;
      Debug.DrawLine(pointOnAxis-(lineLength/2)*lineOfAxis, pointOnAxis + lineLength*lineOfAxis, Color.red);
  }

   
   public Fatigue get_fatigue()
   {
        return obst_end;
   }

  public bool CheckApproachAngle(Vector3 position, Vector3 playerForward)
  {
      Vector3 direction = position - transform.position;
      Vector3 approachDir = get_angle_approach();

      if(Vector3.Dot(direction, transform.forward) > 0)
      {
          Vector3 axisOfReflection = Vector3.Cross(transform.forward, Vector3.up);
          approachDir = Vector3.Reflect(-approachDir, axisOfReflection);
      }

      float angle = Vector3.Angle(playerForward, approachDir);
      return angle <= get_angle_threshold();
  }

  public string get_animation_trigger()
  {
    return animation_trigger;
  }

    public actions[] get_allowed_actions()
    {
        return allowed_actions;
    }

    public Vector3 get_angle_approach()
    {
        return transform.TransformDirection(Quaternion.AngleAxis(angle_approach, Vector3.up)*Vector3.forward);
    }

    public float get_angle_threshold()
    {
        return angle_threshold;
    }

    public way_of_interaction get_possible_interaction()
    {
        return possible_interaction;
    }

    public Vector2 get_obst_len_wid()
    {
        return obst_len_wid;
    }

    public Vector3 get_start_axis_offset(Vector3 position)
    {
        Vector3 diff = position - (transform.position+transform.TransformDirection(start_axis.origin));
        Vector3 axisNormal = transform.TransformDirection(start_axis.distanceFromCenter*(Quaternion.AngleAxis(start_axis.axisNormal, Vector3.up)*Vector3.forward));
        if(Vector3.Dot(axisNormal, diff) < 0)
        {
            axisNormal = -axisNormal;
        }
        return axisNormal;
    }

    public Vector3 get_start_axis_point_on_line(Vector3 position)
    {
        return get_start_axis_offset(position) + transform.TransformPoint(start_axis.origin);
    }

    public bool get_to_lock_orientation()
    {
        return start_axis.lockOrientation;
    }

    public Vector3 get_orientation_facing(Vector3 position)
    {
        Vector3 diff = position - transform.position;
        Vector3 facing = transform.TransformDirection(Quaternion.AngleAxis(start_axis.lockedOrientation, Vector3.up)*Vector3.forward);
        if(Vector3.Dot(diff, transform.forward) < 0)
        {
            facing = -facing;
        }
        return facing;
    }
}
