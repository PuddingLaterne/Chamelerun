using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ChameleonAnimation : ChameleonBehaviour 
{
    public float ScaleVariationSpeed = 0.1f;
    public float ScaleVariation = 1.1f; 

    public GameObject DirectionIndicator;

    [Header("Head")]
    public GameObject Head;
    public GameObject HeadLeft;
    public GameObject HeadCenter;
    public GameObject HeadRight;
    public float AngleThreshold;

    [Header("Body")]
    public GameObject BodyLeft;
    public GameObject BodyRight;

    public override void Reset()
    {
        StopAllCoroutines();
        transform.DOKill();
        transform.localScale = Vector3.one;
    }

    public override void ChameleonUpdate()
    {
        float angle = InputHelper.AimingAngle;

        DirectionIndicator.transform.eulerAngles = new Vector3(0, 0, angle);

        HeadLeft.SetActive(false);
        HeadCenter.SetActive(false);
        HeadRight.SetActive(false);

        if(angle > AngleThreshold && angle < 180 - AngleThreshold)
        {
            angle = angle - 90;
            HeadLeft.SetActive(true);
        }
        else if(angle < 360 - AngleThreshold && angle > 180 + AngleThreshold)
        {
            angle = angle + 90;
            HeadRight.SetActive(true);
        }
        else
        {
            if(angle > 180 - AngleThreshold && angle < 180 + AngleThreshold)
            {
                angle = angle + 180;
            }
            HeadCenter.SetActive(true);
        }
        Head.transform.localEulerAngles = new Vector3(0, 0, angle);

        if (chameleon.Movement.CurrentDirection == ChameleonMovement.Direction.Left)
        {
            BodyLeft.SetActive(true);
            BodyRight.SetActive(false);
        }
        if (chameleon.Movement.CurrentDirection == ChameleonMovement.Direction.Right)
        {
            BodyLeft.SetActive(false);
            BodyRight.SetActive(true);
        }
    }

    public void IndicateInvinbility(bool invincible)
    {
        if (invincible)
        {
            StartCoroutine(VaryScale());
        }
        else
        {
            StopAllCoroutines();
            transform.DOKill();
            transform.localScale = Vector3.one;
        }
    }

    private IEnumerator VaryScale()
    {
        int iterations = 0;
        while (true)
        {
            transform.DOScale(iterations % 2 == 0 ? ScaleVariation : 1f, ScaleVariationSpeed);
            yield return new WaitForSeconds(ScaleVariationSpeed);
            iterations++;
        }
    }
	
}
