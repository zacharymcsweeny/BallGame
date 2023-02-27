using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity;

    float xAxisClamp = 0;
    float followDistance = 10f;
    float cameraHeight = 2f;

    [SerializeField]
    Transform player;

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        RotateCamera();
        ZoomCamera();
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmtX = mouseX * mouseSensitivity;
        float rotAmtY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmtY;

        Vector3 rotPlayer = Camera.main.transform.rotation.eulerAngles;

        rotPlayer.x -= rotAmtY;
        rotPlayer.z = 0;
        rotPlayer.y += rotAmtX;

        if (xAxisClamp > 90)
        {
            xAxisClamp = 90;
            rotPlayer.x = 90;
        }
        else if (xAxisClamp < -90)
        {
            xAxisClamp = -90;
            rotPlayer.x = 270;
        }

        transform.rotation = Quaternion.Euler(rotPlayer);
        transform.position = player.transform.position;
        transform.position -= transform.forward * followDistance;
        transform.position += Vector3.up * cameraHeight;
    }

    void ZoomCamera()
    {
        RaycastHit hit;
        Ray clearance = new Ray(player.transform.position, transform.position - player.transform.position);

        if (Physics.Raycast(clearance, out hit, followDistance))
        {
            transform.position = hit.point;
            transform.position += transform.forward * 0.2f;
        }
    }
}
