using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public AudioClip bulletHitAudio;
    public GameObject bulletHitVFX;
    public float bulletSpeed;
    Rigidbody rb;
    public int damage = 10;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        damage = Random.Range(20, 25);
    }

    public void InitializeBullet(Vector3 originalDirection)
    {
        transform.forward = originalDirection;
        rb.velocity = transform.forward * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(bulletHitAudio, transform.position, 0.1f);
        VFXManager.Instance.Play(bulletHitVFX,transform.position);
        Destroy(gameObject);
    }
}
