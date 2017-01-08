using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ChameleonAnimation : MonoBehaviour 
{
    public float ScaleVariationSpeed = 0.1f;
    public float ScaleVariation = 1.1f; 

    public GameObject DirectionIndicator;
    public GameObject Model;

    public float AngleThreshold;

    private ChameleonBody body;
    private ChameleonTongue tongue;
    private Animator anim;
    private Vector3 baseSize;

    public void Init(ChameleonBody body, ChameleonTongue tongue)
    {
        this.body = body;
        this.tongue = tongue;
        anim = Model.GetComponent<Animator>();
        baseSize = Model.transform.localScale;
    }

    public void Reset()
    {
        StopAllCoroutines();
        Model.transform.DOKill();
        Model.transform.localScale = baseSize;
        anim.SetBool("dead", false);
    }

    public void ChameleonUpdate()
    {
        float angle = InputHelper.AimingAngle;

        DirectionIndicator.transform.eulerAngles = new Vector3(0, 0, angle);

        if(body.CurrentDirection == ChameleonBody.Direction.Left)
        {
            Model.transform.localScale = new Vector3(-Mathf.Abs(Model.transform.localScale.x), Model.transform.localScale.y, Model.transform.localScale.z);
        }
        else if(body.CurrentDirection == ChameleonBody.Direction.Right)
        {
            Model.transform.localScale = new Vector3(Mathf.Abs(Model.transform.localScale.x), Model.transform.localScale.y, Model.transform.localScale.z);
        }

        anim.SetFloat("horizontalInput", Mathf.Abs(InputHelper.HorizontalInput));
        anim.SetFloat("velocityY", body.VelocityY);
        anim.SetFloat("velocityX", Mathf.Abs(body.VelocityX) > 0 ? Mathf.Abs(body.VelocityX) * 0.2f : 1);
        anim.SetBool("tongueActive", tongue.StaticTongue.activeInHierarchy || tongue.DynamicTongue.activeInHierarchy);
        anim.SetBool("grounded", body.IsGrounded);
    }

    public void OnPowerupCollected()
    {
        anim.SetTrigger("powerup");
    }

    public void OnJump()
    {
        anim.SetTrigger("jump");
    }

    public void OnTongueExpanded()
    {
        anim.SetTrigger("tongueExpand");
    }

    public void OnTongueAttached()
    {
        anim.SetBool("tongueAttached", true);
    }

    public void OnTongueReleased()
    {
        anim.SetBool("tongueAttached", false);
    }

    public void OnDead()
    {
        anim.SetBool("dead", true);
        anim.SetTrigger("hurt");
    }

    public void OnHurt()
    {
        StopAllCoroutines();
        anim.SetTrigger("hurt");
        anim.SetBool("hurtActive", true);
        StartCoroutine(SetHurtFlagInactive());
    }

    public void IndicateInvinbility(bool invincible)
    {
        if (invincible)
        {
            //StartCoroutine(VaryScale());
        }
        else
        {
            StopAllCoroutines();
            Model.transform.DOKill();
            Model.transform.localScale = baseSize;
        }
    }

    private IEnumerator SetHurtFlagInactive()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("hurtActive", false);
    }
	
}
