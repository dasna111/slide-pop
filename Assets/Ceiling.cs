using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ceiling : MonoBehaviour
{
    private Vector3 Ceiling1 = new Vector3(-3, 10, 0);
    private Vector3 Ceiling2 = new Vector3(6.32f, 10, 0);
    public GameObject AbbyCeiling;
    public GameObject DukeCeiling;
    public GameObject NatalieCeiling;
    public GameObject RyanCeiling;
    public GameObject FlintCeiling;
    public GameObject IrisCeiling;
    public bool selected = false;

    private void Start()
    {
        Assign(Player1, Player2); 
    }
     
    private void Assign(Player1, Player2)
    {
        if (Player1 == null)
        {
            if (name == "Abby")
            {
                Instantiate(AbbyCeiling, Ceiling1, Quaternion.identity);
            }
            if (name == "Duke")
            {
                Instantiate(DukeCeiling, Ceiling1, Quaternion.identity);
            }
            if (name == "Natalie")
            {
                Instantiate(NatalieCeiling, Ceiling1, Quaternion.identity);
            }
            if (name == "Ryan")
            {
                Instantiate(RyanCeiling, Ceiling1, Quaternion.identity);
            }
            if (name == "Flint")
            {
                Instantiate(FlintCeiling, Ceiling1, Quaternion.identity);
            }
            if (name == "Iris")
            {
                Instantiate(IrisCeiling, Ceiling1, Quaternion.identity);
            }
        }
        if (Player2 == null)
        {
            if (name == "Abby")
            {
                Instantiate(AbbyCeiling, Ceiling2, Quaternion.identity);
            }
            if (name == "Duke")
            {
                Instantiate(DukeCeiling, Ceiling2, Quaternion.identity);
            }
            if (name == "Natalie")
            {
                Instantiate(NatalieCeiling, Ceiling2, Quaternion.identity);
            }
            if (name == "Ryan")
            {
                Instantiate(RyanCeiling, Ceiling2, Quaternion.identity);
            }
            if (name == "Flint")
            {
                Instantiate(FlintCeiling, Ceiling2, Quaternion.identity);
            }
            if (name == "Iris")
            {
                Instantiate(IrisCeiling, Ceiling2, Quaternion.identity);
            }
        }
    }
}
