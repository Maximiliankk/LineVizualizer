using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASM : MonoBehaviour {

    List<GameObject> VertList;
    public GameObject linePrefab;
    public GameObject pointPrefab;

    Camera mainCam;
    public Vector3 cameraFocus = new Vector3(1.5f,1.5f,1.5f);
    private float cameraXangle;
    private float cameraYangle;

    bool oneSelected = false;
    GameObject selectedPoint;
    private Vector3 mouseOldPos;
    private Vector3 mouseMoveDelta;
    private float camDistance = 7;

    // Use this for initialization
    void Start () {
        mainCam = Camera.main;

        createPoint(4,3,1);
        createPoint(4,2,2);
        createPoint(4,1,3);
        createPoint(4,1,4);
        createPoint(3,1,4);
        createPoint(2,2,4);
        createPoint(1,3,4);
        createPoint(2,3,4);
        createPoint(3,2,4);
        createPoint(4,2,4);
        createPoint(4,2,3);
        createPoint(3,2,3);
        createPoint(2,3,3);
        createPoint(3,3,3);
        createPoint(3,3,4);
        createPoint(4,3,4);
        createPoint(4,3,3);
        createPoint(4,3,2);
        createPoint(3,3,2);
        createPoint(2,4,2);
        createPoint(2,4,3);
        createPoint(1,4,3);
        createPoint(1,4,4);
        createPoint(2,4,4);
        createPoint(3,4,4);
        createPoint(3,4,3);
        createPoint(4,4,3);
        createPoint(4,4,2);
        createPoint(3,4,2);
        createPoint(3,4,1);
        createPoint(4,1,1);
        

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    var go = Instantiate(pointPrefab);
                    go.transform.position = new Vector3(i,j,k);
		    go.transform.localScale *= 0.2f;
                    go.transform.GetComponent<Renderer>().material.color = Color.black;
                }
            }
        }
	}
	
        void createPoint(float x, float y, float z) {

           var go = Instantiate(pointPrefab);
           go.transform.position = new Vector3(x-1,y-1,z-1);
           go.transform.GetComponent<Renderer>().material.color = Color.black;

	}

	// Update is called once per frame
	void Update () {

        mouseMoveDelta = Input.mousePosition - mouseOldPos;
        mouseOldPos = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            cameraXangle += (float)mouseMoveDelta.x * 0.001f;
            //cameraYangle += mouseMoveDelta.y;
        }
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            camDistance -= 0.5f;
        }
        else if (d < 0f)
        {
            camDistance += 0.5f;
        }


        mainCam.transform.position = cameraFocus + new Vector3(Mathf.Sin(cameraXangle), 0, Mathf.Cos(cameraXangle)) * (camDistance);
        mainCam.transform.LookAt(cameraFocus);

        if (Input.GetMouseButtonDown(0))
        {
            Ray r = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit rh;

            if(Physics.Raycast(r, out rh))
            {
                if(rh.collider.gameObject.tag == "Point")
                {
                    Debug.Log("Clicked point!");
                    if (oneSelected)
                    {
                        Debug.Log("created line");
                        AddLine(selectedPoint.transform.position, rh.transform.position, 0.2f);
                        selectedPoint.GetComponent<Renderer>().material.color = Color.white;
                        rh.transform.GetComponent<Renderer>().material.color = Color.white;
                        oneSelected = false;
                    }
                    else
                    {
                        Debug.Log("Selected");
                        rh.transform.GetComponent<Renderer>().material.color = Color.red;
                        oneSelected = true;
                        selectedPoint = rh.transform.gameObject;
                    }
                }
            }
        }
        MouseRayCast();
	}

    private void MouseRayCast()
    {
        Ray r = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rh;

        Debug.DrawRay(r.origin, r.direction * 100);

        if (Physics.Raycast(r, out rh))
        {
            if (rh.collider.gameObject.tag == "Point")
            {
                //rh.transform.GetComponent<Renderer>().material.color = Color.green;
            }
        }
        else
        {
            //Debug.Log("nothing hit");
        }
    }

    public void AddLine(Vector3 p1, Vector3 p2, float lw)
    {
        var go = Instantiate(linePrefab);
        go.transform.position = p1;
        go.transform.localScale = new Vector3(go.transform.localScale.x * lw, go.transform.localScale.y * lw, Vector3.Distance(p1,p2));
        go.transform.LookAt(p2);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Save"))
        {

        }
    }
}
