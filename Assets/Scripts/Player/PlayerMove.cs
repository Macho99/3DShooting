using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Transform moveOrigin;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float slideSpeed = 5f;
    [SerializeField] float slideAngle = 20f;

    public bool IsGround {  get; private set; }
    public bool IsRun {  get; private set; }

    Animator anim;
    CharacterController controller;
    Vector2 moveInput;
    float velY;
    bool colResult;
    RaycastHit hitInfo;
    LayerMask environmentMask;

    private void Awake()
    {
        velY = 0f;
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        environmentMask = LayerMask.GetMask("Environment");
    }

    private void Update()
    {
        Move();
        GroundCheck();
        Slide();
    }

    private void Slide()
    {
        //바닥과 충돌중이고, 경사면이고, 플레이어 인풋이 없을때
        if (colResult == true && Vector3.Angle(hitInfo.normal, Vector3.up) > slideAngle)
        {
            if(moveInput.sqrMagnitude < 0.1f)
            {
                Vector3 slideDirection = hitInfo.normal;
                slideDirection.y = 0f;
                controller.Move(slideDirection * slideSpeed * Time.deltaTime);
            }
        }
    }

    private void GroundCheck()
    {
        colResult = Physics.SphereCast(transform.position + controller.center, controller.radius,
            Vector3.down, out hitInfo, controller.center.y + 0.1f - controller.radius, environmentMask);

        IsGround = colResult;
    }

    private void Move()
    {
        if (true == IsGround)
            velY = -2f;
        else
        {
            velY += Physics.gravity.y * Time.deltaTime;
        }

        Vector3 moveDir = new Vector3();
        moveDir += moveInput.x * moveSpeed * moveOrigin.right;

        anim.SetFloat("XSpeed", moveInput.x, 0.5f, Time.deltaTime);
        anim.SetFloat("Speed", moveInput.sqrMagnitude);

        if (IsRun)
        {
            anim.SetFloat("YSpeed", moveInput.y * 2f, 0.5f, Time.deltaTime);
            moveDir += moveInput.y * runSpeed * moveOrigin.forward;

        }
        else
        {
            anim.SetFloat("YSpeed", moveInput.y, 0.5f, Time.deltaTime);
            moveDir += moveInput.y * moveSpeed * moveOrigin.forward;
        }

        moveDir.y = velY;

        controller.Move(moveDir * Time.deltaTime);
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if(false == IsGround) { return; }

        IsGround = false;
        velY = jumpForce;
    }

    private void OnRun(InputValue value)
    {
        IsRun = value.Get<float>() > 0.9f ? true : false;
    }

    private void OnDrawGizmos()
    {
        if(colResult == false) return;

        Gizmos.color = Color.yellow;

        Vector3 colPoint = hitInfo.point;
        colPoint.y += controller.radius;

        Gizmos.DrawWireSphere(colPoint, controller.radius);
    }
}