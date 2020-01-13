using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMoves : MonoBehaviour
{
    private Rigidbody2D rb2d;        //Store a reference to the Rigidbody2D component required to use 2D Physics.
    private BoxCollider2D bc2d;
    public GameObject DukeGame;
    public GameObject Level;
    public float timeLeft;
    public Text Timer;
    private object Won;
    public Spawner2 MainGameScript;
    public Text TimerLabel;
    public float CountDown = 5f;
    public AudioSource DukeMiniGame;
    public AudioSource TriggerLightning;

    // Use this for initialization
    void Start()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
        StartTimer();
    }

    private void StartTimer()
    {
        CountDown -= Time.fixedDeltaTime;
        DukeMiniGame.Play();
        TriggerLightning.Play();
        TimerLabel.text = CountDown.ToString();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        //Store the current horizontal input in the float moveHorizontal.
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal, 0);
        rb2d.MovePosition(movement);
        timeLeft -= Time.deltaTime;

        Timer.text = "Time Left:" + Mathf.Round(timeLeft);
        if (timeLeft < 0)
        {
            DukeGameWin();
        }
    }

    public void DukeGameWin()
    {
        DukeGame.SetActive(false);
        Won = true;
        Spawner2 Ceiling = MainGameScript.GetComponent<Spawner2>();
        Ceiling.MiniGame(Won);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cars"))
        { 
            DukeGameOver();
        }
    }
    public void DukeGameOver()
    {
        DukeGame.SetActive(false);
        Level.SetActive(true);
    }
}
