using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] private CutsceneHandler handler;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (handler == null)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            handler.StartCutscene(gameObject.name);
        }
    }
}
