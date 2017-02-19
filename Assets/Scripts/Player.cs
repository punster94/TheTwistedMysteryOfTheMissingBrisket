using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public DialogManager dialogManager;
    public GameObject PersonBucket;
    public Vector3[] positionsAtEachTime;

    [Space(20)]
    public float movementSpeed;
    public float turnSpeed = 1;
    public float distanceToInteract;

    public Transform JakeMesh;
    [SerializeField]
    private Transform cameraTransform;

    private bool canMove;
    private bool canInteract;
    private Rigidbody rb;

    private float previousMouseX;

    private int theScreenWidth;
    
    void Start()
    {
        theScreenWidth = Screen.width;

        canMove = true;
        canInteract = true;
        rb = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            interact();
        }

        if (canMove)
            handleMovement();
        else
            rb.velocity = Vector3.zero;

        previousMouseX = Input.mousePosition.x;


    }

    public void doneTalking()
    {
        canMove = true;
        canInteract = true;
    }

    private void handleMovement()
    {
        Vector3 forward = JakeMesh.TransformDirection(Vector3.forward);
        //Debug.DrawRay(rotateMesh.position, forward * 5f, Color.red);

        //Move
        float vAxis = Input.GetAxis("Vertical");
        float hAxis = Input.GetAxis("Horizontal");

        Quaternion targetCameraRotation = cameraTransform.rotation;
        float angle = 0.0f;
        float hAngle = Mathf.Abs(vAxis) > 0.0f ? 45 : 90;
        angle = (vAxis >= 0.0f ? 0 : 180) + Mathf.Sign(vAxis) * (Mathf.Abs(hAxis) > 0.0f ? Mathf.Sign(hAxis) * hAngle : 0);
        
        targetCameraRotation = Quaternion.Euler(0.0f, angle, 0.0f) * targetCameraRotation;

        float deltaAngle = Quaternion.Angle(JakeMesh.rotation, targetCameraRotation);
        float lerpParam = Mathf.Abs(vAxis) > 0.0f && Mathf.Abs(hAxis) > 0.0f ? (Mathf.Abs(vAxis) + Mathf.Abs(hAxis)) / 2.0f : Mathf.Abs(vAxis) + Mathf.Abs(hAxis);
        
        
        JakeMesh.rotation = Quaternion.Lerp(JakeMesh.rotation, targetCameraRotation, lerpParam);
        
        Vector3 targetVel = cameraTransform.forward * vAxis; //Forward/backward movement
        targetVel += cameraTransform.TransformDirection(Vector3.right) * hAxis; //Strafe
        rb.velocity = targetVel * movementSpeed * Time.deltaTime;

        //Rotate
        if (!Cursor.visible) 
        {
            Vector3 rot = cameraTransform.eulerAngles;
            rot.y += turnSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
            cameraTransform.eulerAngles = rot;
        }
        
        /*
         Vector3 rot = rotateMesh.eulerAngles;
        float mouseX = Input.mousePosition.x;
        if (mouseX < 0)
        {
            rot.y -= Time.deltaTime * turnSpeed * 10;
        }
        if (mouseX > theScreenWidth)
        {
            rot.y += Time.deltaTime * turnSpeed * 10;
        }
        else
        {
            rot.y += Time.deltaTime * turnSpeed * (Input.mousePosition.x - previousMouseX);
        }

        rotateMesh.eulerAngles = rot;
         */





        //Previous code:
        //transform.Rotate(Vector3.up, turnSpeed * Input.GetAxis("Horizontal"));


    }

    private void interact()
    {
        Person interactedPerson = selectPersonToInteract();

        if (interactedPerson)
        {
            dialogManager.startTalkingToPerson(interactedPerson);
            canMove = false;
            canInteract = false;
        }
    }

    private Person selectPersonToInteract()
    {
        Person closestPerson = null;
        float currentClosestDistance = Mathf.Infinity;
    
        foreach (Transform child in PersonBucket.transform)
        {
            float distance = Vector3.Distance(child.position, transform.position);

            if (distance < currentClosestDistance && distance <= distanceToInteract)
            {
                closestPerson = child.GetComponent<Person>();
                currentClosestDistance = distance;
            }
        }
        return closestPerson;
    }
}
