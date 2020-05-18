using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangedTextMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Vector3.up;
    }
}
