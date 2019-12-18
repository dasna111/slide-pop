using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public GameObject Cars;
    public Vector3 center;
    public Vector3 size;
    private float time = 0;
    public float SpawnEvery;


    // We marked this as "Fixed"Update because we
    // are using it to mess with physics.
    void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time > SpawnEvery)
        {
            SpawnObject();
            time = time - 1;
        }
        

    }
    public object Choose(object a, object b, params object[] p)
    {
        int random = Random.Range(0, p.Length + 2);
        if (random == 0) return a;
        if (random == 1) return b;
        return p[random - 2];
    }
    public void SpawnObject()
    {
        Vector3 pos = new Vector3 ((int)Choose(-1, 0, 1), 6);

        Instantiate(Cars, pos, Quaternion.identity);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
}
