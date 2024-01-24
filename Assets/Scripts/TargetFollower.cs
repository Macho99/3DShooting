using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : MonoBehaviour
{
    public Transform Target { get; set; }

    private void Update()
    {
        if (Target == null) return;

        transform.position = Target.position;
        transform.rotation = Target.rotation;
    }
}
