using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System.Collections;
using System;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] Rig rig;
    [SerializeField] MultiAimConstraint rightHandIK;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] float ikLerpSpeed = 10f;
    [SerializeField] WeaponHolder weaponHolder;

    public float RigIKWeight
    {
        get { return rig.weight; }
        set { rig.weight = value; }
    }
    public float HandIKWeight
    {
        get
        {
            return rightHandIK.weight;
        }
        set
        {
            rightHandIK.weight = value;
            leftHandIK.weight = value;
        }
    }

    public bool IsToss { get; private set; }
    public float IKLerpSpeed { get { return ikLerpSpeed; } }
    public WeaponHolder WeaponHolder { get {  return weaponHolder; } }
    public Animator Anim { get { return anim; } }

    private Animator anim;

	private void Awake()
	{
		anim = GetComponentInChildren<Animator>();
	}

	private void OnFire(InputValue value)
    {
		weaponHolder.Fire(value.isPressed);
    }

    private void OnToss(InputValue value)
    {
        if (value.isPressed == true)
        {
            weaponHolder.Toss();
        }
    }

    private void OnReload(InputValue value)
    {
        if (value.isPressed == true)
        {
            weaponHolder.Reload();
        }
    }

    private void OnNum1(InputValue value)
    {
        if (value.isPressed == true)
        {
            weaponHolder.ChangeGun(1);
        }
    }

    private void OnNum2(InputValue value)
    {
        if (value.isPressed == true)
        {
            weaponHolder.ChangeGun(2);
        }
    }

    public void SetAnimTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void SetAnimBool(string name, bool val)
    {
        anim.SetBool(name, val);
    }

    public bool IsAnimWait()
    {
        return anim.GetCurrentAnimatorStateInfo(1).IsName("Wait");
    }

    public void SetHandWeight(float weight)
    {

    }
}