using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour
{
    public int personId;
    public string personName;

    public Sprite portrait;

    public Vector3[] positionsAtEachTime;

    void Start ()
    {
	    
	}
	
	void Update ()
    {
	
	}

    public void interact()
    {
        print("You found " + personName + "!");
    }
}
