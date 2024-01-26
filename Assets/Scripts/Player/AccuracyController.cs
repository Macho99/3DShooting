using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyController : MonoBehaviour
{
    private WeaponHolder weaponHolder;
    private int accuracyInt;
    const float accuracyMultiple = 0.001f;
    private float moveFactor;
    public float Accuracy
    {
        get { return (accuracyInt * accuracyMultiple) + moveFactor; }
    }

    private void Start()
    {
        weaponHolder = GetComponentInParent<WeaponHolder>();
    }

    public void SetAccuracy(int accuracyInt)
    {
        StopAllCoroutines();
        this.accuracyInt = accuracyInt;
    }

    public void AddAccuracy(int amount)
    {
        if (amount < 0f)
        {
            Debug.LogError($"{amount}는 음수입니다");
            return;
        }
        this.accuracyInt += amount;
    }

    public void SubAccuracy(int amount, float delay)
    {
        if (amount < 0f)
        {
            Debug.LogError($"{amount}는 음수입니다");
            return;
        }
        _ = StartCoroutine(CoSubAccuracy(amount, delay));
    }

    private IEnumerator CoSubAccuracy(int amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        this.accuracyInt -= amount;
    }

    public void SetMoveFactor(float moveFactor)
    {
        this.moveFactor = moveFactor;
    }
}
