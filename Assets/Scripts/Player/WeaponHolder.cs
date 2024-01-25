using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponHolder : MonoBehaviour
{
	[SerializeField] Transform leftHand;

	[SerializeField] Transform leftHandHoldTarget;
	[SerializeField] Transform leftHandHoldHint;

	[SerializeField] Transform subWeaponHoldLeft;
	[SerializeField] Transform subWeaponHoldRight;

	[SerializeField] Gun[] guns;

	private Rig rig;
	private float ikLerpSpeed;
	private PlayerAction playerAction;
	private Gun curGun;
	private Animator anim;
	private AccuracyController accuracyController;

	public float Accuracy { get { return accuracyController.Accuracy; } }
	public bool IsToss { get; private set; }

	private void Awake()
	{
		anim = GetComponentInParent<Animator>();
		Gun slottedGun = GetComponentInChildren<Gun>();
		accuracyController = new GameObject("AccuracyController").AddComponent<AccuracyController>();
		accuracyController.transform.parent = transform;
		accuracyController.transform.localPosition = Vector3.zero;

		ChangeWeapon(slottedGun);
	}

	private void Start()
	{
		playerAction = GetComponentInParent<PlayerAction>();
		rig = playerAction.Rig;
		ikLerpSpeed = playerAction.IKLerpSpeed;
	}

	private void SetTargetAndHint()
	{
		Transform target, hint;
		curGun.GetTargetAndHint(out target, out hint);

		leftHandHoldTarget.position = target.position;
		leftHandHoldTarget.rotation = target.rotation;
		leftHandHoldHint.position = hint.position;
		leftHandHoldHint.rotation = hint.rotation;
	}

	public void ChangeWeapon(int idx)
	{
		if (curGun == guns[0])
		{
			ChangeWeapon(guns[1]);
		}
		else
		{
			ChangeWeapon(guns[0]);
		}
	}

	public void ChangeWeapon(Gun newGun)
	{
		Vector3 prevLocalPos;
		Quaternion prevLocalRot;
		if (curGun != null)
		{
			prevLocalPos = curGun.transform.localPosition;
			prevLocalRot = curGun.transform.localRotation;
			curGun.transform.parent = subWeaponHoldLeft;
			curGun.transform.localPosition = prevLocalPos;
			curGun.transform.localRotation = prevLocalRot;
		}

		curGun = newGun;
		prevLocalPos = curGun.transform.localPosition;
		prevLocalRot = curGun.transform.localRotation;
		curGun.transform.parent = transform;
		curGun.transform.localPosition = prevLocalPos;
		curGun.transform.localRotation = prevLocalRot;

		anim.runtimeAnimatorController = curGun.GetAnimController();
		SetTargetAndHint();

		accuracyController.SetAccuracy(curGun.BaseAccuracy);
	}

	public void Toss()
	{
		if (curGun?.IsReload == true) { return; }
		if (IsToss == true) { return; }

		anim.SetTrigger("Toss");
		rig.weight = 0f;
		IsToss = true;
		if(curGun != null)
		{
			curGun.transform.parent = leftHand;
		}
		_ = StartCoroutine(CoTossEndCheck());
	}

	private IEnumerator CoTossEndCheck()
	{
		yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(1).IsName("Wait"));
		anim.Play("Empty");

		while (true == IsToss)
		{
			rig.weight = Mathf.Lerp(rig.weight, 1f, Time.deltaTime * ikLerpSpeed);
			if (rig.weight > 0.99f)
			{
				IsToss = false;
				break;
			}
			yield return null;
		}
		rig.weight = 1f;
		if (curGun != null)
		{
			curGun.transform.parent = transform;
		}
    }

    public void Fire(bool val)
	{
		curGun?.Fire(val);
	}

	public void Reload()
	{
		curGun?.Reload();
    }
    public void AddAccuracy(float amount)
    {
        accuracyController.AddAccuracy(amount);
    }

    public void SubAccuracy(float amount, float delay)
    {
        accuracyController.SubAccuracy(amount, delay);
    }

	private void OnDisable()
	{
		enabled = true;
	}
}
