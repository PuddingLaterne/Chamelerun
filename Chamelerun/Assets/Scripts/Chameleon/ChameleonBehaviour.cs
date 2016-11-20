using UnityEngine;
using System.Collections;

public class ChameleonBehaviour : MonoBehaviour 
{
    protected Chameleon chameleon;

    public virtual void Init(Chameleon chameleon)
    {
        this.chameleon = chameleon;
    }

    public virtual void Reset() { }
    public virtual void ChameleonUpdate() { }
}
