using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 lookOffset;
    public Vector3 lookAngle = new Vector3(0f, 1f, 1f);
    public float distanceToTarget = 10f;
    public float lerpSpeed = 60f;

    Vector3 dirToMe;

	void Start ()
    {
        dirToMe = (transform.position - target.position).normalized;
    }
	
	void Update ()
    {
        //         transform.position = Vector3.Lerp(transform.position, target.position, lerpSpeed * Time.deltaTime);
        // 
        //         Quaternion rot = transform.rotation;
        //         rot = Quaternion.Lerp(rot, target.rotation, lerpSpeed * Time.deltaTime);
        //         transform.rotation = rot;

        //         Vector3 rot = transform.eulerAngles;
        //         rot.y = Mathf.Lerp(rot.y, target.eulerAngles.y, lerpSpeed * Time.deltaTime);
        //         transform.eulerAngles = rot;


        //Place behind
        //transform.position = target.position + target.TransformDirection(lookAngle * distanceToTarget);
        //Debug.DrawRay(target.position, target.TransformDirection(lookAngle * distanceToTarget), Color.yellow);


        //Rotation
        //transform.LookAt(target.position + lookOffset);
// 
//         //Positioning
//         Vector3 targetPos = target.position + dirToMe * distanceToTarget;
//         transform.position = targetPos;
    }
}
