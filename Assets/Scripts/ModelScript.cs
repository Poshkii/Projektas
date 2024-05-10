using UnityEngine;

public class ModelScript : MonoBehaviour
{
    public Sprite defaultSprite;
    void Start()
    {
        GameManager manager = FindObjectOfType<GameManager>();
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.sprite = defaultSprite;

        if (manager != null)
        {
            render.sprite = manager.SetSprite();
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
