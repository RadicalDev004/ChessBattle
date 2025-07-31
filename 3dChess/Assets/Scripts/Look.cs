using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public GameObject Board;
    public Quaternion initialAngle;
    public float speed = 25;
    public float maxAngle = 50;

    private void Awake()
    {
        initialAngle = transform.rotation;
    }

    private void Update()
    {
        Ref.Camera.transform.LookAt(Board.transform);

        float signedAngle = GetSignedAngle(initialAngle, transform.rotation, Vector3.right);

        if (Input.GetKey(KeyCode.LeftArrow) && signedAngle < maxAngle)
        {
            transform.Rotate(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow) && signedAngle > -maxAngle)
        {
            transform.Rotate(-speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAngle();
        }
    }

    float GetSignedAngle(Quaternion from, Quaternion to, Vector3 axis)
    {
        Quaternion delta = Quaternion.Inverse(from) * to;
        delta.ToAngleAxis(out float angle, out Vector3 angleAxis);

        // Normalize and determine the sign
        if (angle > 180f) angle -= 360f;

        float sign = Mathf.Sign(Vector3.Dot(axis, angleAxis));
        return angle * sign;
    }

    public void ResetAngle()
    {
        Tween.Rotation(transform, initialAngle, 0.5f, 0, Tween.EaseInOut);
    }
}
