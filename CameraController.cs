using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController: MonoBehaviour
{
 
    public float rotationSpeed = 1;
    public Transform target;
    public Transform player;
    float mouseX;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }


    private void LateUpdate()
    {
        
        CameraControl();
    }

    void CameraControl()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;

        transform.LookAt(target);

        target.rotation = Quaternion.Euler(0, mouseX, 0);
        player.rotation = Quaternion.Euler(0, mouseX, 0);
    }
}
