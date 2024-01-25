using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyController : MonoBehaviour
{
    private WeaponHolder weaponHolder;
    private float accuracy;
    public float Accuracy
    {
        get { return accuracy; }
        private set
        {
            accuracy = value;
        }
    }

    private void Start()
    {
        weaponHolder = GetComponentInParent<WeaponHolder>();
    }

    public void SetAccuracy(float accuracy)
    {
        StopAllCoroutines();
        Accuracy = accuracy;
    }

    public void AddAccuracy(float amount)
    {
        if (amount < 0f)
        {
            Debug.LogError($"{amount}는 음수입니다");
            return;
        }
        Accuracy += amount;
    }

    public void SubAccuracy(float amount, float delay = 0f)
    {
        if (amount < 0f)
        {
            Debug.LogError($"{amount}는 음수입니다");
            return;
        }
        _ = StartCoroutine(CoSubAccuracy(amount, delay));
    }

    private IEnumerator CoSubAccuracy(float amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        Accuracy -= amount;
    }
}
