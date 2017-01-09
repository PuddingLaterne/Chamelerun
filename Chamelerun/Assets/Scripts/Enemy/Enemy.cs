using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Enemy : MonoBehaviour 
{
    public int MaxHealth;
    public float InvincibilityTime;
    public float DeathAnimationSpeedScale = 2f;
    public bool CauseDamageOnCollision = true;

    [Header("Tongue Interaction")]
    public float KnockbackForce;
    public bool DamagedByPunching = true;

    [Header("Bouncing")]
    public float UpperSideAngle;
    public float Bounciness;
    public bool DamagedByBouncing = true;

    [Header("Points")]
    public int PointsForDamage;
    public int PointsForKill;

    public UnityAction OnDamaged = delegate { };
    public UnityAction<float> OnKilled = delegate { };

    private EnemyMovement movement;
    private Animator anim;

    private bool hasBounceAnimation;
    private bool hasHurtAnimation;
    private bool hasDeathAnimation;
    private float deathAnimationClipLength;

    private int currentHealth;
    private bool isInvincible;

    public void Awake()
    {
        movement = GetComponentInChildren<EnemyMovement>();
        anim = GetComponentInChildren<Animator>();
        hasBounceAnimation = anim.HasParameter("bounce");
        hasHurtAnimation = anim.HasParameter("hurt");
        hasDeathAnimation = anim.HasParameter("death");
        deathAnimationClipLength = anim.GetAnimationClipLength("death") / DeathAnimationSpeedScale;
    }

    public void OnEnable()
    {
        gameObject.SetLayersRecursively(LayerMask.NameToLayer("Enemy"));
        currentHealth = MaxHealth;
        isInvincible = false;
    }

    public void Punch(Vector2 direction)
    {
        if (movement != null)
        {
            movement.KnockBack(direction * KnockbackForce);
        }
        if (DamagedByPunching)
        {
            ApplyDamage();
        }
    }

    public Vector2 Bounce()
    {
        if (DamagedByBouncing)
        {
            ApplyDamage();
        }
        if (hasBounceAnimation) anim.SetTrigger("bounce");
        return Vector2.up * Bounciness;
    }

    private void ApplyDamage()
    {
        if(isInvincible || currentHealth < 0) return;
        if (currentHealth > 0)
        {
            currentHealth--;
            StartCoroutine(WaitForInvincibilityTime());
            OnDamaged();
            if(hasHurtAnimation) anim.SetTrigger("hurt");
        }
        else
        {
            OnKilled(deathAnimationClipLength);
            gameObject.SetLayersRecursively(LayerMask.NameToLayer("Default"));
            if (hasDeathAnimation) anim.SetTrigger("death");
        }
    }

    private IEnumerator WaitForInvincibilityTime()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
    }
}
