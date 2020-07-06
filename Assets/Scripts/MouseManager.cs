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


    Color heldColor = new Color(0.8f, 0.8f, 1.0f, 0.7f);
    Color normalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (heldObject == null)
            {
                Ray ray = theCamea.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                // check if we are clicking on something
                if (Physics.Raycast(ray, out hitInfo))
                {
                    // copy object if sheft is held
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        // should check for parent here
                        GameObject objectCopy = Instantiate(hitInfo.collider.transform.root.gameObject);
                        editorObjects.Add(objectCopy);
                        heldObject = objectCopy;
                    }
                    else
                    {
                        heldObject = hitInfo.collider.transform.root.gameObject;
                        heldObject.GetComponent<Renderer>().material.color = heldColor;
                    }

                    screenPoint = Camera.main.WorldToScreenPoint(heldObject.transform.position);
                    offset = heldObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                }
            }

            // put down object again
            else if (heldObject != null)
            {
                heldObject.GetComponent<Renderer>().material.color = normalColor;
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
                        if (Vector2.Distance(theCamea.WorldToScreenPoint(point.transform.position), theCamea.WorldToScreenPoint(point2.transform.position)) < attachDistance)
                        {
                            // need to check that it isn't inside object. all objects already have a collider, though maybe i will need to ignore colliders of snapped objects.
                            // needs to be a system to check if snapping points are occupied
                            // this does not support being snapped to multiple objects
                            // probably should have a system of accepting placement on click rather than assuming it's ok
                            // ^ would also allow for canceling movement 


                            // prevents snapping on the inside of points. assumes rotation is consistent. this will probably break if i implement part rotation
                            // yes, this system is crap. replace asap
                            if (point.transform.rotation == Quaternion.Inverse(point2.transform.rotation))
                            {
                                // snap to place
                                currentObj.transform.position = obj.transform.position + point.transform.localPosition - point2.transform.localPosition;

                                //getting component should be done somewhere else
                                point.GetComponent<AttachPoint>().attachedTo = point2;
                                point2.GetComponent<AttachPoint>().attachedTo = point;
                                return;
                            }

                        }
                        point.GetComponent<AttachPoint>().attachedTo = null;
                        point2.GetComponent<AttachPoint>().attachedTo = null;
                    }
                }
            }
        }
    }
}
