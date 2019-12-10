using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public Text TextScore;
    private int Score;
    //CircleCollider2D CC2D;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D CC2D)
    {
 
        if (CC2D.gameObject.tag == "Target")
        {
            Debug.Log("Bullet hit Target");
            Score++;
            TextScore.text = "Score: " + Score;
            Destroy(gameObject);

        }
    }
}
