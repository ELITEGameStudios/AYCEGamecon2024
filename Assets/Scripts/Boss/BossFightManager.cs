using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    // This manager object will be positioned where metal boxes should spawn
    // Collider will be the zone which the bossfight commences
    public static BossFightManager Instance { get; private set; }
    [SerializeField] private GameObject explosionObject, metalBoxObj, bossPrefab;
    [SerializeField] private Collider2D explosionCol;
    [SerializeField] private Door entryDoor, exitDoor;
    [SerializeField] private BossScript boss;
    [SerializeField] private Transform bossSpawnPoint, explosionForceOrgin;
    [SerializeField] private float explosionTime, timer, explosionDistance, explosionForce;
    public float ExplosionTime {get {return explosionTime;}}
    bool Elapsed {get {return timer <= 0;}}

    private bool isDead = false;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null) {Instance = this;}
        else if(Instance != this) {Destroy(this);}
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead && exitDoor.State == Door.DoorState.CLOSED){
            exitDoor.Open();
        }
    }

    public void Reset(){
        Destroy(boss.gameObject);
        GameObject newBoss = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
        boss = newBoss.GetComponent<BossScript>();
        entryDoor.Open(true);
    } 

    public void DropMetalBox(){
        GameObject newBox = Instantiate(metalBoxObj, transform.position, transform.rotation);
    }

    public void TriggerExplosion(){
        if(explosionObject != null){
            StartCoroutine(ExplosionCoroutine());
        }
    }

    public void OnDeath(){
        exitDoor.Open();
        isDead = true;
    }

    public void ExplosionBoom(){
        
        foreach (GameObject chain in GameObject.FindGameObjectsWithTag("chain")){
            // part.transform.localScale = animatedObjParent.transform.localScale;
            // part.SetActive(true);
            // part.transform.SetParent(null);
            float distance = Vector2.Distance(explosionForceOrgin.position, chain.transform.position);
            if( distance > explosionDistance){continue;}

            Vector2 closestPoint = chain.GetComponent<Collider2D>().ClosestPoint(explosionForceOrgin.position);
            Vector2 forceVector = (closestPoint - (Vector2)explosionForceOrgin.position).normalized * explosionForce * Random.Range(0.1f, 1f) *(explosionDistance - distance) / explosionDistance;
            
            chain.GetComponent<Rigidbody2D>().AddForceAtPosition(forceVector, explosionForceOrgin.position);
        }
    }

    IEnumerator ExplosionCoroutine(){
        
        // explosionObject.SetActive(true);
        ExplosionScript.Instance.StartExplosion();

        yield return new WaitForSeconds(explosionTime);

        // ExplosionScript.Instance.StopExplosion();
        // explosionObject.SetActive(false);

        DropMetalBox();

    }

    void OnTriggerEnter2D(Collider2D col){
        if(col == Player.main.MainCol && !isDead){
            entryDoor.Close();
            boss.ActivateRobot();
        }
    }
}
