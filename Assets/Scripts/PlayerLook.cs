using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] Transform camRoot;
    [SerializeField] float sensivility = 10f;
    [SerializeField] Transform aimPoint;

    Vector2 lookInput;
    float yAngle;
    float xAngle;
    Vector3 headPosition;
    Vector3 initCamPos;
    float camDist;
    LayerMask environmentMask;

    private void Awake()
    {
        float height = GetComponent<CharacterController>().height;
        headPosition = new Vector3(0f, height, 0f);
        initCamPos = camRoot.transform.localPosition;
        environmentMask = LayerMask.GetMask("Environment");
        camDist = (initCamPos - headPosition).magnitude;
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
        Vector3 lookPoint = Camera.main.transform.position + Camera.main.transform.forward * 10f;
        lookPoint.y = transform.position.y;
        aimPoint.transform.position = lookPoint;
        transform.LookAt(aimPoint);
    }

    private void LateUpdate()
    {
        Look();
        ObstacleCheck();
    }

    private void Look()
    {
        xAngle += lookInput.x * Time.deltaTime * sensivility;
        yAngle += lookInput.y * Time.deltaTime * sensivility;
        yAngle = Mathf.Clamp(yAngle, -40f, 40f);

        camRoot.rotation = Quaternion.Euler(new Vector3(-yAngle, xAngle, 0f));
    }

    private void ObstacleCheck()
    {
        Vector3 origin = transform.position + headPosition;
        bool result = Physics.Raycast(
            origin,
            camRoot.position - origin,
            out RaycastHit hitInfo,
            camDist,
            environmentMask);

        if (true == result)
        {
            camRoot.position = hitInfo.point;
        }
        else if (false == result)
        {
            camRoot.localPosition = initCamPos;
        }
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
}
