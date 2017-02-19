using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LogLeftPage_Updater : MonoBehaviour
{
    [SerializeField]
    Transform entriesParent;
    [SerializeField]
    GameObject entryPf;
    [SerializeField]
    Text PersonNameText;
    [SerializeField]
    Scrollbar scrollbar;

    public float ScrollbarValue { get { return scrollbar.value; } }

    public void UpdateContent(string personName, List<LogEntry> listOfEntries, float scrollbarValue)
    {
        //Update Person's name (header)
        PersonNameText.text = personName;

        //Update all entries
        RemoveAllEntries();
        foreach (var entry in listOfEntries)
        {
            GameObject newEntry = Instantiate(entryPf, entriesParent) as GameObject;
            newEntry.GetComponent<LogLeftPage_SingleEntry>().InitializeContent(entry, entry.IndexInList);
            newEntry.GetComponent<RectTransform>().localScale = Vector3.one;
            Vector3 pos = newEntry.GetComponent<RectTransform>().localPosition;
            pos.z = 0f;
            newEntry.GetComponent<RectTransform>().localPosition = pos;
        }

        //Update Scrollbar
        scrollbar.value = scrollbarValue;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveAllEntries();
        }
    }

    void RemoveAllEntries()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in entriesParent)
        {
            children.Add(child.gameObject);
        }

        List<GameObject> childrenToDestroy = new List<GameObject>();
        for (int i = 0; i < entriesParent.childCount; i++)
        {
            //Destroy all except the first;
            childrenToDestroy.Add(children[i]);
        }

        childrenToDestroy.ForEach(child => Destroy(child));
    }
}