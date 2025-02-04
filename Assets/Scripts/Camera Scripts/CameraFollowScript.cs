using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Vector2 targetOffset;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private float Kp;

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPos = (Vector2)playerObj.transform.position + targetOffset;
        Vector2 CurrentOffset = targetPos - (Vector2)transform.position;
        
        Vector2 direction = CurrentOffset.normalized;
        float distance = CurrentOffset.magnitude;

        transform.position += (Vector3)direction * distance * Kp;

    }
}
