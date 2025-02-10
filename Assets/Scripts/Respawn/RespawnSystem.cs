using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    [SerializeField] private List<RespawnPoint> respawnPoints;
    public static RespawnSystem Instance {get; private set;}
    [SerializeField] private RespawnPoint nextPoint;



    void Awake(){
        if(Instance == null){ Instance = this;}
        else if(Instance != this){ Destroy(this);}

        respawnPoints = new();
    }

    public void AddRespawnPoint(RespawnPoint point){
        respawnPoints.Add(point);
    }

    public void PingPoint(RespawnPoint point){
        if(!respawnPoints.Contains(point)){AddRespawnPoint(point);}
        nextPoint = point;
    }

    public void RespawnPlayer(){
        Player.main.transform.position = nextPoint.transform.position;
        CameraFollowScript.Instance.transform.position = 
            Player.main.transform.position + 
            (Vector3)CameraFollowScript.Instance.TargetOffset +
            Vector3.forward * CameraFollowScript.Instance.transform.position.z;
    }
}
