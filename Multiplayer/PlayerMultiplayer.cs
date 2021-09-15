using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMultiplayer : MonoBehaviour, IPunObservable
{
    public AudioClip shootingAudio;
    public GameObject shootingVFX;

    public int rotationSpeed = 1;
    float mouseX;
    Rigidbody rb;
    PhotonView photonView;
    public GameObject tankMesh;
    BoxCollider boxCollider;
    public float moveSpeed = 5.0f;
    public GameObject bullet;
    public Transform shootPos;
    public Slider HealthBar;
    public float fireRate = 0.7f;
    public float nextShot;
    float cameraDist = 3.0f;
    
    private int health = 100;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        boxCollider = GetComponent<BoxCollider>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }


        Move();
        UpdateCamera();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("Shoot", RpcTarget.AllViaServer);
        }

    }


    private void UpdateCamera()
    {

        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;


        Camera.main.transform.position = transform.position - transform.forward * cameraDist;
        Camera.main.transform.LookAt(transform.position);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x , Camera.main.transform.position.y + 1.5f, Camera.main.transform.position.z);
        Camera.main.transform.localRotation = Quaternion.Euler(0, mouseX, 0);
        transform.rotation = Quaternion.Euler(0, mouseX, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            var bullet = collision.gameObject.GetComponent<BulletMultiplayer>();
            Debug.Log("Bullet Collision");
            TakeDamage(bullet);
        }
    }

    private void TakeDamage(BulletMultiplayer bullet)
    {

        health -= bullet.damage;
        HealthBar.value = health;
        if (health <= 0)
        {
            bullet.Owner.AddScore(1);
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        boxCollider.enabled = false;
        tankMesh.SetActive(false);
        HealthBar.gameObject.SetActive(false);
        rb.isKinematic = true;
        Debug.Log("Player Died");
        StartCoroutine(RespawnWaitTime());
    }

    private IEnumerator RespawnWaitTime()
    {
        yield return new WaitForSeconds(2f);

        tankMesh.SetActive(true);
        boxCollider.enabled = true;
        rb.isKinematic = false;
        health = 100;
        HealthBar.value = health;
        HealthBar.gameObject.SetActive(true);
    }

    [PunRPC]
    private void Shoot()
    {
        if (Time.time > nextShot)
        {
            nextShot = Time.time + fireRate;
            var bulletShot = Instantiate(bullet, shootPos.position, Quaternion.identity);
            bulletShot.GetComponent<BulletMultiplayer>()?.InitializeBullet(transform.rotation * Vector3.forward, photonView.Owner);

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       if(stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            HealthBar.value = health;
        }
    }
}
