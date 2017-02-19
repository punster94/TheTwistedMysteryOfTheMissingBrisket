using UnityEngine;
using System.Collections;

public class InvTransformUnderstand1 : MonoBehaviour
{
    Transform t;
    Vector3 globalPoint;
    Vector3 localPoint;


    void Start ()
    {
        t = transform;
	}
	
	void Update ()
    {
        //InverseTransformPoint is basically turning an existing world space point, and turning that into a relative position, in the local space. 
        //How would you express a world position in local space? worldPos - transform.position.
        //imaginaryLocalSpacePoint + transform.position    into a world space point, but now also considering the scale and rotation. 
        globalPoint = Vector3.right;
        Vector3 localPoint_WithRotationAndScaleConsideration = transform.InverseTransformPoint(globalPoint);
        localPoint = globalPoint - transform.position ; //The point effectively has no relativity.

        Debug.DrawLine(transform.position, globalPoint, Color.green, 0.1f);
        Debug.DrawLine(Vector3.zero, localPoint_WithRotationAndScaleConsideration, Color.yellow, 0.1f);
        Debug.DrawLine(Vector3.zero, localPoint, Color.blue, 0.1f);
    }

    void OnGUI ()
    {
        GUI.Label(new Rect(20, 20, 200, 20), globalPoint.ToString()); GUI.Label(new Rect(220, 20, 200, 20), t.position.ToString());
        GUI.Label(new Rect(20, 50, 200, 20), localPoint.ToString());
    }
}
