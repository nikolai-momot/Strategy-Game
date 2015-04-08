using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public int Scrollspeed = 15;
    public int EdgeDist = 15;

    private Camera cam;
    private GameObject UpperLeft,BottomRight;
    private float ULx, ULy, BRx, BRy;
    private float vertView, horizView, widthOverheight;
    private Vector3 OffsetPosition;

	// Use this for initialization
	void Start () {        
        GameObject UpperLeft = GameObject.FindGameObjectWithTag("TopLeft");
        GameObject BottomRight = GameObject.FindGameObjectWithTag("BottomRight");
        cam = GetComponent<Camera>();
        ULx = UpperLeft.transform.position.x;
        ULy = UpperLeft.transform.position.y;
        BRx = BottomRight.transform.position.x;
        BRy = BottomRight.transform.position.y;
        transform.position = new Vector3(0, 0, -50);
    }
	
	// Update is called once per frame
	void Update () {
        vertView = cam.orthographicSize;
        horizView = vertView * Screen.width / Screen.height;

        if (Input.mousePosition.x < EdgeDist && transform.position.x >= (ULx + horizView)) OffsetPosition = new Vector3(transform.position.x - Time.deltaTime * Scrollspeed, transform.position.y, -50);
        if (Input.mousePosition.x > Screen.width - EdgeDist && transform.position.x < (BRx - horizView)) OffsetPosition = new Vector3(transform.position.x + Time.deltaTime * Scrollspeed, transform.position.y, -50);
        if (Input.mousePosition.y < EdgeDist && transform.position.y > (BRy + vertView)) OffsetPosition = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * Scrollspeed, -50);
        if (Input.mousePosition.y > Screen.height - EdgeDist && transform.position.y <= (ULy - vertView)) OffsetPosition = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * Scrollspeed, -50);
        transform.position = OffsetPosition;

        /*if (Input.GetKey(KeyCode.W) && cam.orthographicSize > 2) { //Zoom in and out
            cam.orthographicSize -= Time.deltaTime * Scrollspeed;
        }
        if (Input.GetKey(KeyCode.S)) {
            cam.orthographicSize += Time.deltaTime * Scrollspeed;
        }*/
    }
}
