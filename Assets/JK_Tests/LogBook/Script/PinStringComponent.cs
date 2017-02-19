using UnityEngine;
using System.Collections;

public class PinStringComponent : MonoBehaviour
{
    [SerializeField] PingString pinGrp;
    [SerializeField] bool isfirstPin;

    bool inMoving = false;

    public void Clicked ()
    {
        if (pinGrp.TryMovePin(isfirstPin))
        {            
            inMoving = true;
        }
    }

    public void FinishedMoving ()
    {
        inMoving = false;
    }
	
	void Update ()
    {
	    if (inMoving)
        {
            transform.position = Input.mousePosition;
        }
	}
}