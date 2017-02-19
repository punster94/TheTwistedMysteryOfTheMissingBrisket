using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PingString : MonoBehaviour
{
    [SerializeField] Transform firstPin;
    [SerializeField] Transform secondPin;
    [SerializeField] RectTransform inBtwString;
    [SerializeField] float stringlengthMod = 1f;

    LogManager logManager;

    Transform activePin;
    Transform nonActivePin;
    bool inPlacement = false;
    bool inMoving = false;
    int colorIndex = 0;
    
    //Moving
    public bool TryMovePin(bool isfirstPinActive)
    {
        if (logManager.CursorMode == LogManager.DetectiveCursorMode.Idle)
        {
            logManager.MovePin(this);
            if (isfirstPinActive)
            {
                activePin = firstPin;
                nonActivePin = secondPin;
            }
            else
            {
                activePin = secondPin;
                nonActivePin = firstPin;
            }

            inMoving = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FinishMoving()
    {
        UpdateInbetweenString();
        activePin.GetComponent< PinStringComponent>().FinishedMoving();
        activePin = null;
    }

    //Placement
    public void BeginPlacement(LogManager log)
    {
        inPlacement = true;
        logManager = log;
        activePin = secondPin;
        nonActivePin = firstPin;
    }

    public void FinishPlacement()
    {
        inPlacement = false;
        UpdateInbetweenString();
        activePin.position = Input.mousePosition;
    }

    public void DestroyAll ()
    {
        DestroyImmediate(firstPin.gameObject);
        DestroyImmediate(secondPin.gameObject);
        DestroyImmediate(inBtwString.gameObject);
        DestroyImmediate(gameObject.gameObject);
    }

    public void ToggleColor ()
    {
        colorIndex++;

        int finalIndex = colorIndex % LogManager.ToggleColors.Length;
        Color color = LogManager.ToggleColors[finalIndex];
        firstPin.GetComponent<Image>().color = color;
        secondPin.GetComponent<Image>().color = color;
        inBtwString.gameObject.GetComponent<Image>().color = color;
    }	

	void Update ()
    {
        if (inPlacement || (inMoving && activePin != null))
        {
            UpdateInbetweenString();
        }
	}

    void UpdateInbetweenString()
    {
        //Set position
        inBtwString.position = nonActivePin.transform.position;
        activePin.position = Input.mousePosition;

        //Length
        float pinDist = Vector3.Distance(nonActivePin.transform.position, Input.mousePosition);
        Vector2 stringWidthHeight = inBtwString.sizeDelta;
        stringWidthHeight.x = pinDist * stringlengthMod;
        inBtwString.sizeDelta = stringWidthHeight;

        //Rotation
        Vector3 startPos = nonActivePin.transform.position;

        float angle = Vector3.Angle(Vector3.right, Input.mousePosition - startPos);
        if ((Input.mousePosition - startPos).y < 0)
        {
            angle = angle + (180f - angle) * 2;
        }

        /*Vector3 tgtEuler = Quaternion.Euler(0f, 0f, angle) * Vector3.right;*/
        Vector3 tgtEuler = new Vector3(0f, 0f, angle);
        inBtwString.eulerAngles = tgtEuler;
        //print("StartAngle: " + startPos + ", mousePos: "  + Input.mousePosition + ", Angle: " + angle + ", tgtEuler: " + tgtEuler);
    }
}
