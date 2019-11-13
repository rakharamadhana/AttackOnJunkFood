using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public bool testFloatingJoystick = true;
    public float moveSpeed = 5;
    public float maxDashTime = 1.0f;
    public float dashSpeed = 1.0f;
    public float dashStoppingSpeed = 0.1f;
    public float maxDashLimit = 3.0f;
    public float dashLimit = 3.0f;
    public float dashRechargeTime = 3.0f;
    public float lookSpeed = 50;
    public Crosshairs crosshairs;
    public FixedJoystick movementJoystick;
    public FixedJoystick rotationJoystick;
    public FixedButton fireButton;
    public GameObject monsterInfoUI;

    Animator anim;

    Camera viewCamera;
    Transform camT;
    Vector3 camForward;
    Vector3 move;
    Vector3 moveInput;

    float forwardAmount;
    float turnAmount;
    private float currentDashTime;

    bool isGrounded;
    public bool isDashing;

    PlayerController controller;
    GunController gunController;
    Vector3 moveDir;

    Quaternion targetRotation;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gunController.EquipGun(0);
        currentDashTime = maxDashTime;
    }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;

        camT = Camera.main.transform;

        isGrounded = true;

        FindObjectOfType<EnemySpawner>().OnNewWave += OnNewWave;

        if(testFloatingJoystick)
        {
            Cursor.visible = true;
            movementJoystick.gameObject.SetActive(true);
            rotationJoystick.gameObject.SetActive(true);
            crosshairs.gameObject.SetActive(false);
        }
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        //gunController.EquipGun(waveNumber - 1);
    }

    // Update is called once per frame
    void Update()
    {
        float m_horizontal;
        float m_vertical;

        float r_horizontal;
        float r_vertical;

        // Floating Joystick Input
        if (testFloatingJoystick)
        {

            // Joystick Movement Input
            m_horizontal = movementJoystick.Horizontal;
            m_vertical = movementJoystick.Vertical;

            //m_horizontal = Input.GetAxisRaw("Horizontal");
            //m_vertical = Input.GetAxisRaw("Vertical");

            r_horizontal = rotationJoystick.Horizontal;
            r_vertical = rotationJoystick.Vertical;

            // Animate Joystick Look
            if (camT != null)
            {
                camForward = Vector3.Scale(camT.up, new Vector3(1, 0, 1)).normalized;
                move = m_vertical * camForward + m_horizontal * camT.right;

            }
            else
            {
                move = m_vertical * Vector3.forward + m_horizontal * Vector3.right;
            }

            if (move.magnitude > 1)
            {
                move.Normalize();
            }
            Move(move);

            // Joystick Look Input
            Vector3 rotationInput = new Vector3(r_horizontal, 0, r_vertical);
            if (rotationInput != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(rotationInput);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, lookSpeed);
        }
        else
        {
            m_horizontal = Input.GetAxisRaw("Horizontal");
            m_vertical = Input.GetAxisRaw("Vertical");

            // Animate Mouse Look
            if (camT != null)
            {
                camForward = Vector3.Scale(camT.up, new Vector3(1, 0, 1)).normalized;
                move = m_vertical * camForward + m_horizontal * camT.right;
            }
            else
            {
                move = m_vertical * Vector3.forward + m_horizontal * Vector3.right;
            }

            if (move.magnitude > 1)
            {
                move.Normalize();
            }
            Move(move);

            // Look Input
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
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

        // Fire
        if (monsterInfoUI.gameObject.activeSelf != true)
        {
            if (fireButton.Pressed || Input.GetKey(KeyCode.Mouse0))
            {
                gunController.OnTriggerHold();
            }
            else
            {
                gunController.OnTriggerRelease();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                gunController.Reload();
            }
        }

        //Falling
        if (transform.position.y < -10)
        {
            TakeDamage(health);
        }

        // Movement Input
        Vector3 moveInput = new Vector3(m_horizontal, 0, m_vertical);
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        moveDir = moveVelocity;
        controller.Move(moveVelocity);

        // Dashing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }
        if (currentDashTime < maxDashTime)
        {
            isDashing = true;
            controller.Move(moveDir * dashSpeed);
            currentDashTime += dashStoppingSpeed;
        }
        else
        {
            isDashing = false;
            //Debug.Log("Nothing");
            GetComponent<TrailRenderer>().enabled = false;
        }

        if (dashLimit == 0)
        {
            StartCoroutine("resetDashLimit");
            //Debug.Log(dashLimit);
        }
        else if (dashLimit == maxDashLimit)
        {
            StopCoroutine("resetDashLimit");
        }
    }
    public void Dash()
    {
        if (dashLimit > 0)
        {
            AudioManager.instance.PlaySound("Dash", transform.position);
            currentDashTime = 0.0f;
            GetComponent<TrailRenderer>().enabled = true;
            dashLimit--;   
        }
    }

    IEnumerator resetDashLimit()
    {
        yield return new WaitForSeconds(dashRechargeTime);
        dashLimit = maxDashLimit;
        //Debug.Log("Dash: " + dashLimit + "/" + maxDashLimit);
        
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
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
