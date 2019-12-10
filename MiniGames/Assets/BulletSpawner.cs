using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public float speedMult = 1.0f;
    public float rangeMult = 1.0f;
    // Use this for initialization
    public GameObject bullet;
    public float shootInterval = 1.0f;
    float basex = 0.0f;
    float shootTimeAc = 0.0f;
    // Update is called once per frame
    void Start()
    {
        basex = transform.position.x;
    }
    void Update()
    {
        Vector3 position = transform.position;
        float interval = Mathf.Sin(Time.time * (speedMult / rangeMult)) * rangeMult;
        bool shoot = false;
        if (Time.deltaTime + shootTimeAc > shootInterval)
        {
            shootTimeAc = 0.0f;
            shoot = true;
        }

        else
            shootTimeAc += Time.deltaTime;

        position.x = basex + interval;

        transform.position = position;
        if (shoot)
            Instantiate(bullet, transform.position, bullet.transform.rotation);
    }
}
