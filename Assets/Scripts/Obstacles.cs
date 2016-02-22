﻿using UnityEngine;
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


    // Use this for initialization
    void Start()
    {


	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public actions[] get_allowed_actions()
    {
        return allowed_actions;
    }

    public Vector3 get_angle_approach()
    {
        return Quaternion.AngleAxis(angle_approach, Vector3.up)*Vector3.forward;
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

}
