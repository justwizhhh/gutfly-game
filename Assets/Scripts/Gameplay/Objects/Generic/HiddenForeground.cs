using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenForeground : MonoBehaviour
{
    // ----------------------
    //
    // This is a chunk of level foreground that disappears when the player collides with it, acting as a hidden wall/path
    //
    // WIP - will need to change this to use a shader later ( :( )
    //
    // ----------------------

    public float AppearMinAlpha;
    public float AppearSpeed;

    private bool isAppearing;
    private float currentAlpha;

    // Component references
    private Material material;

    private void Awake()
    {
        material = GetComponent<TilemapRenderer>().material;
    }

    private void Update()
    {
        // TO-DO - replace this with a shader (i hate tilemaprenderer)
        currentAlpha = Mathf.Clamp(currentAlpha + (Time.deltaTime * AppearSpeed * (!isAppearing ? 1 : -1)), AppearMinAlpha, 1);
        material.SetFloat(Shader.PropertyToID("_FadeTransition"), currentAlpha);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAppearing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAppearing = false;
        }
    }
}
