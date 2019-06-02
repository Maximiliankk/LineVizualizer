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
    List<GameObject> FirstPoints = new List<GameObject>();
    List<GameObject> FirstLines = new List<GameObject>();

    List<GameObject> HamiltonPoints = new List<GameObject>();
    List<GameObject> HamiltonLines = new List<GameObject>();

    List<GameObject> TriadPoints = new List<GameObject>();
    List<GameObject> TriadLines = new List<GameObject>();

    List<GameObject> GridPoints = new List<GameObject>();
    List<GameObject> GridLines = new List<GameObject>();

    public GameObject linePrefab;
    public GameObject pointPrefab;

    Camera mainCam;
    public Vector3 cameraFocus = new Vector3(1.5f, 1.5f, 1.5f);

    const float defaultLineWidth = 0.2f;
    public float defaultCameraAngleX = 45;
    public float defaultCameraAngleY = 45;
    public float defaultCamDistance = 10;
    private float cameraXangle;
    private float cameraYangle;
    private float camDistance;

    bool oneSelected = false;
    GameObject selectedPoint;
    private Vector3 mouseOldPos;
    private Vector3 mouseMoveDelta;
    private bool toggleControls = true;
    private bool toggleLabels = true;
    public float cameraRotateSpeed = 0.001f;
    public float gridLinesThickness = 0.05f;
    public float cameraZoomSpeed = 0.5f;
    Color currentColor = Color.black;

    GUIStyle pointLabelStyle = new GUIStyle();
    GUIStyle menuLabelStyle = new GUIStyle();

    // Use this for initialization
    void Start()
    {
        ResetCamera();
        pointLabelStyle.normal.textColor = Color.red;
        menuLabelStyle.normal.textColor = Color.black;

        mainCam = Camera.main;

//        DrawFirstPath();
//        DrawTriadSet();

        // grid points


    }

    void drawGridLines(int dim, float scale, Color c)
    {
        // grid lines 
        var P0 = new Vector3(0, 0, 0);
        var P1 = new Vector3(0, 0, 0);

        // draw 16 lines of the grid along x axis
        for (int j = 0; j < dim; j++)
        {
            P0 = new Vector3(0, j, 0);
            P1 = new Vector3(dim - 1, j, 0);

            for (int k = 0; k < dim; k++)
            {
                AddLine(P0, P1, gridLinesThickness, c, GridLines);
                P0.z += 1; P1.z += 1;
            }
        }
        for (int j = 0; j < dim; j++)
        {
            P0 = new Vector3(0, j, 0);
            P1 = new Vector3(0, j, dim - 1);

            for (int k = 0; k < dim; k++)
            {
                AddLine(P0, P1, gridLinesThickness, c, GridLines);
                P0.x += 1; P1.x += 1;
            }
        }
        for (int j = 0; j < dim; j++)
        {
            P0 = new Vector3(0, 0, j);
            P1 = new Vector3(0, dim - 1, j);

            for (int k = 0; k < dim; k++)
            {
                AddLine(P0, P1, gridLinesThickness, c, GridLines);
                P0.x += 1; P1.x += 1;
            }
        }
        // points
        drawGridPoints(dim, scale, Color.black);
    }

    // scale is multiplier
    void drawGridPoints(int dim, float scale, Color c)
    {
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                for (int k = 0; k < dim; k++)
                {
                    var go = Instantiate(pointPrefab);
                    go.transform.position = new Vector3(i, j, k);
                    go.transform.localScale *= defaultLineWidth * scale;
                    go.transform.GetComponent<Renderer>().material.color = c;
                    GridPoints.Add(go);
                }
            }
        }
    }

    // Makes a sphere and a line (by default)
    void createPoint(float x, float y, float z, bool makeLine, List<GameObject> pointbuffer, List<GameObject> linebuffer)
    {
        GameObject go = Instantiate(pointPrefab);
        go.transform.position = new Vector3(x - 1, y - 1, z - 1);
        go.transform.GetComponent<Renderer>().material.color = Color.black;
        go.GetComponent<Renderer>().material.color = currentColor;

        pointbuffer.Add(go);
        if (makeLine)
        { 
            AddLine(go.transform.position, pointbuffer[pointbuffer.Count - 2].transform.position, defaultLineWidth * 2, currentColor, linebuffer);
        }
    }

    void SetColor(Color c)
    {
        currentColor = c;
    }

    //void DrawFirstPath()
    //{
    //    SetColor(Color.black); // color of this path
    //    createPoint(4, 3, 1, false);
    //    createPoint(4, 2, 2, true, TriadLines, TriadPoints);
    //    createPoint(4, 1, 3);
    //    createPoint(4, 1, 4);
    //    createPoint(3, 1, 4);
    //    createPoint(2, 2, 4);
    //    createPoint(1, 3, 4);
    //    createPoint(2, 3, 4);
    //    createPoint(3, 2, 4);
    //    createPoint(4, 2, 4);
    //    createPoint(4, 2, 3);
    //    createPoint(3, 2, 3);
    //    createPoint(2, 3, 3);
    //    createPoint(3, 3, 3);
    //    createPoint(3, 3, 4);
    //    createPoint(4, 3, 4);
    //    createPoint(4, 3, 3);
    //    createPoint(4, 3, 2);
    //    createPoint(3, 3, 2);
    //    createPoint(2, 4, 2);
    //    createPoint(2, 4, 3);
    //    createPoint(1, 4, 3);
    //    createPoint(1, 4, 4);
    //    createPoint(2, 4, 4);
    //    createPoint(3, 4, 4);
    //    createPoint(3, 4, 3);
    //    createPoint(4, 4, 3);
    //    createPoint(4, 4, 2);
    //    createPoint(3, 4, 2);
    //    createPoint(3, 4, 1);
    //    createPoint(4, 4, 1);
    //}

    void DrawHamiltonCycle()
    {
        SetColor(Color.black); // color of this path
        createPoint(4, 4, 3, false, HamiltonPoints, HamiltonLines);
        createPoint(3, 4, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 4, 2, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 4, 2, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 4, 1, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 4, 1, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 3, 1, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 2, 2, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 1, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 1, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 1, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(2, 2, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(1, 3, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(2, 3, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(2, 3, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 3, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 3, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 3, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 3, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 2, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 2, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 2, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 2, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 3, 2, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 3, 2, true, HamiltonPoints, HamiltonLines);
        createPoint(2, 4, 2, true, HamiltonPoints, HamiltonLines);
        createPoint(2, 4, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(1, 4, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(1, 4, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(2, 4, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 4, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 4, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 5, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 6, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 5, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 4, 5, true, HamiltonPoints, HamiltonLines);
        createPoint(3, 3, 6, true, HamiltonPoints, HamiltonLines);
        createPoint(4, 3, 5, true, HamiltonPoints, HamiltonLines);
        createPoint(5, 3, 4, true, HamiltonPoints, HamiltonLines);
        createPoint(6, 3, 3, true, HamiltonPoints, HamiltonLines);
        createPoint(5, 4, 3, true, HamiltonPoints, HamiltonLines);
    }

    void DrawFirstPath()
    {
        SetColor(Color.black); // color of this path
        createPoint(4, 3, 1, false, FirstPoints, FirstLines);
        createPoint(4, 2, 2, true, FirstPoints, FirstLines);
        createPoint(4, 1, 3, true, FirstPoints, FirstLines);
        createPoint(4, 1, 4, true, FirstPoints, FirstLines);
        createPoint(3, 1, 4, true, FirstPoints, FirstLines);
        createPoint(2, 2, 4, true, FirstPoints, FirstLines);
        createPoint(1, 3, 4, true, FirstPoints, FirstLines);
        createPoint(2, 3, 4, true, FirstPoints, FirstLines);
        createPoint(3, 2, 4, true, FirstPoints, FirstLines);
        createPoint(4, 2, 4, true, FirstPoints, FirstLines);
        createPoint(4, 2, 3, true, FirstPoints, FirstLines);
        createPoint(3, 2, 3, true, FirstPoints, FirstLines);
        createPoint(2, 3, 3, true, FirstPoints, FirstLines);
        createPoint(3, 3, 3, true, FirstPoints, FirstLines);
        createPoint(3, 3, 4, true, FirstPoints, FirstLines);
        createPoint(4, 3, 4, true, FirstPoints, FirstLines);
        createPoint(4, 3, 3, true, FirstPoints, FirstLines);
        createPoint(4, 3, 2, true, FirstPoints, FirstLines);
        createPoint(3, 3, 2, true, FirstPoints, FirstLines);
        createPoint(2, 4, 2, true, FirstPoints, FirstLines);
        createPoint(2, 4, 3, true, FirstPoints, FirstLines);
        createPoint(1, 4, 3, true, FirstPoints, FirstLines);
        createPoint(1, 4, 4, true, FirstPoints, FirstLines);
        createPoint(2, 4, 4, true, FirstPoints, FirstLines);
        createPoint(3, 4, 4, true, FirstPoints, FirstLines);
        createPoint(3, 4, 3, true, FirstPoints, FirstLines);
        createPoint(4, 4, 3, true, FirstPoints, FirstLines);
        createPoint(4, 4, 2, true, FirstPoints, FirstLines);
        createPoint(3, 4, 2, true, FirstPoints, FirstLines);
        createPoint(3, 4, 1, true, FirstPoints, FirstLines);
        createPoint(4, 4, 1, true, FirstPoints, FirstLines);
    }

    void DrawTriadSet()
    {
        SetColor(Color.green); // color of this path
        createPoint(6, 3, 3, false, TriadPoints, TriadLines);
        createPoint(3, 6, 3, true, TriadPoints, TriadLines);
        createPoint(3, 3, 6, true, TriadPoints, TriadLines);
        createPoint(4, 4, 4, true, TriadPoints, TriadLines);
        createPoint(5, 3, 4, true, TriadPoints, TriadLines);
        createPoint(5, 4, 3, true, TriadPoints, TriadLines);
        createPoint(4, 5, 3, true, TriadPoints, TriadLines);
        createPoint(3, 5, 4, true, TriadPoints, TriadLines);
        createPoint(3, 4, 5, true, TriadPoints, TriadLines);
        createPoint(4, 3, 5, true, TriadPoints, TriadLines);
    }

    void ClearPointsAndLines()
    {
        //ClearBuffer(PointList);
        //ClearBuffer(LineList);
        //PointList.Clear();
        //LineList.Clear();
    }

    void ClearBuffer(List<GameObject> buffer)
    {
        foreach(var go in buffer)
        {
            Destroy(go);
        }
        buffer.Clear();
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

        if (Input.GetMouseButton(0)) // left mouse button
        {
            cameraXangle -= mouseMoveDelta.x * cameraRotateSpeed;
            cameraYangle -= mouseMoveDelta.y * cameraRotateSpeed;
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

        // camera rotation
        mainCam.transform.parent.rotation = Quaternion.identity;
        mainCam.transform.position = new Vector3(camDistance, 0, 0);
        mainCam.transform.parent.Rotate(0, -cameraXangle, cameraYangle);

        if (Input.GetMouseButtonDown(1)) // right mouse button
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
    }

    // creates a cylinder in the scene connecting p1 to p2
    // of thickness linewidth
    // and color lineColor
    public void AddLine(Vector3 p1, Vector3 p2, float linewidth = defaultLineWidth, Color lineColor = default(Color), List<GameObject> buffer = null)
    {
        var go = Instantiate(linePrefab);
        go.transform.position = p1;
        go.transform.localScale = new Vector3(go.transform.localScale.x * linewidth, go.transform.localScale.y * linewidth, Vector3.Distance(p1, p2));
        go.transform.GetComponentInChildren<Renderer>().material.color = lineColor;
        go.transform.LookAt(p2);
        if(buffer != null)
        {
            buffer.Add(go);
        }
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
            int buttonWidths = 150;
            LeftUIypos += 40;
            if (GUI.Button(new Rect(10, LeftUIypos, 130, 20), "Toggle Labels (L)"))
            {
                ToggleLabels();
            }
            LeftUIypos += 20;
            GUI.Label(new Rect(10, LeftUIypos, buttonWidths, 20), "Left mouse drag.....rotate view", menuLabelStyle);
            LeftUIypos += 20;
            GUI.Label(new Rect(10, LeftUIypos, buttonWidths, 20), "Right mouse click.....connect points", menuLabelStyle);
            LeftUIypos += 20;
            GUI.Label(new Rect(10, LeftUIypos, buttonWidths, 20), "Scrollwheel.....zoom in/out", menuLabelStyle);
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, buttonWidths, 20), "Reset (LeftShift + R)"))
            {
                ResetCamera();
            }
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, buttonWidths, 20), "Animate"))
            {
                StopAllCoroutines();
                StartCoroutine(AnimateLine());
            }
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, buttonWidths, 20), "Clear Points"))
            {
                StopAllCoroutines();
                ClearPointsAndLines();
            }
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, buttonWidths, 20), "Draw First Path"))
            {
                StopAllCoroutines();
                DrawFirstPath();
            }
            if (GUI.Button(new Rect(10 + buttonWidths + 10, LeftUIypos, buttonWidths, 20), "Delete First Path"))
            {
                ClearBuffer(FirstLines);
                ClearBuffer(FirstPoints);
            }
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, buttonWidths, 20), "Draw Triad Set"))
            {
                StopAllCoroutines();
                DrawTriadSet();
            }
            if (GUI.Button(new Rect(10 + buttonWidths + 10, LeftUIypos, buttonWidths, 20), "Delete Triad Set"))
            {
                ClearBuffer(TriadLines);
                ClearBuffer(TriadPoints);
            }
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, buttonWidths, 20), "Draw Hamilton Set"))
            {
                StopAllCoroutines();
                DrawHamiltonCycle();
            }
            if (GUI.Button(new Rect(10 + buttonWidths + 10, LeftUIypos, buttonWidths, 20), "Delete Hamilton Set"))
            {
                ClearBuffer(HamiltonLines);
                ClearBuffer(HamiltonPoints);
            }
            LeftUIypos += 20;
            if (GUI.Button(new Rect(10, LeftUIypos, buttonWidths, 20), "Draw 6x6 cube"))
            {
                drawGridLines(6, 0.5f, new Color(1, 1, 1) * 0.7f);
            }
            if (GUI.Button(new Rect(10 + buttonWidths + 10, LeftUIypos, buttonWidths, 20), "Delete 6x6 cube"))
            {
                ClearBuffer(GridLines);
                ClearBuffer(GridPoints);
            }
        }
        if (toggleLabels)
        {
            List<GameObject>[] pointlists = { HamiltonPoints, FirstPoints, TriadPoints };

            for (int j = 0; j < pointlists.Length; j++)
            {
                for (int i = 0; i < pointlists[j].Count; i++) // for each point in PointList
                {
                    var pointCoords = pointlists[j][i].transform.position;
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
    }

    private void ResetCamera()
    {
        cameraXangle = defaultCameraAngleX;
        cameraYangle = defaultCameraAngleY;
        camDistance = defaultCamDistance;
    }

    IEnumerator AnimateLine()
    {
        List<GameObject>[] linelists = { HamiltonLines, FirstLines, TriadLines };
        List<GameObject>[] pointlists = { HamiltonPoints, FirstPoints, TriadPoints };
        
        //Debug.Log("hlines" + HamiltonLines.Count);
        //Debug.Log("hpoints" + HamiltonPoints.Count);

        //Debug.Log("first lines" + FirstLines.Count);
        //Debug.Log("first points" + FirstPoints.Count);

        //Debug.Log("triad lines" + TriadLines.Count);
        //Debug.Log("triad points" + TriadPoints.Count);

        for (int i = 0; i < linelists.Length; i++)
        {
            for (int j = 0; j < linelists[i].Count; j++)
            {
                if(j <= linelists[i].Count)
                {
                    linelists[i][j].GetComponentInChildren<Renderer>().material.color = Color.yellow;
                }
                if (j <= pointlists[i].Count)
                {
                    pointlists[i][j].GetComponentInChildren<Renderer>().material.color = Color.yellow;
                }
                yield return new WaitForSeconds(0.1f);
            }
            if(pointlists[i].Count > 1)
            {
                pointlists[i][pointlists[i].Count - 1].GetComponentInChildren<Renderer>().material.color = Color.yellow;
            }
        }
    }
}
