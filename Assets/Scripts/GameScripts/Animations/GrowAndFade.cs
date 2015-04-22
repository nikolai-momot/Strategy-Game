using UnityEngine;
using System.Collections;

public class GrowAndFade : MonoBehaviour {

    private float timer = 0.0f;
    private float scale = 1.0f;
    private float size;
    private SpriteRenderer sprite;
    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        transform.position += Vector3.up;
    }

    // Update is called once per frame
    void Update() {
        if (timer >= 1.0f) Destroy(gameObject);
        size = scale + 3*(timer);
        transform.localScale = new Vector3(size, size, 1);
        sprite.color = new Color(1f,1f,1f,1- timer);
        timer += Time.deltaTime;
    }
}
