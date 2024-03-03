using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_paralax : MonoBehaviour
{
    private float length,
                  startPos,
                  speed = 3,
                  centerPoint = 0;
    public float parallaxEffect;
    float distance = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance += parallaxEffect * -speed * Time.deltaTime;        

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (transform.position.x < centerPoint - length)
        {            
            startPos = 0;
            distance = 0;
        }
        
    }
}
