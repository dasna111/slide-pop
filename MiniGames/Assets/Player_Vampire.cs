using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Vampire : MonoBehaviour
{
    //movement speed in units per second
    public float movementSpeed = 5f;
    public GameObject VampireGame;
    private float timer = 0;

    void Update()
    {
        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        //update the position
        transform.position = transform.position + new Vector3(horizontalInput * movementSpeed * Time.deltaTime, verticalInput * movementSpeed * Time.deltaTime, 0);
        timer += Time.deltaTime;
        if (timer >= 5)
            VampireGameWin();

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            VampireGameOver();
        }
    }

    private void VampireGameOver()
    {
        VampireGame.SetActive(false);
        //TODO reference to main script
    }
    private void VampireGameWin()
    {
        VampireGame.SetActive(false);
        //TODO reference to main script
    }
}
