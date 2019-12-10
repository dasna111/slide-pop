﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : MonoBehaviour
{
    private Animation anim;
    public int CW;

    void Update()
    { 
        anim = gameObject.GetComponent<Animation>();
        for (int i = 0; i < CW; i++)
        {
            int selectedValue = (int)Choose(1, 2, 3);
            if (selectedValue == 1)
                Haduken();
            if (selectedValue == 2)
                HuricaneKick();
            if (selectedValue == 1)
                DragonPunch();
        }
    }
    public object Choose(object a, object b, params object[] p)
    {
        int random = Random.Range(0, p.Length + 2);
        if (random == 0) return a;
        if (random == 1) return b;
        return p[random - 2];
    }
    void Haduken ()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
             if (Input.GetKeyDown(KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.RightArrow))
             {
                 if (Input.GetKeyDown(KeyCode.RightArrow))
                 {
                   anim.Play("Haduken");
                 }
             }
        }
    }
        
     void  HuricaneKick()
     {
         if (Input.GetKeyDown(KeyCode.DownArrow))
             {
             if (Input.GetKeyDown(KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.LeftArrow))
             {
                 if (Input.GetKeyDown(KeyCode.LeftArrow))
                 {
                    anim.Play("HuricaneKick");
                 }
             }
         }
     }
        
       void DragonPunch ()
       {
         if (Input.GetKeyDown(KeyCode.DownArrow))
         {
                if (Input.GetKeyDown(KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.DownArrow))
                {
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            anim.Play("DragonPunch");
                        }
                }
         }
       }
       
    
}