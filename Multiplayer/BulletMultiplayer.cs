using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMultiplayer : MonoBehaviour
{
    public AudioClip bulletHitAudio;
    public GameObject bulletHitVFX;
    public float bulletSpeed;
    Rigidbody rb;
    public Photon.Realtime.Player Owner;
    public int damage = 10;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void InitializeBullet(Vector3 originalDirection, Photon.Realtime.Player player)
    {
        transform.forward = originalDirection;
        rb.velocity = transform.forward * bulletSpeed;
        Owner = player;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(bulletHitAudio, transform.position, 0.1f);
        VFXManager.Instance.Play(bulletHitVFX, transform.position);
        Destroy(gameObject);
    }
}
