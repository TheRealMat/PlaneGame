using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField]
    private Camera theCamera = null;

    private GameObject heldObject = null;

    bool isEditing = true;

    private Vector3 screenPoint;
    private Vector3 offset;

    [SerializeField]
    private float attachDistance = 0;

    public List<GameObject> editorObjects = new List<GameObject>();

    private List<GameObject> attachPointsList = new List<GameObject>();

    Color heldColor = new Color(0.8f, 0.8f, 1.0f, 0.7f);

    // this will need to be changed if glass parts are added
    Color normalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                // check if we are clicking on something
                if (Physics.Raycast(ray, out hitInfo))
                {
                    // copy object if sheft is held
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        CopyObject(hitInfo.collider.transform.root.gameObject);
                    }
                    else
                    {
                        HoldObject(hitInfo.collider.transform.root.gameObject);
                    }

                    screenPoint = Camera.main.WorldToScreenPoint(heldObject.transform.position);
                    offset = heldObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                }
            }

            // put down object again
            else if (heldObject != null)
            {
                PutDown();
            }
        }

        if (isEditing)
        {
            if (heldObject != null)
            {
                MovingObject();
            }
        }
    }

    private void MovingObject()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        heldObject.transform.position = curPosition;

        CheckForAttach(heldObject);
        GeneratePointsList();
        AttachAllPoints();
    }

    private void CopyObject(GameObject obj)
    {
        GameObject objectCopy = Instantiate(obj);
        editorObjects.Add(objectCopy);
        HoldObject(objectCopy);
    }
    private void HoldObject(GameObject obj)
    {
        heldObject = obj;
        // this requires the material to be set to transparent
        heldObject.GetComponent<Renderer>().material.color = heldColor;
    }
    private void PutDown()
    {
        heldObject.GetComponent<Renderer>().material.color = normalColor;
        heldObject = null;
    }


    private float GetScreenDistance(Vector3 point1, Vector3 point2, Camera camera)
    {
        return Vector2.Distance(camera.WorldToScreenPoint(point1), camera.WorldToScreenPoint(point2));
    }

    // i probably shouldn't generate entire list every time
    private void GeneratePointsList()
    {
        // generate list of all attachment points
        attachPointsList.Clear();
        // add all points to list
        foreach (GameObject obj in editorObjects)
        {
            foreach (GameObject point in obj.GetComponent<VehiclePart>().AttachPoints)
            {
                attachPointsList.Add(point);
            }
        }
    }

    private void AttachAllPoints()
    {
        // attach objects together
        foreach (GameObject point in attachPointsList)
        {
            AttachPoint attachPoint = point.GetComponent<AttachPoint>();
            foreach (GameObject otherPoint in attachPointsList)
            {
                AttachPoint OtherAttachPoint = otherPoint.GetComponent<AttachPoint>();
                // see if other point exists at same location
                if (otherPoint != point && point.transform.position == otherPoint.transform.position)
                {
                    attachPoint.AttachedTo = otherPoint;
                    OtherAttachPoint.AttachedTo = point;
                }

                // remove attachment if points do not exist at same location
                if (attachPoint.AttachedTo != null)
                {
                    if(attachPoint.AttachedTo.transform.position != point.transform.position)
                    {
                        attachPoint.AttachedTo = null;
                        OtherAttachPoint.AttachedTo = null;
                    }
                }
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
                        if (GetScreenDistance(point.transform.position, point2.transform.position, theCamera) < attachDistance)
                        {
                            // need to check that it isn't inside object. all objects already have a collider, though maybe i will need to ignore colliders of snapped objects.
                            // needs to be a system to check if snapping points are occupied
                            // probably should have a system of accepting placement on click rather than assuming it's ok
                            // ^ would also allow for canceling movement 


                            // prevents snapping on the inside of points. assumes rotation is consistent. this will probably break if i implement part rotation
                            // yes, this system is crap. replace asap
                            if (point.transform.rotation == Quaternion.Inverse(point2.transform.rotation))
                            {
                                // snap to place
                                currentObj.transform.position = obj.transform.position + point.transform.localPosition - point2.transform.localPosition;
                            }

                        }
                    }
                }
            }
        }
    }
}
