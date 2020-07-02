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

    [SerializeField]
    private float attachDistance = 0;

    public List<GameObject> editorObjects = new List<GameObject>()
    {

    };

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

                CheckForAttach(heldObject);
            }
        }
    }

    // i'm not sure how this could be done better
    public void CheckForAttach(GameObject currentObj)
    {
        foreach (GameObject obj in editorObjects)
        {
            if (obj != currentObj)
            {
                foreach (GameObject point in obj.GetComponent<VehiclePart>().AttachPoints)
                {
                    foreach (GameObject point2 in currentObj.GetComponent<VehiclePart>().AttachPoints)
                    {
                        if (Vector3.Distance(obj.transform.position + point.transform.localPosition, currentObj.transform.position + point2.transform.localPosition) < attachDistance)
                        {
                            // need to check that it isn't inside object
                            // some sort of system to connect objects absed on what looks to be close from camera. maybe check in a cylinder from camera to attach things from further away. better system might be projecting to a 2d plane
                            // maybe get all points and select closest
                            // also there needs to be an indication of where the snapping points are
                            // needs to be a system to check if snapping points are occupied

                            currentObj.transform.position = obj.transform.position + point.transform.localPosition -point2.transform.localPosition;
                            return;
                        }
                    }
                }
            }
        }
    }
}
