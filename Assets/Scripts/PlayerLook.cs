using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] Transform camRoot;
    [SerializeField] Transform character;
    [SerializeField] float sensivility = 10f;
    [SerializeField] float characterRotationSpeed = 10f;
    [SerializeField] Transform aimPoint;

    Vector2 lookInput;
    float yAngle;
    float xAngle;
    LayerMask environmentMask;

    private void Awake()
    {
        float height = GetComponent<CharacterController>().height;
        environmentMask = LayerMask.GetMask("Environment");
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        bool result = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
            out RaycastHit hitInfo, 100f, environmentMask);

        Vector3 lookPoint;
        if(true == result && (hitInfo.point - transform.position).sqrMagnitude > 3f * 3f)
        {
            lookPoint = hitInfo.point;
        }
        else
        {
            lookPoint = Camera.main.transform.position + Camera.main.transform.forward * 5f;
            lookPoint.y = 0f;
        }

        aimPoint.transform.position = lookPoint;
        Vector3 lookDir = new Vector3(lookPoint.x - transform.position.x, 0f, lookPoint.z - transform.position.z);
        float lookAngle = Vector3.Angle(lookDir, Vector3.forward);

        if (lookDir.x < 0f)
            lookAngle = 360f - lookAngle;

        float prevAngle = character.rotation.eulerAngles.y;
        float nextAngle = Mathf.LerpAngle(prevAngle, lookAngle, Time.deltaTime * characterRotationSpeed);

        character.rotation = Quaternion.Euler(new Vector3(0f, nextAngle, 0f));
        //character.LookAt(aimPoint);
    }

    private void LateUpdate()
    {
        Look();
    }

    private void Look()
    {
        xAngle += lookInput.x * Time.deltaTime * sensivility;
        yAngle += lookInput.y * Time.deltaTime * sensivility;
        yAngle = Mathf.Clamp(yAngle, -40f, 40f);

        camRoot.rotation = Quaternion.Euler(new Vector3(-yAngle, xAngle, 0f));
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
}
