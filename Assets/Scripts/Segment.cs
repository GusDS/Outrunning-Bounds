using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (transform.position.z + 200 < Control.playerPosition.z)
            Destroy(gameObject);
    }
}
