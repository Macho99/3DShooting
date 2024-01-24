using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System.Collections;
using System;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] Rig rig;
    [SerializeField] float IKLerpSpeed = 10f;

    [SerializeField] Transform weaponHold;
    [SerializeField] Transform subWeaponHold;

    [SerializeField] TargetFollower leftHandHoldTarget;
    [SerializeField] TargetFollower leftHandHoldHint;

    [SerializeField] Gun[] guns;

    private Gun curGun;

    public bool IsReload { get; private set; }
    public bool IsToss { get; private set; }


    Coroutine reloadEndCheckCoroutine;
    Coroutine tossEndCheckCoroutine;
    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        Gun slottedGun= weaponHold.GetComponentInChildren<Gun>();

        ChangeWeapon(slottedGun);
    }

    private void SetTargetAndHint()
    {
        Transform target, hint;
        curGun.GetTargetAndHint(out target, out hint);

        leftHandHoldTarget.Target = target;
        leftHandHoldHint.Target = hint;
    }

    private void OnFire(InputValue value)
    {
        if(value.isPressed == true)
        {
            if (IsReload == true) { return; }
            if (IsToss == true) { return; }

            anim.SetTrigger("Fire");
        }
    }

    private void OnReload(InputValue value)
    {
        if(value.isPressed == true)
        {
            if(IsReload == true) { return; }
            if(IsToss == true) { return; }

            if(reloadEndCheckCoroutine != null)
            {
                StopCoroutine(reloadEndCheckCoroutine);
            }

            anim.SetTrigger("Reload");
            rig.weight = 0f;
            IsReload = true;
            reloadEndCheckCoroutine = StartCoroutine(CoReloadEndCheck());
        }
    }

    private void OnToss(InputValue value)
    {
        if (value.isPressed == true)
        {
            if (IsReload == true) { return; }
            if (IsToss == true) { return; }

            if(tossEndCheckCoroutine != null)
            {
                StopCoroutine(tossEndCheckCoroutine);
            }

            anim.SetTrigger("Toss");
            rig.weight = 0f;
            IsToss = true;
            curGun.gameObject.SetActive(false);
            tossEndCheckCoroutine = StartCoroutine(CoTossEndCheck());
        }
    }

    private IEnumerator CoTossEndCheck()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(1).IsName("Wait"));
        anim.Play("Empty");
        curGun.gameObject.SetActive(true);
        while (true)
        {
            rig.weight = Mathf.Lerp(rig.weight, 1f, Time.deltaTime * IKLerpSpeed);
            if (rig.weight > 0.99f)
            {
                rig.weight = 1f;
                IsToss = false;
                break;
            }
            yield return null;
        }
    }

    private IEnumerator CoReloadEndCheck()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(1).IsName("Wait"));
        anim.Play("Empty");
        while (true)
        {
            rig.weight = Mathf.Lerp(rig.weight, 1f, Time.deltaTime * IKLerpSpeed);
            if (rig.weight > 0.99f)
            {
                rig.weight = 1f;
                IsReload = false;
                break;
            }
            yield return null;
        }
    }

    private void OnChangeWeapon(InputValue value)
    {
        if (IsReload == true) { return; }
        if (IsToss == true) { return; }

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
            curGun.transform.parent = subWeaponHold;
            curGun.transform.localPosition = prevLocalPos;
            curGun.transform.localRotation = prevLocalRot;
        }

        curGun = newGun;
        prevLocalPos = curGun.transform.localPosition;
        prevLocalRot = curGun.transform.localRotation;
        curGun.transform.parent = weaponHold;
        curGun.transform.localPosition = prevLocalPos;
        curGun.transform.localRotation = prevLocalRot;

        anim.runtimeAnimatorController = curGun.GetAnimController();

        SetTargetAndHint();
    }
}
