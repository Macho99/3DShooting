using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform leftHandTarget;
    [SerializeField] Transform leftHandHint;
    [SerializeField] RuntimeAnimatorController controller;

    public void GetTargetAndHint(out Transform target, out Transform hint)
    {
        target = leftHandTarget;
        hint = leftHandHint;
    }

    public RuntimeAnimatorController GetAnimController()
    {
        return controller;
    }
}
