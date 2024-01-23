using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] TwoBoneIKConstraint leftHandHold;
    [SerializeField] float IKLerpSpeed = 10f;

    public bool IsReload { get; private set; }


    
    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void OnFire(InputValue value)
    {
        if(value.isPressed == true)
        {
            anim.SetTrigger("Fire");
        }
    }

    private void OnReload(InputValue value)
    {
        if(value.isPressed == true)
        {
            if(IsReload == true) { return; }

            anim.SetTrigger("Reload");
            leftHandHold.weight = 0f;
            IsReload = true;
            _ = StartCoroutine(CoReloadEndCheck());
        }
    }

    private IEnumerator CoReloadEndCheck()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(1).IsName("Wait"));
        IsReload = false;
        anim.Play("Empty");
        while (true)
        {
            leftHandHold.weight = Mathf.Lerp(leftHandHold.weight, 0.6f, Time.deltaTime * IKLerpSpeed);
            if (leftHandHold.weight > 0.5f)
                break;
            yield return null;
        }
    }
}
