using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterPlatform : MonoBehaviour
{
    public float duration = 2f; // Duration of fadeout
    private Material material;
    internal Vector3 spawnPos = Vector3.zero;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Vector3 colliderCenter = GetComponent<BoxCollider2D>().bounds.center;
        Vector3 colliderExtents = GetComponent<BoxCollider2D>().bounds.extents;
        Vector3 playerExtents = player.GetComponent<BoxCollider2D>().bounds.extents;
        Vector3 spawnPos = new Vector3(colliderCenter.x, colliderCenter.y + colliderExtents.y + playerExtents.y, 0);
        //Debug.Log(colliderCenter.x); //-8.144544
        //Debug.Log(colliderCenter.y + colliderExtents.y + playerExtents.y); //1.308019
        player.SetSpawnPos(spawnPos);

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            material = renderer.material;

        StartCoroutine(FadeOut(5f));
    }

    void Update()
    {
        if (player.raycastCheck)
        {
            StartCoroutine(FadeOut(0f));
        }
    }

    private IEnumerator FadeOut(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (material != null)
        {
            float time = 0f;
            Color color = material.color;
            while (time < duration)
            {
                float t = time / duration;
                material.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1, 0, t));
                time += Time.deltaTime;
                yield return null;
            }
            material.color = new Color(color.r, color.g, color.b, 0);
        }
        Destroy(gameObject);
    }
}
