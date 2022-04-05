// What is the point of this compared to Vector3.distance?
// It doesn't respect walls.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBall : MonoBehaviour
{
    Material mat;
    public LayerMask playerMask;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.CheckSphere(transform.position, 10, playerMask)) {
            mat.color = Color.green;
        }
        else {
            mat.color = Color.red;
        }
    }
}
