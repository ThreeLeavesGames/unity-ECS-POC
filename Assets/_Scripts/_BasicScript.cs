using System;
using UnityEngine;

[ExecuteInEditMode]
public class _BasicScript : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB = new Vector3(0,0,30);
    public Vector3 direction;

    void Start()
    {
        pointA= transform.position;
        // Destination - Origin.
     direction =  (pointB - pointA).normalized;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
