using UnityEngine;
using System.Collections;
using System;

public class VisualizeDistanceToInteract : MonoBehaviour
{
    public Player PlayerManager;

    void Start ()
    {
        if (PlayerManager == null)
        {
            PlayerManager = GetComponent<Player>();
        }
    }

    void OnDrawGizmosSelected()
    {
        try
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, PlayerManager.distanceToInteract);
        }
        catch (Exception ex) {}
    }
}
