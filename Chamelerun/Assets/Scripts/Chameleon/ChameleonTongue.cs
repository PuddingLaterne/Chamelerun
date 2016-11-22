using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ChameleonTongue : ChameleonBehaviour 
{
    private struct TongueSegment
    {
        public HingeJoint2D Joint;
        public Rigidbody2D Rigidbody;
        public BoxCollider2D Collider;
        public Transform Sprite;
        public Transform Transform;
    }

    private enum AttachmentState
    {
        None, Attached, Collecting, Damaged, Failed
    }

    [Header("Interaction Layers")]
    public LayerMask AttachmentLayers;
    public LayerMask CollectableLayers;
    public LayerMask PunchableLayers;
    public LayerMask DamagingLayers;

    [Header("Tongue")]
    public int NumSegments;
    public Vector2 OffsetOnBody;
    public float MinLength;
    public float TongueWidth = 0.5f;
    public int NumRaycasts = 3;

    [Header("Speed")]
    public float ManualLengthVariantSpeed;
    public float RetractionSpeed;
    public float ExpansionSpeed;

    [Header("References")]
    public GameObject DynamicTongue;
    public GameObject StaticTongue;
    public GameObject Beginning;
    public HingeJoint2D End;
    public GameObject SegmentPrefab;

    public UnityAction OnAttached = delegate { };
    public UnityAction OnReleased = delegate { };

    private LayerMask tongueInteractionLayers;

    private HingeJoint2D firstJoint;
    private DistanceJoint2D distanceJoint;
    private float endPointRadius;

    private TongueSegment[] tongueSegments;
    private float currentSegmentLength;
    private float currentTongueLength;
    private float currentAngle;
    private Vector2 currentOrigin;

    public bool IsAttached { get; private set; }
    private bool isExpanding;
    private bool isRetracting;

    private GameObject attachedObject;

    public override void Init(Chameleon chameleon)
    {
        base.Init(chameleon);

        tongueInteractionLayers = AttachmentLayers | CollectableLayers | DamagingLayers | PunchableLayers;

        endPointRadius = End.GetComponent<CircleCollider2D>().radius;
        currentSegmentLength = (MinLength - endPointRadius * 2f) / NumSegments;
        currentTongueLength = MinLength;

        CreateTongueSegments();
        
        firstJoint = Beginning.GetComponent<HingeJoint2D>();
        firstJoint.connectedBody = tongueSegments[0].Rigidbody;
        firstJoint.connectedAnchor = new Vector2(0, -currentSegmentLength / 2f);

        distanceJoint = Beginning.GetComponent<DistanceJoint2D>();
        distanceJoint.distance = 0;
    }

    private void CreateTongueSegments()
    {
        tongueSegments = new TongueSegment[NumSegments];
        for (int i = 0; i < NumSegments; i++)
        {
            GameObject tongueSegmentObject = Instantiate(SegmentPrefab);

            tongueSegmentObject.transform.SetParent(DynamicTongue.transform);
            tongueSegmentObject.transform.localPosition = Vector3.zero;
            tongueSegmentObject.transform.localEulerAngles = Vector3.zero;
            tongueSegmentObject.transform.localScale = Vector3.one;

            TongueSegment tongueSegment = new TongueSegment();
            tongueSegment.Joint = tongueSegmentObject.GetComponent<HingeJoint2D>();
            tongueSegment.Rigidbody = tongueSegmentObject.GetComponent<Rigidbody2D>();
            tongueSegment.Collider = tongueSegmentObject.GetComponent<BoxCollider2D>();
            tongueSegment.Sprite = tongueSegmentObject.transform.GetChild(0);
            tongueSegment.Transform = tongueSegmentObject.transform;

            tongueSegment.Collider.size = new Vector2(tongueSegment.Collider.size.x, currentSegmentLength);
            tongueSegment.Sprite.localScale = new Vector3(tongueSegment.Sprite.localScale.x, currentSegmentLength, 1);

            tongueSegments[i] = tongueSegment;
        }
        for (int i = 0; i < NumSegments - 1; i++)
        {
            tongueSegments[i].Joint.connectedBody = tongueSegments[i + 1].Rigidbody;
            tongueSegments[i].Joint.anchor = new Vector2(0, currentSegmentLength / 2f);
            tongueSegments[i].Joint.connectedAnchor = new Vector2(0, -currentSegmentLength / 2f);
        }
        tongueSegments[NumSegments - 1].Joint.connectedBody = End.GetComponent<Rigidbody2D>();
        tongueSegments[NumSegments - 1].Joint.anchor = new Vector2(0, currentSegmentLength / 2f);
    }

    public override void Reset()
    {
        StopAllCoroutines();

        AdjustTongueLength(MinLength);

        IsAttached = false;
        isExpanding = false;
        isRetracting = false;

        DynamicTongue.SetActive(false);
        StaticTongue.SetActive(false);

        attachedObject = null;
    }

    public override void ChameleonUpdate()
    {
        currentOrigin = chameleon.Position + OffsetOnBody;

        if(InputHelper.TongueInput && !isExpanding && !isRetracting)
        {
            if(IsAttached)
            {
                Release();
                StartCoroutine(Retract());  
            }
            else
            {
                StartCoroutine(Expand());             
            }
        }

        if(IsAttached)
        {
            float lengthChange = InputHelper.VerticalInput * ManualLengthVariantSpeed * Time.deltaTime;
            float targetTongueLength = Mathf.Clamp(currentTongueLength - lengthChange, MinLength, chameleon.Power.GetMaxTongueLength());
               
            if (targetTongueLength != currentTongueLength)
            {
                AdjustTongueLength(targetTongueLength);
            }
            
            if(InputHelper.JumpPressed)
            {
                Release();
                StartCoroutine(Retract());  
            }
        }
    }

    private IEnumerator Expand()
    {
        isExpanding = true;
        DynamicTongue.SetActive(false);
        StaticTongue.SetActive(true);

        float lengthDifference, newLength, maxLength;      
        AttachmentState attachmentState = AttachmentState.None;

        while(isExpanding || isRetracting)
        {
            currentAngle = InputHelper.AimingAngle;
            StaticTongue.transform.eulerAngles = new Vector3(0, 0, currentAngle);
            StaticTongue.transform.position = currentOrigin;

            maxLength = chameleon.Power.GetMaxTongueLength();
            lengthDifference = (isExpanding ? ExpansionSpeed : -RetractionSpeed) * Time.fixedDeltaTime;
            newLength = Mathf.Clamp(currentTongueLength + lengthDifference, MinLength, maxLength);

            AdjustTongueLength(newLength);

            if (isExpanding && newLength == maxLength)
            {
                isExpanding = false;
                isRetracting = true;

                attachmentState = AttachmentState.Failed;
            }

            if(isRetracting && newLength == MinLength)
            {
                isRetracting = false;
            }

            if (attachmentState == AttachmentState.None)
            {
                attachmentState = TryAttaching();
                switch (attachmentState)
                {
                    case AttachmentState.Attached:
                        isExpanding = false;
                        DynamicTongue.SetActive(true);
                        break;
                    case AttachmentState.Damaged:
                        isExpanding = false;
                        isRetracting = true;
                        chameleon.ApplyDamage();
                        break;
                    case AttachmentState.Collecting:
                        isExpanding = false;
                        isRetracting = true;
                        break;
                    case AttachmentState.Failed:
                        isExpanding = false;
                        isRetracting = true;
                        break;
                }
            }
            AdjustAttachedObjectPosition();

            yield return new WaitForFixedUpdate();
        }

        StaticTongue.SetActive(false);
        attachedObject = null;
    }

    private IEnumerator Retract()
    {
        isRetracting = true;
        bool targetLengthReached = false;
        while (!targetLengthReached)
        {
            float newLength = currentTongueLength - RetractionSpeed * Time.fixedDeltaTime;
            if (newLength <= MinLength)
            {
                newLength = MinLength;
                targetLengthReached = true;
            }
            AdjustTongueLength(newLength);
            yield return new WaitForFixedUpdate();
        }
        isRetracting = false;
        DynamicTongue.SetActive(false);
        StaticTongue.SetActive(false);
    }

    private AttachmentState TryAttaching()
    {
        Vector2 direction = Vector2.up.Rotate(currentAngle);
        Vector2 offsetDirection = direction.Rotate(90);

        bool numRaycastsIsEven = NumRaycasts % 2 == 0;
        float rayCastOffset = TongueWidth / (numRaycastsIsEven ? NumRaycasts : NumRaycasts - 1);
        RaycastHit2D hit;
        int i = numRaycastsIsEven ? 1 : 0;
        while(i <= (numRaycastsIsEven ? (NumRaycasts / 2) : ((NumRaycasts - 1) / 2)))
        {
            hit = Physics2D.Raycast(currentOrigin, direction + offsetDirection * rayCastOffset * i, currentTongueLength, tongueInteractionLayers);
            if (hit)
            {
                int layer = hit.transform.gameObject.layer.ToBitmask();
                if (layer.IsPartOfBitmask(AttachmentLayers))
                {
                    Attach(hit, direction);
                    return AttachmentState.Attached;
                }
                if (layer.IsPartOfBitmask(CollectableLayers))
                {
                    attachedObject = hit.transform.gameObject;
                    return AttachmentState.Collecting;
                }
                if (layer.IsPartOfBitmask(DamagingLayers))
                {
                    return AttachmentState.Damaged;
                }
                if (layer.IsPartOfBitmask(PunchableLayers))
                {
                    Punch(hit, direction);
                    return AttachmentState.Failed;
                }
            }
            if (rayCastOffset < 0 || i == 0) i++;
            if (i != 0) rayCastOffset *= -1;
        }

        return AttachmentState.None;
    }

    private void Attach(RaycastHit2D hit, Vector2 direction)
    {
        AdjustTongueLength(hit.distance);

        Beginning.transform.position = (currentOrigin);
        Beginning.transform.eulerAngles = new Vector3(0, 0, currentAngle);
        for (int i = 0; i < NumSegments; i++)
        {
            tongueSegments[i].Transform.position = currentOrigin + (endPointRadius + currentSegmentLength * (i + 0.5f)) * direction;
            tongueSegments[i].Transform.eulerAngles = new Vector3(0, 0, currentAngle);
        }
        End.transform.position = hit.point;
        End.transform.eulerAngles = new Vector3(0, 0, currentAngle);

        End.connectedBody = hit.rigidbody;
        End.connectedAnchor = hit.rigidbody.transform.InverseTransformPoint(hit.point);
        End.enabled = true;
        
        IsAttached = true;
        OnAttached();
    }

    private void AdjustAttachedObjectPosition()
    {
        if (attachedObject != null)
        {
            attachedObject.transform.position = currentOrigin + Vector2.up.Rotate(currentAngle) * currentTongueLength;
        }
    }

    private void Release()
    {
        End.enabled = false;
        End.connectedBody = null;
        
        IsAttached = false;
        OnReleased();
    }

    private void AdjustTongueLength(float newLength)
    {
        currentTongueLength = newLength;
        currentSegmentLength = (newLength - endPointRadius * 2f) / NumSegments;

        distanceJoint.distance = newLength;
        firstJoint.connectedAnchor = new Vector2(0, -currentSegmentLength / 2f);

        for (int i = 0; i < NumSegments; i++)
        {
            tongueSegments[i].Collider.size = new Vector2(tongueSegments[i].Collider.size.x, currentSegmentLength);
            tongueSegments[i].Sprite.localScale = new Vector3(tongueSegments[i].Sprite.localScale.x, currentSegmentLength, 1);

            tongueSegments[i].Joint.anchor = new Vector2(0, currentSegmentLength / 2f);
            if (i != NumSegments - 1)
            {
                tongueSegments[i].Joint.connectedAnchor = new Vector2(0, -currentSegmentLength / 2f);
            }
        }

        StaticTongue.transform.localScale = new Vector3(1, currentTongueLength, 1);
    }

    private void Punch(RaycastHit2D hit, Vector2 direction)
    {
        Enemy hitEnemy = hit.transform.GetComponentInChildren<Enemy>();
        if (hitEnemy != null)
        {
            hitEnemy.Punch(direction);
        }
    }
}
