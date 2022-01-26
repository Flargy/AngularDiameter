using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private LayerMask pickupMask = 0;
    [SerializeField] private LayerMask pushingMask = 0;

    private Vector3 movementVector = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;
    private Vector3 objectCenter = Vector3.zero;

    private RaycastHit hit;
    private GameObject heldObject = null;
    private bool holdingObject = false;

    private float originalDistanceToPlayer = 0f;
    private Vector3 originalScale = Vector3.zero;
    private Collider heldObjectCollider = null;
    private Vector3 extents = Vector3.zero;
    private List<Direction> raycastPoints = new List<Direction>();
   
    void Update()
    {
        Movement();
        CameraMovement();
        GrabObject();
        if(holdingObject == true)
        {
            PushObject();
        }

    }

    private void Movement()
    {
        Transform tr = transform;
        movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Jump"), Input.GetAxisRaw("Vertical")) ;
        tr.position += tr.rotation * movementVector * (Time.deltaTime * movementSpeed);
    }

    private void CameraMovement()
    {
        cameraRotation = new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0f);
        transform.eulerAngles += cameraRotation;
    }

    private void GrabObject()
    {
        if (Input.GetKeyDown(KeyCode.E) && holdingObject == false)
        {
            if(Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, Mathf.Infinity, pickupMask))
            {
                // saves values and changes collision on held object
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                heldObject = hit.collider.gameObject;
                heldObject.layer = 9;
                Rigidbody objRigidbody = heldObject.GetComponent<Rigidbody>();
                objRigidbody.useGravity = false;
                objRigidbody.isKinematic = true;
                originalDistanceToPlayer = Vector3.Distance(transform.position, heldObject.transform.position);
                originalScale = heldObject.transform.localScale;
                heldObjectCollider = heldObject.GetComponent<Collider>();
                extents = heldObjectCollider.bounds.extents;
                heldObjectCollider.enabled = false;
                holdingObject = true;

                Vector3 center = heldObjectCollider.bounds.center;

                Vector3 corner;
                
                // saves a list of points to encapsulate the held object for raycasts

                corner = center + extents;
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center - extents;
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(-extents.x, -extents.y, extents.z);
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(-extents.x, extents.y, extents.z);
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(-extents.x, extents.y, -extents.z);
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(extents.x, -extents.y, -extents.z);
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(extents.x, -extents.y, extents.z);
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(extents.x, extents.y, -extents.z);
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));
                
                raycastPoints.Add(new Direction(transform.InverseTransformPoint(center).normalized, Vector3.Distance(transform.position, center)));

                objectCenter = transform.InverseTransformPoint(center).normalized;

            }
        }
        // drops the object if held
        else if (Input.GetKeyDown(KeyCode.E))
        {
            heldObject.layer = 8;
            Rigidbody heldRigidbody = heldObject.GetComponent<Rigidbody>();
            heldObject.GetComponent<Collider>().enabled = true;
            heldRigidbody.useGravity = true;
            heldRigidbody.isKinematic = false;
            heldObject = null;
            holdingObject = false;
            heldObjectCollider = null;
            raycastPoints.Clear();
        }
    }

    private void PushObject()
    {

        float lowestDistance = Mathf.Infinity;
        float distanceFromPlayer = 0f;
        Vector3 shortestPath = Vector3.zero;
        
        foreach (Direction dir in raycastPoints)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(dir.vec), out hit, Mathf.Infinity, pushingMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(dir.vec) * hit.distance, Color.yellow);

                if (hit.distance - dir.mag < lowestDistance)
                {
                    lowestDistance = hit.distance - dir.mag;
                    distanceFromPlayer = hit.distance - heldObject.transform.localScale.x / 2;
                    shortestPath = transform.TransformDirection(dir.vec) * hit.distance;
                }
            }
        }

        ScaleObject(distanceFromPlayer);

        Debug.DrawRay(transform.position, shortestPath, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(objectCenter) * 10, Color.blue);
        
        heldObject.transform.position = transform.position + transform.TransformDirection(objectCenter) * (distanceFromPlayer);


    }

    private void ScaleObject(float distance)
    {
        float scale = distance / originalDistanceToPlayer * originalScale.x;
        Vector3 newScale = new Vector3(scale, scale, scale);

        heldObject.transform.localScale = newScale;
    }
}
