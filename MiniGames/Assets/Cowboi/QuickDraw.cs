﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickDraw : MonoBehaviour
{
    //Spawn this object
    public GameObject spawnObject;
    public GameObject CowBoi;

    public float maxTime = 5;
    public float minTime = 2;
    private float ReactTime;
    public float MaxReactTime;

    //current time
    private float time;

    //The time to spawn the object
    private float spawnTime;

    void Start()
    {
        SetRandomTime();
        time = minTime;
    }

    void FixedUpdate()
    {

        //Counts up
        time += Time.deltaTime;

        //Check if its the right time to spawn the object
        if (time >= spawnTime)
        {
            SpawnObject();
            SetRandomTime();
        }

    }


    //Spawns the object and resets the time
    void SpawnObject()
    {
        time = 0;
        Instantiate(spawnObject, transform.position, spawnObject.transform.rotation);
        ReactTime += Time.deltaTime;
        if (Input.anyKey && ReactTime < MaxReactTime)
        {
            CowBoiGameWin();
        }
        if (ReactTime > MaxReactTime)
            CowBoiGameOver();
    }

    //Sets the random time between minTime and maxTime
    void SetRandomTime()
    {
        spawnTime = Random.Range(minTime, maxTime);
    }
    private void CowBoiGameOver()
    {
        CowBoi.SetActive(false);
        //TODO reference to main script
    }
    private void CowBoiGameWin()
    {
        CowBoi.SetActive(false);
        //TODO reference to main script
    }
}
