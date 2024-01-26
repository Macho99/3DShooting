using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponHolder : MonoBehaviour
{
	public enum State { Idle, Fire, Reload, Change, Toss, Size }

	[SerializeField] Transform leftHand;

	[SerializeField] Transform leftHandHoldTarget;
	[SerializeField] Transform leftHandHoldHint;

	[SerializeField] Transform subWeaponHoldLeft;
	[SerializeField] Transform subWeaponHoldRight;
	[SerializeField] Grenade grenadePrefab;

	private float ikLerpSpeed;
	private PlayerAction playerAction;
	private Animator anim;
	private AccuracyController accuracyController;

	private Grenade curGrenade;
	private Gun[] guns;
	private Gun curGun;
	private Gun newGun;
	private int curGunNum;

	public State CurState { get; set; }
	public float Accuracy { get { return accuracyController.Accuracy; } }

	private void Awake()
	{
		CurState = State.Idle;
		guns = GetComponentsInChildren<Gun>();
		accuracyController = new GameObject("AccuracyController").AddComponent<AccuracyController>();
		accuracyController.transform.parent = transform;
		accuracyController.transform.localPosition = Vector3.zero;

		if (guns.Length >= 1)
		{
			guns[0].transform.SetParent(subWeaponHoldLeft, false);
		}
		if (guns.Length >= 2)
		{
			guns[1].transform.SetParent(subWeaponHoldRight, false);
		}
	}

	private void Start()
	{
		playerAction = GetComponentInParent<PlayerAction>();
		anim = playerAction.Anim;
		ikLerpSpeed = playerAction.IKLerpSpeed;
		ChangeGun(1);
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

	public void ChangeGun(int num)
	{
		if (guns.Length < num) { return; }
		if (guns[num - 1] == null ) { return; }
		if(num == curGunNum) { return; }
		if (State.Idle != CurState) return;

		curGunNum = num;
		ChangeGun(guns[num - 1]);
	}

	public void ChangeGun(Gun newGun)
	{
		CurState = State.Change;
		anim.SetTrigger("ChangeGun");
		curGun?.GunDisable();
		playerAction.RigIKWeight = 0f;
		this.newGun = newGun;
	}

	public void HandUp()
	{
		Vector3 prevLocalPos;
		Quaternion prevLocalRot;
		if (curGun != null)
		{
			prevLocalPos = curGun.transform.localPosition;
			prevLocalRot = curGun.transform.localRotation;

			if (guns[0] == curGun)
			{
				curGun.transform.parent = subWeaponHoldLeft;
			}
			else
			{
				curGun.transform.parent = subWeaponHoldRight;
			}

			curGun.transform.localPosition = prevLocalPos;
			curGun.transform.localRotation = prevLocalRot;
		}
	}

	public void DrawGun()
	{
		curGun = newGun;

		if (curGun != null)
		{
			curGun.transform.SetParent(transform, false);
			
			SetTargetAndHint();

			accuracyController.SetAccuracy(curGun.BaseAccuracy);
		}
		else
		{
			accuracyController.SetAccuracy(0);
		}

		newGun = null;
		_ = StartCoroutine(CoWait());
	}

	public IEnumerator CoWait()
	{
		yield return new WaitUntil(()=>playerAction.IsAnimWait());

		if(curGun != null)
		{
			anim.runtimeAnimatorController = curGun.GetAnimController();
		}

		anim.SetTrigger("Exit");

		while (true)
		{
			playerAction.RigIKWeight = Mathf.Lerp(playerAction.RigIKWeight, 1f, Time.deltaTime * ikLerpSpeed);
			if (playerAction.RigIKWeight > 0.99f)
			{
				playerAction.RigIKWeight = 1f;
				break;
			}
			yield return null;
		}
		CurState = State.Idle;
	}

	public void Toss()
	{
		if (CurState != State.Idle) return;

		CurState = State.Toss;
		anim.SetTrigger("Toss");
		playerAction.HandIKWeight = 0f;

		if(curGun != null)
		{
			curGun.transform.parent = leftHand;
		}
		curGrenade = LeanPool.Spawn(grenadePrefab, transform.position, transform.rotation, transform);
		_ = StartCoroutine(CoTossEndCheck());
	}

	public void TossGrenade()
	{
		curGrenade.Init(transform.forward);
	}

	private IEnumerator CoTossEndCheck()
	{
		yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(1).IsName("Wait"));
		anim.Play("Empty");

		while (State.Toss == CurState)
		{
			playerAction.HandIKWeight = Mathf.Lerp(playerAction.HandIKWeight, 1f, Time.deltaTime * ikLerpSpeed);
			if (playerAction.HandIKWeight > 0.99f)
			{
				break;
			}
			yield return null;
		}

		CurState = State.Idle;
		playerAction.HandIKWeight = 1f;

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

    public void AddAccuracy(int amount)
    {
        accuracyController.AddAccuracy(amount);
    }

    public void SubAccuracy(int amount, float delay = 0f)
    {
        accuracyController.SubAccuracy(amount, delay);
    }

	public void SetMoveFactor(float moveFactor)
	{
		accuracyController.SetMoveFactor(moveFactor);
	}

	private void OnDisable()
	{
		enabled = true;
	}
}
