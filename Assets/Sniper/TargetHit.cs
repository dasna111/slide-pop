using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHit : MonoBehaviour
{
    public static int score;

    void OnTriggerEnter2D(Collider2D CC2D)
    {
        if (CC2D.gameObject.tag == "Bullet")
        {
            Debug.Log("TargetDestoyed!!!!!");
            Destroy(gameObject);
            score++;
        }
    }
}
