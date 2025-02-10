using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Vector2 defaultTargetOffset, currentTargetOffset, offsetInTargetMode;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private float Kp, currentKp;
    public Transform target {get; private set;}
    public bool targetObjMode {get {return target != null;}}
    public static CameraFollowScript Instance {get; private set;}
    public Vector2 TargetOffset {get {return currentTargetOffset;}}

    void Awake(){
        if(Instance == null) Instance = this;
        else if(Instance != this) Destroy(this);
        target = null;
        currentKp = Kp;
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 targetPos = 
            targetObjMode ? 
            (Vector2) target.position + offsetInTargetMode : 
            (Vector2) playerObj.transform.position + currentTargetOffset;

        Vector2 CurrentOffset = targetPos - (Vector2)transform.position;
        Vector2 direction = CurrentOffset.normalized;
        float distance = CurrentOffset.magnitude;

        // For player tracking only
        currentTargetOffset = defaultTargetOffset;
        currentTargetOffset.x *= Player.main.Movement.flipDirRaw;

        transform.position += (Vector3)direction * distance * currentKp;
    }

    public void SetTarget(CameraTargetZone zone){
        if(zone == null){
            target = null;
            offsetInTargetMode = Vector2.zero;
            currentKp = Kp;
            return;
        }

        target = zone.transform;
        offsetInTargetMode = zone.Offset;
        currentKp = zone.ModifiesEasing ? zone.customEasing : Kp;
    }
}
