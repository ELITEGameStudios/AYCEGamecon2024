using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTaggedObject : MonoBehaviour
{
    [SerializeField] private string tagToDisable = "HideOnStart";

    void Awake()
    {
        if (CompareTag(tagToDisable))
        {
            gameObject.SetActive(false);
            Debug.Log(tagToDisable + " was disabled.");
        }
    }
}
