using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Vampire : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            Destroy(gameObject);
        }
    }
}
