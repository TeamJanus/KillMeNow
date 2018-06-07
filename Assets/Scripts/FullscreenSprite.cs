using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script taken from Kyle W Banks, at https://kylewbanks.com/blog/create-fullscreen-background-image-in-unity2d-with-spriterenderer
public class FullscreenSprite : MonoBehaviour {

    private void Awake() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector2 scale = transform.localScale;
        if (cameraSize.x >= cameraSize.y) {  //landscape
            scale *= cameraSize.x / spriteSize.x;
        } else { //portrait
            scale *= cameraSize.y / spriteSize.y;
        }

        transform.position = Vector2.zero;
        transform.localScale = scale;

    }
}
