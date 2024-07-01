using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class RedSpark : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        var forceVector = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(0.4f, 0.7f));
        rb.AddForce(forceVector);

        StartCoroutine(SparkAnim());
    }

    private IEnumerator SparkAnim()
    {
        var currentScale = 0f;
        var newScale = UnityEngine.Random.Range(1.5f, 2f);
        
        while (currentScale < newScale)
        {
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            currentScale += Time.deltaTime;
            yield return null;
        }

        while (currentScale > 1f)
        {
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            currentScale -= Time.deltaTime;
            yield return null;
        }

        var currentAlph = 1f;
        var startColor = sr.color;
        while (currentAlph > 0f)
        {
            sr.color = new Color(startColor.r, startColor.g, startColor.b, currentAlph);
            transform.localScale = new Vector3(currentAlph, currentAlph, currentAlph);
            currentAlph -= Time.deltaTime / 1.5f;
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
