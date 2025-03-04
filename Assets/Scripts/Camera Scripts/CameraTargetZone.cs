using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetZone : MonoBehaviour
{
    [SerializeField] private Vector2 offset;
    public Vector2 Offset {get {return offset;}}
    [SerializeField] private bool modifiesEasing;
    public bool ModifiesEasing {get{return modifiesEasing;}}
    public bool ModifiesZoom {get{return zoom > 0.0f;}}
    public float customEasing;
    public float zoom = 0.0f;

    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other) {
        if(other.attachedRigidbody == Player.main.Rb){
            CameraFollowScript.Instance.SetTarget(this);
        } 
    }
    void OnTriggerExit2D(Collider2D other) {
        try{
            if(
                other.attachedRigidbody == Player.main.Rb && 
                CameraFollowScript.Instance.target == transform
            )
            {
                CameraFollowScript.Instance.SetTarget(null);
            }
        }
        catch(NullReferenceException) {}
    }
}
