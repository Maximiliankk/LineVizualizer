using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Todo:
/// -add chord progression audio
/// -show/highlight which interval is being played with audio
/// -read a text file with points
/// -write edited points out to a file
/// 
/// </summary>
public class ASM : MonoBehaviour
{
    List<GameObject> PointList = new List<GameObject>();
    public GameObject linePrefab;
    public GameObject pointPrefab;

    Camera mainCam;
    public Vector3 cameraFocus = new Vector3(1.5f, 1.5f, 1.5f);

    const float defaultLineWidth = 0.2f;
    const float defaultCameraAngle = 0.785f; // PI / 4f
    public float cameraXangle = defaultCameraAngle;
    private float cameraYangle;
    private const float defaultCamDistance = 7;
    private float camDistance = defaultCamDistance;

    bool oneSelected = false;
    GameObject selectedPoint;
    private Vector3 mouseOldPos;
    private Vector3 mouseMoveDelta;
    private bool toggleControls = true;
    private bool toggleLabels = true;
    public float cameraRotateSpeed = 0.001f;
    public float gridLinesThickness = 0.05f;
    public float cameraZoomSpeed = 0.5f;

    GUIStyle pointLabelStyle = new GUIStyle();
    GUIStyle menuLabelStyle = new GUIStyle();

    // Use this for initialization
    void Start()
    {
        pointLabelStyle.normal.textColor = Color.red;
        menuLabelStyle.normal.textColor = Color.black;

        mainCam = Camera.main;

        AddLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0), 0.6f, Color.grey);
        AddLine(new Vector3(0, 0, 0), new Vector3(0, 10, 0), 0.6f, Color.grey);
        AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10), 0.6f, Color.grey);

        createPoint(4, 3, 1, false);
        createPoint(4, 2, 2);
        createPoint(4, 1, 3);
        createPoint(4, 1, 4);
        createPoint(3, 1, 4);
        createPoint(2, 2, 4);
        createPoint(1, 3, 4);
        createPoint(2, 3, 4);
        createPoint(3, 2, 4);
        createPoint(4, 2, 4);
        createPoint(4, 2, 3);
        createPoint(3, 2, 3);
        createPoint(2, 3, 3);
        createPoint(3, 3, 3);
        createPoint(3, 3, 4);
        createPoint(4, 3, 4);
        createPoint(4, 3, 3);
        createPoint(4, 3, 2);
        createPoint(3, 3, 2);
        createPoint(2, 4, 2);
        createPoint(2, 4, 3);
        createPoint(1, 4, 3);
        createPoint(1, 4, 4);
        createPoint(2, 4, 4);
        createPoint(3, 4, 4);
        createPoint(3, 4, 3);
        createPoint(4, 4, 3);
        createPoint(4, 4, 2);
        createPoint(3, 4, 2);
        createPoint(3, 4, 1);
        createPoint(4, 4, 1);

        // grid points
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    var go = Instantiate(pointPrefab);
                    go.transform.position = new Vector3(i, j, k);
                    go.transform.localScale *= defaultLineWidth;
                    go.transform.GetComponent<Renderer>().material.color = Color.black;
                }
            }
        }

        // grid lines 
        var P0 = new Vector3(0, 0, 0);
        var P1 = new Vector3(0, 0, 0);

        // draw 16 lines of the grid along x axis
        for (int j = 0; j < 4; j++)
        {
            P0 = new Vector3(0, j, 0);
            P1 = new Vector3(3, j, 0);
            AddLine(P0, P1, gridLinesThickness, Color.red);
            P0.z += 1; P1.z += 1;
            AddLine(P0, P1, gridLinesThickness, Color.red);
            P0.z += 1; P1.z += 1;
            AddLine(P0, P1, gridLinesThickness, Color.red);
            P0.z += 1; P1.z += 1;
            AddLine(P0, P1, gridLinesThickness, Color.red);
        }
        // draw 16 lines of the grid along z axis
        for (int j = 0; j < 4; j++)
        {
            P0 = new Vector3(0, j, 0);
            P1 = new Vector3(0, j, 3);
            AddLine(P0, P1, gridLinesThickness, Color.blue);
            P0.x += 1; P1.x += 1;
            AddLine(P0, P1, gridLinesThickness, Color.blue);
            P0.x += 1; P1.x += 1;
            AddLine(P0, P1, gridLinesThickness, Color.blue);
            P0.x += 1; P1.x += 1;
            AddLine(P0, P1, gridLinesThickness, Color.blue);
        }
        // draw 16 lines of the grid along y axis
        for (int j = 0; j < 4; j++)
        {
            P0 = new Vector3(0, 0, j);
            P1 = new Vector3(0, 3, j);
            AddLine(P0, P1, gridLinesThickness, Color.green);
            P0.x += 1; P1.x += 1;   
            AddLine(P0, P1, gridLinesThickness, Color.green);
            P0.x += 1; P1.x += 1;
            AddLine(P0, P1, gridLinesThickness, Color.green);
            P0.x += 1; P1.x += 1;
            AddLine(P0, P1, gridLinesThickness, Color.green);
        }
        // draw the grid (small) spheres
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    var go = Instantiate(pointPrefab);
                    go.transform.position = new Vector3(i, j, k);
                    go.transform.localScale *= defaultLineWidth;
                    go.transform.GetComponent<Renderer>().material.color = Color.black;
                }
            }
        }
    }
    // Makes a sphere and a line (by default)
    void createPoint(float x, float y, float z, bool makeLine = true)
    {
        GameObject go = Instantiate(pointPrefab);
        go.transform.position = new Vector3(x - 1, y - 1, z - 1);
        go.transform.GetComponent<Renderer>().material.color = Color.black;
        if (makeLine)
        {
            AddLine(go.transform.position, PointList[PointList.Count - 1].transform.position, defaultLineWidth * 2);
        }
        PointList.Add(go);
    }

    // Update is called once per frame
    void Update()
    {
        // update the delta movement of the mouse
        mouseMoveDelta = Input.mousePosition - mouseOldPos;
        // update the old mouse position
        mouseOldPos = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.T)) // if T is pressed
        {
            ToggleControls(); // toggle the UI on/off
        }
        if (Input.GetKeyDown(KeyCode.L)) // if L is pressed
        {
            ToggleLabels(); // toggle the Labels on/off
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) // if R is pressed while LeftShift is held
        {
            ResetCamera(); // reset the camera angle to default
        }

        if (Input.GetMouseButton(1)) // right mouse button held
        {
            cameraXangle += (float)mouseMoveDelta.x * cameraRotateSpeed;
            //cameraYangle += mouseMoveDelta.y;
        }
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            camDistance -= cameraZoomSpeed;
        }
        else if (d < 0f)
        {
            camDistance += cameraZoomSpeed;
        }

        mainCam.transform.position = cameraFocus + (new Vector3(Mathf.Sin(cameraXangle), 0, Mathf.Cos(cameraXangle))) * (camDistance);
        mainCam.transform.LookAt(cameraFocus);

        if (Input.GetMouseButtonDown(0)) // left mouse button pressed
        {
            Ray r = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit rh;

            if (Physics.Raycast(r, out rh))
            {
                if (rh.collider.gameObject.tag == "Point")
                {
                    Debug.Log("Clicked point!");
                    if (oneSelected)
                    {
                        Debug.Log("created line");
                        AddLine(selectedPoint.transform.position, rh.transform.position);
                        selectedPoint.GetComponent<Renderer>().material.color = Color.black;
                        rh.transform.GetComponent<Renderer>().material.color = Color.black;
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
        DebuggingUpdate();
    }

    private void DebuggingUpdate()
    {
        Ray r = mainCam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(r.origin, r.direction * 100); // debug drawing in Unity Scene Tab for camera look direction
    }
    // creates a cylinder in the scene connecting p1 to p2
    // of thickness linewidth
    // and color lineColor
    public void AddLine(Vector3 p1, Vector3 p2, float linewidth = defaultLineWidth, Color lineColor = default(Color))
    {
        var go = Instantiate(linePrefab);
        go.transform.position = p1;
        go.transform.localScale = new Vector3(go.transform.localScale.x * linewidth, go.transform.localScale.y * linewidth, Vector3.Distance(p1, p2));
        go.transform.GetComponentInChildren<Renderer>().material.color = lineColor;
        go.transform.LookAt(p2);
    }
    // toggles the UI control buttons on and off
    private void ToggleControls()
    {
        toggleControls = !toggleControls;
    }
    // toggles the labels on points on and off
    private void ToggleLabels()
    {
        toggleLabels = !toggleLabels;
    }
    // handles all immediate mode GUI (IMGUI) screenspace UI
    private void OnGUI()
    {
        var LeftUIypos = 10;
        if (GUI.Button(new Rect(10, LeftUIypos, 130, 20), "Toggle Controls (T)"))
        {
            ToggleControls();
        }
        if (toggleControls)
        {
            LeftUIypos += 40;
            if (GUI.Button(new Rect(10, LeftUIypos, 130, 20), "Toggle Labels (L)"))
            {
                ToggleLabels();
            }
            LeftUIypos += 20;
            GUI.Label(new Rect(10, LeftUIypos, 230, 20), "Right mouse drag.....rotate view", menuLabelStyle);
            LeftUIypos += 20;
            GUI.Label(new Rect(10, LeftUIypos, 230, 20), "Left mouse click.....connect points", menuLabelStyle);
            LeftUIypos += 20;
            GUI.Label(new Rect(10, LeftUIypos, 230, 20), "Scrollwheel.....zoom in/out", menuLabelStyle);
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, 230, 20), "Reset (LeftShift + R)"))
            {
                ResetCamera();
            }
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, 230, 20), "Animate"))
            {
                StopAllCoroutines();
                StartCoroutine(AnimateLine());
            }
        }
        if (toggleLabels)
        {
            for (int i = 0; i < PointList.Count; i++) // for each point in PointList
            {
                var pointCoords = PointList[i].transform.position;
                var screenPoint = mainCam.WorldToScreenPoint(pointCoords);
                var width = 50f;
                var height = 20f;
                // show the label
                GUI.Label(
                    new Rect(screenPoint.x - width / 2,
                    Screen.height - screenPoint.y, width, height),
                    "[" + (pointCoords.x + 1) + ", " + (pointCoords.y + 1) + ", " + (pointCoords.z + 1) + "]",
                    pointLabelStyle);
            }
        }
    }

    private void ResetCamera()
    {
        cameraXangle = defaultCameraAngle;
        camDistance = defaultCamDistance;
        for (int i = 0; i < PointList.Count; i++)
        {
            PointList[i].GetComponent<Renderer>().material.color = Color.black;
        }
    }

    IEnumerator AnimateLine()
    {
        for (int i = 0; i < PointList.Count; i++)
        {
            PointList[i].GetComponent<Renderer>().material.color = Color.black;
        }
        for (int i = 0; i < PointList.Count; i++)
        {
            PointList[i].GetComponent<Renderer>().material.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
