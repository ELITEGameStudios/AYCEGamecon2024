using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAllChildren : MonoBehaviour
{

    [SerializeField] private bool destroySelf;
    [SerializeField] private float timeOffset, interval;

    void Start(){
        StartCoroutine(DestroyChildrenCoroutine());
    }

    IEnumerator DestroyChildrenCoroutine()
    {
        yield return new WaitForSeconds(timeOffset);
         
        for (int i = transform.childCount-1; i < transform.childCount; i--){  
            Destroy(transform.GetChild(i).gameObject);
            yield return new WaitForSeconds(interval); 
        }
        
        if(destroySelf){Destroy(gameObject);}
    }
}
