using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform leftHandTarget;
    [SerializeField] Transform leftHandHint;
	[SerializeField] Transform muzzle;
	[SerializeField] ParticleSystem muzzleParticle;
	[SerializeField] Bullet bulletPrefab;
    [SerializeField] RuntimeAnimatorController controller;

	[SerializeField] float rpm = 600f;
	[SerializeField] float bulletSpeed = 100f;
	[SerializeField] float damage = 10f;
	[SerializeField] int baseAccuracy = 5;
	[SerializeField] int accAmountPerFire = 50;
	[SerializeField] float accRecoveryDelay = 0.5f;
	[SerializeField] int magazineSize = 30;

	private int curMagazine;
	private float fireDelay;
	private float ikLerpSpeed;
	private PlayerAction playerAction;
	private WeaponHolder weaponHolder;
	private Light muzzleLight;
	private Coroutine fireCoroutine;
	private float lastFireTime;

	public int BaseAccuracy { get { return baseAccuracy; } }
	public bool FireInput { get; private set; }


	private void Awake()
	{
		muzzleLight = GetComponentInChildren<Light>(true);
		fireDelay = 60 / rpm;
		curMagazine = magazineSize;
	}

	private void Start()
	{
		playerAction = FieldSceneFC.Player.GetComponent<PlayerAction>();
		weaponHolder = playerAction.WeaponHolder;
		ikLerpSpeed = playerAction.IKLerpSpeed;
	}

    public void GetTargetAndHint(out Transform target, out Transform hint)
    {
        target = leftHandTarget;
        hint = leftHandHint;
    }

    public RuntimeAnimatorController GetAnimController()
    {
        return controller;
    }

	public void Reload()
	{
		if (weaponHolder.CurState != WeaponHolder.State.Idle) return;

		weaponHolder.CurState = WeaponHolder.State.Reload;
		playerAction.SetAnimTrigger("Reload");
		playerAction.HandIKWeight = 0f;
		_ = StartCoroutine(CoReloadEndCheck());
	}

	private IEnumerator CoReloadEndCheck()
	{
		yield return new WaitUntil(() => playerAction.IsAnimWait());
		playerAction.SetAnimTrigger("Exit");
		while (weaponHolder.CurState == WeaponHolder.State.Reload)
		{
			playerAction.HandIKWeight = Mathf.Lerp(playerAction.HandIKWeight, 1f, Time.deltaTime * ikLerpSpeed);
			if (playerAction.HandIKWeight > 0.99f)
			{
				break;
			}
			yield return null;
		}
		weaponHolder.CurState = WeaponHolder.State.Idle;
		curMagazine = magazineSize;
		playerAction.HandIKWeight = 1f;
	}

	public void Fire(bool val)
	{
		FireInput = val;
		if(val == true) {
			_ = StartCoroutine(CoFire());
		}
	}

	IEnumerator CoFire()
	{
		while (true)
		{
			if(weaponHolder.CurState == WeaponHolder.State.Idle) { break; }
			yield return null;
		}

		weaponHolder.CurState = WeaponHolder.State.Fire;
		while (FireInput == true)
        {
			if (curMagazine <= 0)
				break;

			playerAction.SetAnimBool("IsFire", true);
			curMagazine--;
			lastFireTime = Time.time;
            muzzleParticle.Play();
            muzzleLight.gameObject.SetActive(true);
            _ = StartCoroutine(CoLightOff());

            Bullet bullet = LeanPool.Spawn(bulletPrefab, muzzle.position, Quaternion.identity);
			float accuracy = weaponHolder.Accuracy;
            bullet.Init((muzzle.forward + Random.insideUnitSphere * accuracy) * bulletSpeed);
			weaponHolder.AddAccuracy(accAmountPerFire);
			weaponHolder.SubAccuracy(accAmountPerFire, accRecoveryDelay);

            yield return new WaitForSeconds(fireDelay);
		}

		weaponHolder.CurState = WeaponHolder.State.Idle;
		playerAction.SetAnimBool("IsFire", false);
	}

	IEnumerator CoLightOff()
	{
		yield return new WaitForSeconds(0.02f);
		muzzleLight.gameObject.SetActive(false);
	}

	public void GunDisable()
	{
		StopAllCoroutines();
		FireInput = false;
		playerAction.SetAnimBool("IsFire", false);
		muzzleLight.gameObject.SetActive(false);
	}
}
