using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindScript : MonoBehaviour
{
    public float strength = 1f;
    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3 (transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.velocity;
            velocity.x -= strength * Time.deltaTime;
            rb.velocity = velocity;
            //rb.AddForce(new Vector2(-strength * Time.deltaTime, 0));
        }
    }
}
