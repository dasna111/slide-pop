using UnityEngine;
using UnityEngine.UI;

public class Scope : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float Power;
    public float down;
    public float maxup;
    public float maxdown;
    public int ammo;
    public GameObject Bullet;
    public GameObject Game;
    public GameObject Level;
    public Spawner MainGameScript;
    private float timer = 0;
    public int targetCount;
    private object Won;
    public Text TimerLabel;
    public float CountDown = 5f;
    public AudioSource SniperMiniGame;
    public AudioSource TriggerLightning;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        StartTimer();
    }

    private void StartTimer()
    {
        CountDown -= Time.fixedDeltaTime;
        SniperMiniGame.Play();
        TriggerLightning.Play();
        TimerLabel.text = CountDown.ToString();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = rb2d.position;
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && ammo > 0)
        {
            Instantiate(Bullet,pos, Quaternion.identity);
            rb2d.AddForce(transform.up * Power);
            ammo--;
        }
        if (TargetHit.score > targetCount-1)
            GameWin();
        timer += Time.deltaTime;
        if (timer > 5 || ammo <= 0)
            GameOver();
        


        rb2d.AddForce(transform.up * -down);

        if (rb2d.velocity.y < 0f)
        {
            rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxdown);
        }
        else
        {
            rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxup);
        }
    }

    private void GameOver()
    {
        Game.SetActive(false);
        Level.SetActive(true);
    }
    public void GameWin()
    {
        Game.SetActive(false);
        Spawner Ceiling = MainGameScript.GetComponent<Spawner>();
        Won = true;
        Ceiling.MiniGame(Won);
    }
}
