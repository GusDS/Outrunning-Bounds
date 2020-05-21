using System.Collections.Generic;
using UnityEngine;

public class TrackMgr : MonoBehaviour
{
    public Vector3 startingPoint;
    public GameObject trackPrefab;
    public GameObject grassPrefab;
    public GameObject[] terrainPrefabs;
    public int rowSegments = 5;
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
    private int curveDuration = 6;
    private int slopeDuration = 4;
    private int curveRandom = 0;
    private int slopeRandom = 0;
    private float curveAmount = 0;
    private float slopeAmount = 0;
    private Vector3 roadOffset = Vector3.zero;
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
        control.player.transform.position = startingPoint;
    }

    private void Update()
    {
        if (rowPoints[trackSegment].z < Control.playerPosition.z + (quadSizeZ * 25))
            NewSegmentsRow();
    }

    public void NewSegmentsRow()
    {
        if (curveDuration == 0)
        {
            if (Random.value > 0.3f) // chance of curve
            {
                curveDuration = Random.Range(3, 8); //(5, 11);
                curveRandom = Random.Range(1, 5);
                if      (curveRandom == (int)CurveTypes.smallRight) curveAmount = quadSizeX * 1.5f / curveDuration; // small = 1 width across N duration
                else if (curveRandom == (int)CurveTypes.smallLeft) curveAmount = quadSizeX * -1.5f / curveDuration;
                else if (curveRandom == (int)CurveTypes.sharpRight) curveAmount = quadSizeX * 3f / curveDuration; // sharp = 2 widht across N duration
                else if (curveRandom == (int)CurveTypes.sharpLeft) curveAmount = quadSizeX * -3f / curveDuration;
                curveOffset = Vector3.right * curveAmount;
            }
            else curveOffset = Vector3.zero;
        }
        else curveDuration--;

        if (slopeDuration == 0)
        {
            if (Random.value > 0.3f) // chance of slope
            {
                slopeDuration = Random.Range(5, 11);
                slopeRandom = Random.Range(1, 5);
                if      (slopeRandom == (int)SlopeTypes.smallDown) slopeAmount = quadSizeZ * -1f / slopeDuration; // small = 1 width across N duration
                else if (slopeRandom == (int)SlopeTypes.smallUp) slopeAmount = quadSizeZ * 1f / slopeDuration;
                else if (slopeRandom == (int)SlopeTypes.sharpDown) slopeAmount = quadSizeZ * -2f / slopeDuration; // sharp = 2 widht across N duration
                else if (slopeRandom == (int)SlopeTypes.sharpUp) slopeAmount = quadSizeZ * 2f / slopeDuration;
                slopeOffset = Vector3.up * slopeAmount;
            }
            else slopeOffset = Vector3.zero;
        }
        else slopeDuration--;

        // First, road calculations and drawing (later, ground points will be based on road points)
        roadOffset = (Vector3.forward * quadSizeZ) + curveOffset + slopeOffset;

        // NEW ROAD CURVES:
        //      10x points X and Z = 100x triangles/meshes, last 2 new points > rowPointsNew list
        //
        rowPointsNew[trackSegment] = rowPoints[trackSegment] + roadOffset;
        rowPointsNew[trackSegment + 1] = rowPoints[trackSegment + 1] + roadOffset;
        CreateQuadMesh(trackPrefab, rowPoints[trackSegment], rowPoints[trackSegment + 1], rowPointsNew[trackSegment], rowPointsNew[trackSegment + 1]);

        // Calculate right ground points and draw quads
        for (int i = trackSegment + 1; i < rowSegments + 1; i++) rowPointsNew[i] = rowPoints[i] + roadOffset;
        for (int i = trackSegment + 1; i < rowSegments; i++) {
            CreateQuadMesh(grassPrefab, rowPoints[i], rowPoints[i + 1], rowPointsNew[i], rowPointsNew[i + 1]);
            //CreateTerrainObject((i-trackSegment)/3, rowPoints[i], rowPoints[i + 1], rowPointsNew[i], rowPointsNew[i + 1]);
        }
        // Calculate left ground points and draw quads
        for (int i = trackSegment - 1; i >= 0; i--) rowPointsNew[i] = rowPoints[i] + roadOffset;
        for (int i = trackSegment - 1; i >= 0; i--) {
            CreateQuadMesh(grassPrefab, rowPoints[i], rowPoints[i + 1], rowPointsNew[i], rowPointsNew[i + 1]);
            //CreateTerrainObject((trackSegment-i)/3, rowPoints[i], rowPoints[i + 1], rowPointsNew[i], rowPointsNew[i + 1]);
        }

        //    // rowQuads.Add(tempQuad);
        for (int i = 0; i < rowSegments + 1; i++) rowPoints[i] = rowPointsNew[i];
    }
    public void CreateQuadMesh(GameObject segmentPrefab, Vector3 pointX0Z0, Vector3 pointX1Z0, Vector3 pointX0Z1, Vector3 pointX1Z1)
    {
        Vector3[] vertices = new Vector3[4] { pointX0Z0, pointX1Z0, pointX0Z1, pointX1Z1 };
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.uv = uv;
        tempQuad = Instantiate(segmentPrefab, Vector3.zero, Quaternion.identity);
        tempQuad.GetComponent<MeshFilter>().mesh = mesh;
        tempQuad.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void CreateTerrainObject(int objects, Vector3 pointX0Z0, Vector3 pointX1Z0, Vector3 pointX0Z1, Vector3 pointX1Z1)
    {
        for (int i = 0; i < objects; i++)
        {
            int randomObj = Random.Range(0, terrainPrefabs.Length);
            tempObj = Instantiate(terrainPrefabs[randomObj], Vector3.zero, Quaternion.identity);
            tempObj.transform.SetParent(tempQuad.transform);
            //tempObj.transform.Translate(pos + Vector3.right * Random.Range(0, quadSizeX) + Vector3.forward * Random.Range(0, quadSizeZ));
            tempObj.transform.Translate(pointX0Z0 + (pointX0Z1 - pointX0Z0) * Random.value + (pointX1Z0 - pointX0Z0) * Random.value);

            if (randomObj == 0) // StoneFlat
            {
                tempObj.transform.localScale = Vector3.one * Random.Range(1f, 5f);
                tempObj.transform.Translate(Vector3.up * Random.Range(-1f, 0));
                tempObj.transform.Rotate(Vector3.up * Random.Range(0, 360));
                tempObj.GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 97), 20, 20, 255);
            }
            else // TreeRound
            {
                tempObj.transform.localScale = Vector3.one * Random.Range(1f, 5f);
                tempObj.transform.Translate(Vector3.up * Random.Range(0, -2.5f));
                tempObj.transform.Rotate(Vector3.up * Random.Range(0, 360));
                tempObj.GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(200, 256), (byte)Random.Range(0, 100), 255);
            }
        }
    }
}
