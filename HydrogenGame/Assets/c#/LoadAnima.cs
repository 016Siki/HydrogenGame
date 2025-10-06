using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAnima : MonoBehaviour
{
    public float speed ;

    void Update()
    {
        if (transform.parent != null)
        {
            // Z²‚ğ’†S‚Ée‚Ìü‚è‚ğ‰ñ“]
            transform.RotateAround(transform.parent.position, Vector3.forward, -speed * Time.deltaTime);
        }
    }
}
