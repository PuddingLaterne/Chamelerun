using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chameleon : MonoBehaviour 
{
    public float InvincibilityTime  = 0.5f;

    public ChameleonMovement Movement { get; private set; }
    public ChameleonTongue Tongue { get; private set; }
    public ChameleonAnimation Animation { get; private set; }
    public ChameleonPower Power { get; private set; }

    private List<ChameleonBehaviour> behaviours = new List<ChameleonBehaviour>();
    private bool isInvincible;

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

        Tongue.OnAttached += () =>
            {
                Movement.OnTongueAttached();
            };
        Tongue.OnReleased += () =>
            {
                Movement.OnTongueReleased();
            };
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

    public void Update()
    {
        foreach (ChameleonBehaviour behaviour in behaviours)
        {
            behaviour.ChameleonUpdate();
        }
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

    private IEnumerator WaitForInvincibilityTime()
    {
        isInvincible = true;
        Animation.IndicateInvinbility(true);
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
        Animation.IndicateInvinbility(false);
    }
}
