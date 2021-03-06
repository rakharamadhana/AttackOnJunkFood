﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking};
    State currentState;

    public ParticleSystem deathEffect;
    public int scorePoint;
    public static event System.Action OnDeathStatic;

    public float moveSpeed;
    public float hitsToKillPlayer;
    public float enemyHealth;
    public Image healthbar;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    Player playerEntity;
    Color originalColor;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerEntity = FindObjectOfType<Player>();
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            originalColor = gameObject.GetComponentInChildren<Renderer>().material.color;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponentInChildren<CapsuleCollider>().radius;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        SetCharacteristics();
        base.Start();
        gameObject.GetComponentInChildren<Renderer>().material.color = originalColor;
        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }
    }

    private void SetCharacteristics()
    {
        ParticleSystem ps = deathEffect.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule psmain = ps.main;

        pathfinder.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }

        startingHealth = enemyHealth;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        healthbar.fillAmount -= damage / enemyHealth;
        if (damage >= health)
        {
            if(OnDeathStatic != null)
            {
                OnDeathStatic();
            }

            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject,deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    private void OnDestroy()
    {
        AddScore(scorePoint);
    }

    public void AddScore(int value)
    {
        if (ScoreKeeper.streakCount > 0)
        {
            ScoreKeeper.score += value * ScoreKeeper.streakCount;
        }
        else
        {
            ScoreKeeper.score += value;
        }
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    if (!playerEntity.isDashing)
                    {
                        AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                        StartCoroutine(Attack());
                    }
                }
            }
        }
        
    }
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        //skinMaterial.color = Color.white;
        bool hasAppliedDamage = false;

        while (percent <= 1 )
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }
        //skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;
        
        while (hasTarget)
        {
            if(currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
