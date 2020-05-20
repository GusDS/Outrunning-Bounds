using System.Collections.Generic;
using UnityEngine;

public class TrackMgr : MonoBehaviour
{
    public Vector3 startingPoint;
    public GameObject trackPrefab;
    public GameObject grassPrefab;
    public GameObject[] terrainPrefabs;
    public int rowSegments = 20;
    public int rowsMax = 10;
    public float quadSizeX = 10;
    public float quadSizeZ = 10;
    private readonly int[] tris = new int[6] { 0, 2, 1, 2, 3, 1 };
    private readonly Vector2[] uv = new Vector2[4] { Vector2.zero, Vector2.right, Vector2.up, Vector2.one };

    private Control control;
    private List<Vector3> rowPoints = new List<Vector3>();
    private List<Vector3> rowPointsNew = new List<Vector3>();
    private int trackSegment;
    private Mesh mesh;
    private GameObject tempQuad;
    private GameObject tempObj;
    // private List<GameObject> rowQuads = new List<GameObject>();
    // public List<TrackSegment> segmentsData = new List<TrackSegment>();
    // public List<GameObject> segmentsObj = new List<GameObject>();
    private enum CurveTypes
    {
        straightAhead,
        smallRight,
        smallLeft,
        sharpRight,
        sharpLeft
    }
    private enum SlopeTypes
    {
        straightAhead,
        smallDown,
        smallUp,
        sharpDown,
        sharpUp
    }
    private int curveDuration = 5;
    private int slopeDuration = 5;
    private int curveRandom = 0;
    private int slopeRandom = 0;
    private float curveAmount = 0;
    private float slopeAmount = 0;
    private Vector3 curveOffset = Vector3.zero;
    private Vector3 slopeOffset = Vector3.zero;

    private void Start()
    {
        control = GameObject.Find("Control").GetComponent<Control>();

        // initialize points
        for (int i = 0; i < rowSegments + 1; i++) rowPoints.Add(Vector3.right * i * quadSizeX);
        for (int i = 0; i < rowSegments + 1; i++) rowPointsNew.Add(Vector3.zero);
        trackSegment = rowSegments / 2;
        startingPoint = rowPoints[trackSegment] + (Vector3.right * quadSizeX / 2) + (Vector3.forward * quadSizeZ / 2);

        for (int i = 0; i < rowsMax; i++) NewSegmentsRow(); // rowsMax
        // NewSegmentsRow();
        // Util.DebugMe("trackSegment", trackSegment.ToString());
        control.player.transform.position = startingPoint;
    }

    public void NewSegmentsRow()
    {
        // var offsetTrack = Vector3.right * Random.Range(-1f, 1f);
        // for (int i = 0; i < rowSegments + 1; i++) rowPointsNew[i] = rowPoints[i] + (Vector3.forward * quadSizeZ) + offsetTrack; // + (Vector3.up * Random.Range(-1f, 1f));

        if (curveDuration == 0) {
            curveDuration = Random.Range(3, 8); //(5, 11);
            curveRandom = Random.Range(0, 5);
            switch (curveRandom)
            {
                case (int)CurveTypes.straightAhead:
                    curveAmount = 0;
                    break;
                case (int)CurveTypes.smallRight:
                    curveAmount = quadSizeZ * 1.5f / curveDuration; // small = 1 width across N duration
                    break;
                case (int)CurveTypes.smallLeft:
                    curveAmount = quadSizeZ * -1.5f / curveDuration;
                    break;
                case (int)CurveTypes.sharpRight:
                    curveAmount = quadSizeZ * 3 / curveDuration; // sharp = 2 widht across N duration
                    break;
                case (int)CurveTypes.sharpLeft:
                    curveAmount = quadSizeZ * -3 / curveDuration;
                    break;
                default:
                    break;
            }
            curveOffset = Vector3.right * curveAmount;
        }
        else curveDuration--;

        if (slopeDuration == 0)
        {
            slopeDuration = Random.Range(5, 11);
            slopeRandom = Random.Range(0, 5);
            switch (slopeRandom)
            {
                case (int)SlopeTypes.straightAhead:
                    slopeAmount = 0;
                    break;
                case (int)SlopeTypes.smallDown:
                    slopeAmount = quadSizeZ * -1 / slopeDuration; // small = 1 width across N duration
                    break;
                case (int)SlopeTypes.smallUp:
                    slopeAmount = quadSizeZ * 1 / slopeDuration;
                    break;
                case (int)SlopeTypes.sharpDown:
                    slopeAmount = quadSizeZ * -2 / slopeDuration; // sharp = 2 widht across N duration
                    break;
                case (int)SlopeTypes.sharpUp:
                    slopeAmount = quadSizeZ * 2 / slopeDuration;
                    break;
                default:
                    break;
            }
            slopeOffset = Vector3.up * slopeAmount;
        }
        else slopeDuration--;

        for (int i = 0; i < rowSegments + 1; i++) rowPointsNew[i] = rowPoints[i] + (Vector3.forward * quadSizeZ) + curveOffset + slopeOffset;

        // crear quad track
        CreateQuadMesh(trackPrefab, rowPoints[trackSegment], rowPoints[trackSegment + 1], rowPointsNew[trackSegment], rowPointsNew[trackSegment + 1]);

        // crear quads right
        for (int i = trackSegment + 1; i < rowSegments; i++)
        {
            CreateQuadMesh(grassPrefab, rowPoints[i], rowPoints[i + 1], rowPointsNew[i], rowPointsNew[i + 1]);
            //CreateTerrainObject(/*tempQuad,*/ i - trackSegment);
        }

        // crear quads left
        for (int i = trackSegment - 1; i >= 0; i--)
        {
            CreateQuadMesh(grassPrefab, rowPoints[i], rowPoints[i + 1], rowPointsNew[i], rowPointsNew[i + 1]);
            //CreateTerrainObject(/*tempQuad,*/ trackSegment - i);
        }

        //    // rowQuads.Add(tempQuad);
        for (int i = 0; i < rowSegments + 1; i++) rowPoints[i] = rowPointsNew[i];
    }
    public void CreateQuadMesh(GameObject segmentPrefab, Vector3 pointX0Z0, Vector3 pointX1Z0, Vector3 pointX0Z1, Vector3 pointX1Z1)
    {
        // CreateQuadMesh(trackPrefab, Vector3.zero, new Vector3(quadSizeX, 0, 0), new Vector3(0, 0, quadSizeZ), new Vector3(quadSizeX, 0, quadSizeZ));
        Vector3[] vertices = new Vector3[4] { pointX0Z0, pointX1Z0, pointX0Z1, pointX1Z1 };
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.uv = uv;
        tempQuad = Instantiate(segmentPrefab, Vector3.zero, Quaternion.identity);
        // tempQuad.transform.Translate(); // transform.TransformPoint(vertices[0])
        tempQuad.GetComponent<MeshFilter>().mesh = mesh;
        tempQuad.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void CreateTerrainObject(/*GameObject segment,*/ int objects)
    {
        tempObj = Instantiate(terrainPrefabs[Random.Range(0, terrainPrefabs.Length)], Vector3.zero, Quaternion.identity); // tempQuad.transform.position, tempQuad.transform.rotation
        //Debug.Log(tempQuad.transform.localPosition);
        //Debug.Log(tempQuad.transform.localRotation);
        Debug.Log(tempObj.transform.localPosition);
        Debug.Log(tempObj.transform.localRotation);
        // tempObj.transform.SetParent(tempQuad.transform);
        // tempObj.transform.Translate(tempQuad.transform.position);
        // tempObj.transform.Translate(Vector3.right * Random.Range(0, quadSizeX) + Vector3.forward * Random.Range(0, quadSizeZ));
    }
}
