using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float pushForce = 5f;
    public float upwardForce = 2f;


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;    

        if (rb == null || rb.isKinematic)
        {
            return;
        }

        if (!rb.CompareTag("Pushable"))
        {
            return;
        }

        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        Vector3 pushDir = new(hit.moveDirection.x, upwardForce, hit.moveDirection.z);
        rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
}
