using System;
using System.Collections;
using System.Collections.Generic;
using NTC.Global.System;
using UnityEngine;
 
public class HealthSystem : MonoBehaviour, IDamagable
{
    [SerializeField] float health = 100;
    [SerializeField] GameObject hitVFX;
    [SerializeField] Animator animator;
    private IDieable controller;
    [SerializeField] private RagdollOperations ragdoll;


    public void Start()
    {
        ragdoll = GetComponent<RagdollOperations>();
        controller = GetComponent<IDieable>();
    }

    public void TakeDamage(float damageAmount)
    {
        Debug.Log(gameObject.name + " damage " + damageAmount);
        health -= damageAmount;
        animator.SetTrigger("damage");
 
        if (health <= 0)
        {
            Die();
        }
    }
 
    void Die()
    {
        controller.Die();
        try
        {
            animator.enabled = false;
            ragdoll.EnableRagdoll();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        
    }
    public void HitVFX(Vector3 hitPosition)
    {
        GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
        Destroy(hit, 3f);
 
    }

    public bool IsDead()
    {
        return health <= 0;
    }
}

public interface IDieable
{
    public void Die();
   
}