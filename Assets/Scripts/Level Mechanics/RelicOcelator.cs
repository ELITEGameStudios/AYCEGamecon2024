using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicOcelator : MonoBehaviour
{
    [SerializeField] private float amplitude = 1.0f;
    [SerializeField] private float speed = 2f;

    private Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector2(startPos.x, startPos.y + offset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
