using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private LayerMask mask = 0;

    private Vector3 movementVector = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    private RaycastHit hit;
    private GameObject heldObject = null;
    private bool holdingObject = false;

    private float originalDistanceToPlayer = 0f;
    private Vector3 originalScale = Vector3.zero;
    private Collider heldObjectCollider = null;
    private Vector3 extents = Vector3.zero;
    private List<Direction> boundCorners = new List<Direction>();
   
    void Update()
    {
        Movement();
        CameraMovement();
        GrabObject();
        if(holdingObject == true)
        {
            PushObject();
        }

        if(boundCorners.Count > 0)
        {
            foreach(Direction dir in boundCorners)
            {
                
                Debug.DrawRay(transform.position, transform.TransformDirection(dir.vec) * dir.mag, Color.red);
            }
        }
    }

    private void Movement()
    {
        movementVector = new Vector3(Input.GetAxisRaw("Horizontal") , 0f, Input.GetAxisRaw("Vertical"));

        transform.position += transform.rotation * movementVector * Time.deltaTime * movementSpeed;
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
            if(Physics.SphereCast(transform.position, 1f, transform.forward, out hit, Mathf.Infinity, mask))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().useGravity = false;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
                originalDistanceToPlayer = Vector3.Distance(transform.position, heldObject.transform.position);
                originalScale = heldObject.transform.localScale;
                heldObjectCollider = heldObject.GetComponent<Collider>();
                extents = heldObjectCollider.bounds.extents;
                holdingObject = true;

                Vector3 center = heldObjectCollider.bounds.center;

                Vector3 corner;

                corner = center + extents;
                //boundCorners.Add(new Direction((corner - transform.position).normalized, Vector3.Distance(transform.position, corner)));
                boundCorners.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center - extents;
                boundCorners.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(-extents.x, -extents.y, extents.z);
                boundCorners.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(-extents.x, extents.y, extents.z);
                boundCorners.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(-extents.x, extents.y, -extents.z);
                boundCorners.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(extents.x, -extents.y, -extents.z);
                boundCorners.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(extents.x, -extents.y, extents.z);
                boundCorners.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));

                corner = center + new Vector3(extents.x, extents.y, -extents.z);
                boundCorners.Add(new Direction(transform.InverseTransformPoint(corner).normalized, Vector3.Distance(transform.position, corner)));






            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            heldObject.GetComponent<Rigidbody>().useGravity = true;
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject = null;
            holdingObject = false;
            heldObjectCollider = null;
        }
    }

    //private void PushObject()
    //{

    //    if (Physics.Raycast(transform.position ,transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
    //    {
    //        ScaleObject();
    //        Vector3 distanceFromHit = (transform.position - heldObject.transform.position).normalized* ((heldObject.GetComponent<SphereCollider>().radius + 0.1f )* heldObject.transform.localScale.x);
    //        heldObject.transform.position = hit.point + distanceFromHit;
    //    }

    //}

    //private void ScaleObject()
    //{
    //    float scale = Vector3.Distance(heldObject.transform.position, transform.position) / originalDistanceToPlayer * originalScale.x;
    //    Vector3 newScale = new Vector3(scale, scale, scale);

    //    heldObject.transform.localScale = newScale;
    //}
    private void PushObject()
    {
        
    }

    private void ScaleObject()
    {

    }
}
