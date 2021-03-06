﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GridScript : MonoBehaviour {

    public bool createGridNow, deleteGridNow;
    public int gridWidth, gridLength;
    public GameObject[,] currentGrid;
    public GameObject templateCube;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (deleteGridNow)
        {
            deleteGridNow = false;
            deleteGrid();
        }
        if (createGridNow)
        {
            createGridNow = false;
            createGrid();
        }

    }

    void createGrid()
    {
        deleteGrid();
        currentGrid = new GameObject[gridWidth, gridLength];

        for (int j = 0; j < gridLength; j++)
        {
            for (int i = 0; i < gridWidth; i++)
             {
                Vector3 tempPosition = new Vector3(this.transform.position.x + ( i - gridWidth/2 ), -1, this.transform.position.z + j);
                GameObject tempCube = GameObject.Instantiate(templateCube, tempPosition, Quaternion.identity) as GameObject;
                tempCube.name = "Grid " + i + "," + j;
                tempCube.transform.parent = this.transform;
                currentGrid[i, j] = tempCube;
            }
        }


    }


    void deleteGrid()
    {
        Transform[] children = this.GetComponentsInChildren<Transform>();
        foreach (Transform t in children)
        {
            if(t != this.transform)
             DestroyImmediate(t.gameObject);
        }
        currentGrid = null;
        
       
    }

}
