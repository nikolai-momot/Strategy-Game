using UnityEngine;
using System.Collections;

public class TargetIndAnimate : MonoBehaviour {
    private float timer = 0.0f;
    private float scale = 1.0f;
    private float size;
	
	// Update is called once per frame
	void Update () {
        //if (timer >= 1.0f) Destroy(gameObject);
        //transform.Rotate(Vector3.forward, Time.deltaTime * 100);
        size = scale + (0.25f) * Mathf.Sin(timer);
        transform.localScale = new Vector3(size, size, 1);
        timer += Time.deltaTime;
	}
}
