using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePart : MonoBehaviour
{
    public List<GameObject> AttachPoints = new List<GameObject>()
    {

    };

    private void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.gameObject.TryGetComponent<AttachPoint>(out AttachPoint ap))
            {
                AttachPoints.Add(child.gameObject);
            } 
        }
    }
}
