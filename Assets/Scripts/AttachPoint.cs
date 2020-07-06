using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    // need some way of defining attach direction 

    public GameObject particle = null;

    private GameObject attachedTo = null;
    public GameObject AttachedTo{ get { return attachedTo; } set { attachedTo = value; UpdateParticle(); }}


    private void UpdateParticle()
    {
        if (attachedTo == null)
        {
            particle.SetActive(true);
        }
        else
        {
            particle.SetActive(false);
        }
    }
}
