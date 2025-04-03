using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Vector2 defaultTargetOffset, currentTargetOffset, offsetInTargetMode;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private Camera cam;
    [SerializeField] private float Kp, currentKp, zoomKp, currentZoomKp, default_zoom, target_zoom;
    public Transform target {get; private set;}
    public bool targetObjMode {get {return target != null;}}
    public static CameraFollowScript Instance {get; private set;}
    public Vector2 TargetOffset {get {return currentTargetOffset;}}

    [Header("For camera shake debug")]
    [SerializeField] private int frequency; 
    [SerializeField] private int shakeTime; 
    [SerializeField] private float shakeTimer, intensity; 
    public bool isShaking {get {return shakeTimer > 0;}}


    void Awake(){
        if(Instance == null) Instance = this;
        else if(Instance != this) Destroy(this);

        // if(cam == null){cam = transform.GetChild(0).GetComponent<Camera>();}
        target = null;
        currentKp = Kp;
        currentZoomKp = zoomKp;
        default_zoom = cam.orthographicSize;
        target_zoom = default_zoom;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if(playerObj == null){
            playerObj = Player.main.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Debug.Log(target_zoom + " zoom");

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

        if(!Player.main.dead){
            transform.position += (Vector3)direction * distance * currentKp;
        }
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, target_zoom, currentZoomKp);
        
        // For screen shake
        if(isShaking){
            float shakeX = 0.5f * Mathf.Sin(2*Mathf.PI * (frequency*(shakeTimer/shakeTime) * shakeTimer - 0.25f)) + 0.5f;
            cam.transform.localPosition = Vector3.back + Vector3.right * shakeX ;
            shakeTimer -= Time.deltaTime;
        }
        else{
            cam.transform.localPosition = Vector3.back;
        }

    }

    public void SetTarget(CameraTargetZone zone){
        if(zone == null){
            target = null;
            offsetInTargetMode = Vector2.zero;
            currentKp = Kp;
            target_zoom = default_zoom;
            return;
        }

        target = zone.transform;
        offsetInTargetMode = zone.Offset;
        currentKp = zone.ModifiesEasing ? zone.customEasing : Kp;
        target_zoom = zone.ModifiesZoom ? zone.zoom : default_zoom;
    }

    public void Shake(int frequency, float intensity, int time){
        shakeTime = time;
        shakeTimer = time;
        this.frequency = frequency;
        this.intensity = intensity;
    }
}
