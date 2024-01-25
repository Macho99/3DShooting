using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform leftHandTarget;
    [SerializeField] Transform leftHandHint;
	[SerializeField] Transform muzzle;
	[SerializeField] ParticleSystem muzzleParticle;
	[SerializeField] ParticleSystem hitParticlePrefab;
	[SerializeField] TrailRenderer trailPrefab;
    [SerializeField] RuntimeAnimatorController controller;

	[SerializeField] float rpm = 600f;
	[SerializeField] float trailSpeed = 100f;
	[SerializeField] float damage = 10f;

	private float fireDelay;
	private Rig rig;
	private float ikLerpSpeed;
	private PlayerAction playerAction;
	private Transform aimPoint;
	private WeaponHolder weaponHolder;
	private Light muzzleLight;
	private LayerMask hitMask;
	private Coroutine fireCoroutine;

	private void Awake()
	{
		muzzleLight = GetComponentInChildren<Light>(true);
		fireDelay = 60 / rpm;
		hitMask = LayerMask.GetMask("Environment", "Monster");
	}

	private void Start()
	{
		playerAction = FieldSceneFC.Player.GetComponent<PlayerAction>();
		aimPoint = FieldSceneFC.Player.GetComponent<PlayerLook>().AimPoint;
		weaponHolder = playerAction.WeaponHolder;
		rig = playerAction.Rig;
		ikLerpSpeed = playerAction.IKLerpSpeed;
	}

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
		if(val == true) {
			fireCoroutine = StartCoroutine(CoFire());
		}
		else
		{
			StopCoroutine(fireCoroutine);
		}
	}

	IEnumerator CoFire()
	{
		while (true)
		{
			GunFire();
			yield return new WaitForSeconds(fireDelay);
		}
	}

	private void GunFire()
	{
		playerAction.SetAnimTrigger("Fire");
		muzzleParticle.Play();
		muzzleLight.gameObject.SetActive(true);
		_ = StartCoroutine(CoLightOff());

		TrailRenderer trail = LeanPool.Spawn(trailPrefab, muzzle.position, muzzle.rotation);
		trail.Clear();
		if(Physics.Raycast(muzzle.position, aimPoint.position - muzzle.position, out RaycastHit hitInfo, 200f, hitMask))
		{
			ParticleSystem hitParticle = LeanPool.Spawn(hitParticlePrefab);
			hitParticle.transform.parent = hitInfo.transform;
			hitParticle.transform.position = hitInfo.point;
			hitParticle.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
			LeanPool.Despawn(hitParticle, 10f);

			_= StartCoroutine(CoTrailMove(trail, hitInfo.point));
		}
		else
		{
			_= StartCoroutine(CoTrailMove(trail, aimPoint.position));
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
			trail.transform.Translate(dir * trailSpeed * Time.deltaTime, Space.World);
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
