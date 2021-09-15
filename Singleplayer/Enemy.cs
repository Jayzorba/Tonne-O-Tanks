using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public AudioClip shootingAudio;
    public GameObject shootingVFX;
    public GameObject bullet;
    public Transform shootPos;
    public Slider HealthBar;
    public float fireRate = 0.7f;
    public float nextShot;

    private int health = 100;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.LookAt(other.transform);
            Shoot();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            var bullet = collision.gameObject.GetComponent<Bullets>();
            TakeDamage(bullet.damage);
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        HealthBar.value = health;
        if (health <= 0)
        {
            EnemyDied();
        }
    }

    private void EnemyDied()
    {
        OnEnemyKilled?.Invoke();
        gameObject.SetActive(false);
    }
    private void Shoot()
    {
        if (Time.time > nextShot)
        {
            nextShot = Time.time + fireRate;
            var bulletShot = Instantiate(bullet, shootPos.position, Quaternion.identity);
            bulletShot.GetComponent<Bullets>()?.InitializeBullet(transform.rotation * Vector3.forward);

            AudioManager.Instance.Play3D(shootingAudio, shootPos.position, 0.1f);
            VFXManager.Instance.Play(shootingVFX, shootPos.position);
        }
    }
}
