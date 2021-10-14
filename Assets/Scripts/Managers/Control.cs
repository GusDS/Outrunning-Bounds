using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public GameObject player;
    public static Vector3 playerPosition;
    // private TrackMgr trackMgr;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //trackMgr = GameObject.Find("TrackMgr").GetComponent<TrackMgr>();
        //trackMgr = GameObject.Find("TrackMgr").GetComponent<TrackMgr>();
        //trackMgr.GenerateTrack(1000);
        //for (int i = 0; i < trackMgr.rowsMax; i++) trackMgr.NewSegmentsRow();
    }

    void Update()
    {
        playerPosition = player.transform.position;
    }
}
