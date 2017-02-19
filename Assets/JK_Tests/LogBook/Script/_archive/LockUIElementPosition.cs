using UnityEngine;
using System.Collections;

public class LockUIElementPosition : MonoBehaviour {
    private Vector3 startPosition;
    void Awake()
    {
        startPosition = this.gameObject.GetComponent<RectTransform>().position;
    }

    void Update()
    {
        gameObject.GetComponent<RectTransform>().position = startPosition;
    }
}
