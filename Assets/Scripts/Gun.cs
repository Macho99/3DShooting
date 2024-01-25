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
	[SerializeField] float baseAccuracy = 0.05f;
	[SerializeField] float accAmountPerFire = 0.05f;
	[SerializeField] float accRecoveryDelay = 0.5f;

	private float fireDelay;
	private Rig rig;
	private float ikLerpSpeed;
	private PlayerAction playerAction;
	private WeaponHolder weaponHolder;
	private Light muzzleLight;
	private Coroutine fireCoroutine;
	private float lastFireTime;

	public float BaseAccuracy { get { return baseAccuracy; } }

	private void Awake()
	{
		muzzleLight = GetComponentInChildren<Light>(true);
		fireDelay = 60 / rpm;
	}

	private void Start()
	{
		playerAction = FieldSceneFC.Player.GetComponent<PlayerAction>();
		weaponHolder = playerAction.WeaponHolder;
		rig = playerAction.Rig;
		ikLerpSpeed = playerAction.IKLerpSpeed;
	}

	public bool FireInput { get; private set; }
	public bool IsReload { get; private set; }

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
		if (weaponHolder.IsToss == true) return;
		if (IsReload == true) { return; }

		playerAction.SetAnimTrigger("Reload");
		rig.weight = 0f;
		IsReload = true;
		_ = StartCoroutine(CoReloadEndCheck());
	}

	private IEnumerator CoReloadEndCheck()
	{
		yield return new WaitUntil(() => playerAction.IsAnimWait());
		playerAction.SetAnimTrigger("Exit");
		while (true == IsReload)
		{
			rig.weight = Mathf.Lerp(rig.weight, 1f, Time.deltaTime * ikLerpSpeed);
			if (rig.weight > 0.99f)
			{
				IsReload = false;
				break;
			}
			yield return null;
		}
		rig.weight = 1f;
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
		while (FireInput == true)
        {
			lastFireTime = Time.time;
            playerAction.SetAnimTrigger("Fire");
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
	}

	IEnumerator CoTrailMove(TrailRenderer trail, Vector3 pos)
	{
		//print(pos);
		Vector3 dir = pos - trail.transform.position;
		dir.Normalize();
		float prevSqrDist = (trail.transform.position - pos).sqrMagnitude;
		while (true)
		{
			trail.transform.Translate(dir * bulletSpeed * Time.deltaTime, Space.World);
			float curSqrDist = (trail.transform.position - pos).sqrMagnitude;
			if(prevSqrDist < curSqrDist)
			{
				break;
			}
			else
			{
				prevSqrDist = curSqrDist;
			}
			yield return null;
		}
		LeanPool.Despawn(trail);
	}

	IEnumerator CoLightOff()
	{
		yield return new WaitForSeconds(0.02f);
		muzzleLight.gameObject.SetActive(false);
	}
}
