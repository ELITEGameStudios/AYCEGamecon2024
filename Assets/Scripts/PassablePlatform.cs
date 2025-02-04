using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassablePlatform : MonoBehaviour
{
    Collider2D collider;
    [SerializeField] float error;

    void Awake(){
        error = transform.localScale.y/2;
        collider = gameObject.GetComponent<Collider2D>();
    }
 
    // Update is called once per frame
    void Update()
    {
        collider.isTrigger = ( Player.main.transform.position - transform.position).y - Player.main.transform.localScale.y/2 < error;
    }
}
