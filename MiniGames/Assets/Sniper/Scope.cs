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

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = rb2d.position;
        if (Input.GetKeyDown("space") && ammo > 0)
        {
            Instantiate(Bullet,pos, Quaternion.identity);
            rb2d.AddForce(transform.up * Power);
            ammo--;
        }

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
}
