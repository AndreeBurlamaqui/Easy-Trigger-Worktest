using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 5;
    [SerializeField] float bulletLife = 5;

    private void Start()
    {
        Destroy(gameObject, bulletLife);
    }

    void Update()
    {
        transform.position += transform.right * bulletSpeed * Time.deltaTime;
    }
}
