using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    // need some way of defining attach direction 

    [SerializeField]
    ParticleSystem particle = null;

    public GameObject attachedTo = null;

    public void ToggleParticles()
    {
        particle.gameObject.SetActive(!particle.gameObject.activeSelf);
    }

}
