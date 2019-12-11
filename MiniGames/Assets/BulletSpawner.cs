using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public int numberOfProjectiles;
    public float projectileSpeed;
    public GameObject ProjectilePrefab;
    private Vector2 startPoint;
    private float timer;
    public float ShoutEvery;

    public float RotateBy;
    public float RotateEvery;

    private float zAngle;
    private float STimer;
    private float Rotated;
    private const float radius = 1F;

    private void Update()
    {
        startPoint = transform.position;
        SpawnProjectile(numberOfProjectiles);
    }

    private void SpawnProjectile(int _numberOfProjectiles)
    {
        float angleStep = (360 - Rotated) / _numberOfProjectiles;
        float angle = 0;
        STimer += Time.deltaTime;
        if (STimer > ShoutEvery)
        {
            for (int i = 0; i <= _numberOfProjectiles - 1; i++)
            {
                float projectileDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                float projectileDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

                Vector2 projectileVector = new Vector2(projectileDirXPosition, projectileDirYPosition);
                Vector2 projectileMoveDirection = (projectileVector - startPoint).normalized * projectileSpeed;

                GameObject tmpObj = Instantiate(ProjectilePrefab, startPoint, Quaternion.identity);
                tmpObj.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);
                Rotate();
                angle = angle + angleStep + Rotated;
            }
            STimer = 0;
        }
    }

    private void Rotate()
    {
        timer += Time.deltaTime;
        if (timer > RotateEvery)
        {
            Rotated += RotateBy;
            timer = 0;
        }
    }
}
