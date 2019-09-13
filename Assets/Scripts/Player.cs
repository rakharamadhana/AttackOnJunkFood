using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;
    public Crosshairs crosshairs;

    Animator anim;

    Camera viewCamera;
    Transform camT;
    Vector3 camForward;
    Vector3 move;
    Vector3 moveInput;

    float forwardAmount;
    float turnAmount;

    bool isGrounded;

    PlayerController controller;
    GunController gunController;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;

        camT = Camera.main.transform;

        isGrounded = true;

        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    // Update is called once per frame
    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Animate Movement with Mouse Aim
        if (camT != null)
        {
            camForward = Vector3.Scale(camT.up, new Vector3(1, 0, 1)).normalized;
            move = vertical * camForward + horizontal * camT.right;

        }
        else
        {
            move = vertical * Vector3.forward + horizontal * Vector3.right;
        }

        if(move.magnitude > 1)
        {
            move.Normalize();
        }
        Move(move);

        // Movement Input
        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        // Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
        }

        // Weapon Input
        if(Input.GetButton("Fire1"))
        {
            gunController.OnTriggerHold();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            gunController.OnTriggerRelease();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        // Jump Input
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                anim.SetTrigger("Jump");
                isGrounded = false;
            }
            else
            {
                isGrounded = true;
            }
        }
    }

    void Move(Vector3 move)
    {
        if(move.magnitude > 1)
        {
            move.Normalize();
        }

        this.moveInput = move;

        ConvertMoveInput();
        UpdateAnimator();
    }

    void ConvertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);
        //Debug.Log("localMove => " + localMove);
        turnAmount = localMove.x;
        //Debug.Log("turnAmount => " + turnAmount);
        forwardAmount = localMove.z;
        //Debug.Log("forwardAmount => " + forwardAmount);
    }

    void UpdateAnimator() 
    {
        anim.SetFloat("Forward", forwardAmount);
        anim.SetFloat("Turn", turnAmount);
    }
}
