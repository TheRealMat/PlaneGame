using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField]
    private Camera theCamea = null;

    private GameObject heldObject = null;

    bool isEditing = true;

    private Vector3 screenPoint;
    private Vector3 offset;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // create new object if shift is held


            if (heldObject == null)
            {
                Ray ray = theCamea.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                // check if we are clicking on something
                if (Physics.Raycast(ray, out hitInfo))
                {
                    // need smart way to do this instead
                    try
                    {
                        heldObject = hitInfo.collider.transform.parent.gameObject;
                    }
                    catch
                    {
                        heldObject = hitInfo.collider.transform.gameObject;
                    }

                    screenPoint = Camera.main.WorldToScreenPoint(heldObject.transform.position);
                    offset = heldObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                }
            }
            else if (heldObject != null)
            {
                heldObject = null;
            }
        }

        if (isEditing)
        {
            if (heldObject != null)
            {
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
                heldObject.transform.position = curPosition;


            }
        }
    }
}
