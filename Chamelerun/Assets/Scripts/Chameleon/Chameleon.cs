using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Chameleon : MonoBehaviour 
{
    public float InvincibilityTime  = 0.5f;

    public UnityAction OnPowerChanged = delegate { };
    public UnityAction<PowerupType> OnPowerupAdded = delegate { };
    public UnityAction OnAllPowerLost = delegate { };

    public Transform Transform { get { return body.transform; } }
    public Vector2 Position { get { return body.Position; } }
    public Vector2 TonguePosition { get { return Position + tongue.OffsetOnBody; } }

    public PowerupType[] CurrentPowerups { get { return power.Powerups; } }
    public int CurrentPower { get { return power.Power; } }
    public PowerLevel CurrentPowerLevel { get{ return power.PowerLevel; } }

    private ChameleonBody body;
    private ChameleonTongue tongue;
    private ChameleonAnimation anim;
    private ChameleonPower power;

    private bool isInvincible;

    public void Init()
    {
        body = GetComponentInChildren<ChameleonBody>();
        tongue = GetComponentInChildren<ChameleonTongue>();
        anim = GetComponentInChildren<ChameleonAnimation>();
        power = GetComponentInChildren<ChameleonPower>();

        body.Init(tongue, power);
        tongue.Init(body, power);
        anim.Init(body, tongue);

        tongue.OnAttached += () =>
        {
            body.OnTongueAttached();
            anim.OnTongueAttached();
        };
        tongue.OnReleased += () =>
        {
            body.OnTongueReleased();
            anim.OnTongueReleased();
        };
        tongue.OnExpanded += () => anim.OnTongueExpanded();
        tongue.OnHurt += () => ApplyDamage();
        body.OnJump += () => anim.OnJump();

        power.OnAllPowerLost += () =>
        {
            anim.OnDead();
            OnAllPowerLost();
        };
        power.OnPowerupAdded += (type) =>
        {
            OnPowerupAdded(type);
            anim.OnPowerupCollected();
        };
        power.OnPowerChanged += () => OnPowerChanged();
    }

    public void Reset()
    {
        StopAllCoroutines();

        body.Reset();
        tongue.Reset();
        power.Reset();
        anim.Reset();

        isInvincible = false;
    }

    public void EnableControl(bool enabled)
    {
        body.enabled = enabled;
        tongue.enabled = enabled;
        power.enabled = enabled;
        //anim.enabled = enabled;
    }

    public void ChameleonUpdate()
    {
        body.ChameleonUpdate();
        tongue.ChameleonUpdate();
        anim.ChameleonUpdate();
    }

    public void AddPowerup(PowerupType type)
    {
        power.AddPowerup(type);
    }

    public void ApplyDamage(GameObject source = null)
    {
        if (!isInvincible)
        {
            StartCoroutine(WaitForInvincibilityTime());

            ChameleonBody.Direction direction = ChameleonBody.Direction.None;
            if (source != null)
            {
                direction = source.transform.position.x < Position.x ? ChameleonBody.Direction.Left : ChameleonBody.Direction.Right;
            }
            body.KnockBack(direction);
            anim.OnHurt();
            power.RemovePowerup();
        }
    }

    public void Bounce(Vector2 force)
    {
        body.AddImpulse(force);
        anim.OnJump();
    }

    private IEnumerator WaitForInvincibilityTime()
    {
        isInvincible = true;
        anim.IndicateInvinbility(true);
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
        anim.IndicateInvinbility(false);
    }
}
