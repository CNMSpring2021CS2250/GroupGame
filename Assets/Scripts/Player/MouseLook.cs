using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // Enumeration
    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    // Field
    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float sensitivityHor = 9.0f; //horizontal sensitivity
    public float sensitivityVert = 9.0f; //vertical sensitivity

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private float _rotationX = 0;


    // Update is called once per frame
    void Update()
    {
        if (axes == RotationAxes.MouseX)
        {
            // Horizontal rotation here
            //transform.Rotate(0, sensitivityHor, 0);
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);

        }
        else if (axes == RotationAxes.MouseY)
        {
            // Vertical rotation
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            float rotationY = transform.localEulerAngles.y; //convert quaternian to x,y,z format

            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
        else
        {
            // Do both horizontal and vertical rotations

            // Vertical component
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            // Horizontal component
            float delta = Input.GetAxis("Mouse X") * sensitivityHor;
            float rotationY = transform.localEulerAngles.y + delta; //convert quaternian to x,y,z format

            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
    }
}
