using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public AudioClip shootingAudio;
    public GameObject shootingVFX;
    Rigidbody rb;
    public float moveSpeed = 5.0f;
    public GameObject bullet;
    public Transform shootPos;
    public Slider HealthBar;
    public GameObject gameOverScreen;
    public float fireRate = 1.2f;
    public float nextShot;

    private int health = 100;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //if(Input.GetMouseButton(0))
        //{
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Hit");
            var bullet = collision.gameObject.GetComponent<Bullets>();
            TakeDamage(bullet.damage);
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "HealthPickup")
        {
            if (health >= 100)
            {
                return;
            }

            HealthPickup();
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        HealthBar.value = health;
        if (health <= 0)
        {
            PlayerDied();
        }
    }

    private void HealthPickup()
    {
        health += 20;
        HealthBar.value = health;
    }

    private void PlayerDied()
    {
        gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverScreen.SetActive(true);
    }

    private void Shoot()
    {
        if(Time.time > nextShot)
        {
            nextShot = Time.time + fireRate;
            var bulletShot = Instantiate(bullet, shootPos.position, Quaternion.identity);
            bulletShot.GetComponent<Bullets>()?.InitializeBullet(transform.rotation * Vector3.forward);

            AudioManager.Instance.Play3D(shootingAudio, shootPos.position, 0.1f);
            VFXManager.Instance.Play(shootingVFX, shootPos.position);
        }
    }

    private void Move()
    {

        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            return;
        }

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        var movementDirection = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movementDirection, Space.Self);



    }
}
