using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Chameleon : MonoBehaviour 
{
    public float InvincibilityTime  = 0.5f;

    public UnityAction OnPowerChanged = delegate { };
    public UnityAction OnAllPowerLost = delegate { };

    public ChameleonMovement Movement { get; private set; }
    public ChameleonTongue Tongue { get; private set; }
    public ChameleonAnimation Animation { get; private set; }
    public ChameleonPower Power { get; private set; }

    private List<ChameleonBehaviour> behaviours = new List<ChameleonBehaviour>();
    private bool isInvincible;

    public Transform Transform { get { return Movement.transform; } }
    public Vector2 Position { get { return Transform.position; } }

    public PowerupType[] CurrentPowerups { get { return Power.Powerups; } }
    public int CurrentPower { get { return Power.Power; } }

    public void Init()
    {
        Movement = GetComponentInChildren<ChameleonMovement>();
        Tongue = GetComponentInChildren<ChameleonTongue>();
        Animation = GetComponentInChildren<ChameleonAnimation>();
        Power = GetComponentInChildren<ChameleonPower>();

        behaviours.Add(Movement);
        behaviours.Add(Tongue);
        behaviours.Add(Power);
        behaviours.Add(Animation);

        foreach (ChameleonBehaviour behaviour in behaviours)
        {
            behaviour.Init(this);
        }

        Tongue.OnAttached += () => { Movement.OnTongueAttached(); };
        Tongue.OnReleased += () => { Movement.OnTongueReleased(); };

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

    public void ApplyDamage()
    {
        if (!isInvincible)
        {
            StartCoroutine(WaitForInvincibilityTime());
            Movement.KnockBack();
            Power.RemovePowerup();
        }
    }

    public void Bounce(Vector2 force)
    {
        Movement.Throw(force);
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
