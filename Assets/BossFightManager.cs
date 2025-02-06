using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    // This manager object will be positioned where metal boxes should spawn
    // Collider will be the zone which the bossfight commences
    public static BossFightManager Instance { get; private set; }
    [SerializeField] private GameObject explosionObject, metalBoxObj;
    [SerializeField] private Collider2D explosionCol;
    [SerializeField] private Door entryDoor, exitDoor;
    [SerializeField] private float explosionTime, timer;
    public float ExplosionTime {get {return explosionTime;}}
    bool Elapsed {get {return timer <= 0;}}

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null) {Instance = this;}
        else if(Instance != this) {Destroy(this);}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){

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
    }

    IEnumerator ExplosionCoroutine(){
        
        explosionObject.SetActive(true);

        yield return new WaitForSeconds(explosionTime);

        explosionObject.SetActive(false);

        DropMetalBox();

    }

    void OnTriggerEnter2D(Collider2D col){
        if(col == Player.main.MainCol){
            entryDoor.Close();
        }
    }
}
