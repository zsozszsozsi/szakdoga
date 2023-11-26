using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("CameraSettings")]
    public float movingSpeed = 100f;
    public float rotationSpeed = 200f;

    public static bool IsEnabled = false;

    // Update is called once per frame
    void Update()
    {
        if (IsEnabled) 
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += movingSpeed * Time.deltaTime * transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += movingSpeed * Time.deltaTime * -transform.forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position += movingSpeed * Time.deltaTime * -transform.right; ;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += movingSpeed * Time.deltaTime * transform.right;
            }

            if (Input.GetMouseButton(0))
            {
                transform.eulerAngles += rotationSpeed * Time.deltaTime * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            }
        }
        
    }
}
