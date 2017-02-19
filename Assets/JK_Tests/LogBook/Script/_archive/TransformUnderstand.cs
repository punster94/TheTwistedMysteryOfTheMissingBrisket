using UnityEngine;
using System.Collections;

public class TransformUnderstand : MonoBehaviour
{
    Transform t;
    Vector3 imaginaryLocalRelPoint;
    Vector3 localRight2;


    void Start ()
    {
        t = transform;
	}
	
	void Update ()
    {
        //TransformPoint is basically getting    LocalSpacePoint + transform.position    (but with consideration for scale and rotation) where 
        //the constant knowledge of a local space point (a relative position a Vector3), is being expressed in the form of world space Vector3. 
        imaginaryLocalRelPoint = Vector3.right;
        Vector3 localRightWithRotataionAndScaleConsideration = transform.TransformPoint(imaginaryLocalRelPoint);
        localRight2 = imaginaryLocalRelPoint + transform.position ;

        Debug.DrawLine(transform.position, imaginaryLocalRelPoint, Color.green, 0.1f);
        Debug.DrawLine(transform.position, localRightWithRotataionAndScaleConsideration, Color.yellow, 0.1f);
        Debug.DrawLine(transform.position, localRight2, Color.blue, 0.1f);
    }

    void OnGUI ()
    {
        GUI.Label(new Rect(20, 20, 200, 20), imaginaryLocalRelPoint.ToString()); GUI.Label(new Rect(220, 20, 200, 20), t.position.ToString());
        GUI.Label(new Rect(20, 50, 200, 20), localRight2.ToString());
    }
}
