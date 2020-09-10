using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private LayerMask mask = 0;

    private Vector3 movementVector = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;
    private float turnInput = 0.0f;
    private RaycastHit hit;
    private GameObject heldObject = null;
    private bool holdingObject = false;

    private float originalDistanceToPlayer = 0f;
    private Vector3 originalScale = Vector3.zero;
   
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
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().useGravity = false;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
                originalDistanceToPlayer = Vector3.Distance(transform.position, heldObject.transform.position);
                originalScale = heldObject.transform.localScale;
                holdingObject = true;

            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            heldObject.GetComponent<Rigidbody>().useGravity = true;
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject = null;
            holdingObject = false;
        }
    }

    private void PushObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            Vector3 distanceFromHit = hit.normal * heldObject.GetComponent<SphereCollider>().radius * heldObject.transform.localScale.x;
            heldObject.transform.position = hit.point + distanceFromHit;
            ScaleObject();
        }

    }

    private void ScaleObject()
    {
        float scale = Vector3.Distance(heldObject.transform.position, transform.position) / originalDistanceToPlayer * originalScale.x;
        Vector3 newScale = new Vector3(scale, scale, scale);

        heldObject.transform.localScale = newScale;
    }

}
