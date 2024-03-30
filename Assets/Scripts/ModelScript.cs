using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModelScript : MonoBehaviour
{
    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (sprites.Length > 0)
        {
            int random = Random.Range(0, sprites.Length);
            render.sprite = sprites[random];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimation()
    {
        GetComponent<Animator>().Play("PlatformAnim", -1, 0f);
    }
    public void DropAnimation()
    {
        GetComponent<Animator>().Play("Drop", -1, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.SetParent(null);
    }
}
