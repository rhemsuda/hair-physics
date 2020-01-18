using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 originPosition;
    private Quaternion originRotation;

    private void Start()
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
    }

    void Update ()
    {
        Vector3 movement = transform.forward * Input.GetAxis("Vertical") / 2f;
        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") / 2f);
        transform.position += movement;
	}

    public void ResetPosition()
    {
        transform.position = originPosition;
        transform.rotation = originRotation;
    }
}
