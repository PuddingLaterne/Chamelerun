using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Chameleon : MonoBehaviour 
{
    public float InvincibilityTime  = 0.5f;

    public UnityAction OnPowerChanged = delegate { };
    public UnityAction OnAllPowerLost = delegate { };

    public ChameleonBody Body { get; private set; }
    public ChameleonTongue Tongue { get; private set; }
    public ChameleonAnimation Animation { get; private set; }
    public ChameleonPower Power { get; private set; }

    private List<ChameleonBehaviour> behaviours = new List<ChameleonBehaviour>();
    private bool isInvincible;

    public Transform Transform { get { return Body.transform; } }
    public Vector2 Position { get { return Transform.position; } }

    public PowerupType[] CurrentPowerups { get { return Power.Powerups; } }
    public int CurrentPower { get { return Power.Power; } }

    public void Init()
    {
        Body = GetComponentInChildren<ChameleonBody>();
        Tongue = GetComponentInChildren<ChameleonTongue>();
        Animation = GetComponentInChildren<ChameleonAnimation>();
        Power = GetComponentInChildren<ChameleonPower>();

        behaviours.Add(Body);
        behaviours.Add(Tongue);
        behaviours.Add(Power);
        behaviours.Add(Animation);

        foreach (ChameleonBehaviour behaviour in behaviours)
        {
            behaviour.Init(this);
        }

        Tongue.OnAttached += () => { Body.OnTongueAttached(); };
        Tongue.OnReleased += () => { Body.OnTongueReleased(); };

        Power.OnAllPowerLost += () => { OnAllPowerLost(); };
        Power.OnPowerChanged += () => { OnPowerChanged(); };
    }

    public void Reset()
    {
        StopAllCoroutines();

        isInvincible = false;

        foreach (ChameleonBehaviour behaviour in behaviours)
        {
            behaviour.Reset();
        }
    }

    public void EnableControl(bool enabled)
    {
        foreach (ChameleonBehaviour behaviour in behaviours)
        {
            behaviour.enabled = enabled;
        }
    }

    public void ChameleonUpdate()
    {
        foreach (ChameleonBehaviour behaviour in behaviours)
        {
            behaviour.ChameleonUpdate();
        }
    }

    public void AddPowerup(PowerupType type)
    {
        Power.AddPowerup(type);
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
            Body.KnockBack(direction);

            Power.RemovePowerup();
        }
    }

    public void Bounce(Vector2 force)
    {
        Body.AddImpulse(force);
    }

    private IEnumerator WaitForInvincibilityTime()
    {
        isInvincible = true;
        Animation.IndicateInvinbility(true);
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
        Animation.IndicateInvinbility(false);
    }
}
