using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System.Collections;
using System;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] Rig rig;
    [SerializeField] float ikLerpSpeed = 10f;
    [SerializeField] WeaponHolder weaponHolder;

    public bool IsToss { get; private set; }
    public Rig Rig { get { return rig; } }
    public float IKLerpSpeed { get { return ikLerpSpeed; } }
    public WeaponHolder WeaponHolder { get {  return weaponHolder; } }

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

    public void SetAnimTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public bool IsAnimWait()
    {
        return anim.GetCurrentAnimatorStateInfo(1).IsName("Wait");
    }
}