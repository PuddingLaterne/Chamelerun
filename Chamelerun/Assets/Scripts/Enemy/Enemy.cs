using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Enemy : MonoBehaviour 
{
    public int MaxHealth;
    public float InvincibilityTime;

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

    [Header("Powerups")]
    public PowerupType PowerupDroppedOnDamaged;
    public PowerupType PowerupDroppedOnKilled;

    public UnityAction OnDamaged = delegate { };
    public UnityAction OnKilled = delegate { };

    private EnemyMovement movement;

    private int currentHealth;
    private bool isInvincible;

    public void Awake()
    {
        movement = GetComponentInChildren<EnemyMovement>();
    }

    public void OnEnable()
    {
        currentHealth = MaxHealth;
        isInvincible = false;
    }

    public void Punch(Vector2 direction)
    {
        if (movement != null)
        {
            movement.KnockBack(direction * KnockbackForce);
        }
        ApplyDamage();
    }

    public Vector2 Bounce()
    {
        if (DamagedByBouncing)
        {
            ApplyDamage();
        }
        return Vector2.up * Bounciness;
    }

    private void ApplyDamage()
    {
        if(isInvincible) return;
        if (currentHealth > 0)
        {
            currentHealth--;
            StartCoroutine(WaitForInvincibilityTime());
            OnDamaged();
        }
        else
        {
            OnKilled();
        }
    }

    private IEnumerator WaitForInvincibilityTime()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
    }
}
