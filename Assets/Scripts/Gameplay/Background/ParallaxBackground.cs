using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // ----------------------
    //
    // Class for instantiating and moving a multi-layered parallax background inside of the level, based on the player's movement
    //
    // ----------------------

    [Header("Background Settings")]
    public List<Vector2> ScrollSpeeds = new List<Vector2>();

    // Background layers
    private List<List<RectTransform>> backgroundLayers = new List<List<RectTransform>>();
    private List<List<Vector3>> backgroundLayerOffsets = new List<List<Vector3>>();

    // Camera movement variables
    private Vector2 camPos;
    private Vector2 prevCamPos;
    private Quaternion camRot;

    private Vector2 cameraVelocity;
    private Vector2 cameraVelocityRelative;
    private int stationaryCamCheck;

    // Object references
    private LevelController level;
    private CinemachineVirtualCamera virtualCam;
    private Camera cam;

    private void Awake()
    {
        virtualCam = FindFirstObjectByType<CinemachineVirtualCamera>();
        cam = FindFirstObjectByType<Camera>();
    }

    private void Start()
    {
        level = LevelController.Instance;

        // Create 8 clone objects of each background layer, to ensure they cover the full view of the camera
        foreach (RectTransform layer in GetComponentsInChildren<RectTransform>())
        {
            List<RectTransform> layerList = new List<RectTransform>();
            List<Vector3> layerOffsetList = new List<Vector3>();

            layerList.Add(layer);
            layerOffsetList.Add(new Vector3(0, 0, 0));
            for (int i = 0; i < 8; i++)
            {
                layerList.Add(Instantiate(layer, transform));
                layerOffsetList.Add(new Vector3(0, 0, 0));
            }

            layerOffsetList[1] = new Vector3(layer.rect.width, 0, 0);
            layerList[1].position += layerOffsetList[1];
            layerOffsetList[2] = new Vector3(layer.rect.width, layer.rect.height, 0);
            layerList[2].position += layerOffsetList[2];
            layerOffsetList[3] = new Vector3(0, layer.rect.height, 0);
            layerList[3].position += layerOffsetList[3];
            layerOffsetList[4] = new Vector3(-layer.rect.width, layer.rect.height, 0);
            layerList[4].position += layerOffsetList[4];
            layerOffsetList[5] = new Vector3(-layer.rect.width, 0, 0);
            layerList[5].position += layerOffsetList[5];
            layerOffsetList[6] = new Vector3(-layer.rect.width, -layer.rect.height, 0);
            layerList[6].position += layerOffsetList[6];
            layerOffsetList[7] = new Vector3(0, -layer.rect.height, 0);
            layerList[7].position += layerOffsetList[7];
            layerOffsetList[8] = new Vector3(layer.rect.width, -layer.rect.height, 0);
            layerList[8].position += layerOffsetList[8];

            backgroundLayers.Add(layerList);
            backgroundLayerOffsets.Add(layerOffsetList);
        }

        MoveLayers();
    }

    private void OnEnable()
    {
        camPos = virtualCam.transform.position;
        prevCamPos = camPos;
    }

    public void LateUpdate()
    {
        // Get basic camera movement info
        camPos = cam.transform.position;
        camRot = cam.transform.rotation;
        if (camPos != prevCamPos) 
        {
            cameraVelocity = camPos - prevCamPos;
            stationaryCamCheck = 0;
        }
        else
        {
            stationaryCamCheck++;
            if (stationaryCamCheck > 10)
            {
                cameraVelocity = Vector2.zero;
            }
        }

        // Get the camera's velocity relative to the current direction of "up" in the level
        if (cameraVelocity == camPos)
        {
            cameraVelocityRelative = Vector2.zero;
        }
        else
        {
            cameraVelocityRelative = new Vector2(
                 Vector2.Dot(cameraVelocity, level.RightAxis.normalized),
                 Vector2.Dot(cameraVelocity, level.UpAxis.normalized));
        }

        // Move the background, and all of its individual layers based on their set speeds
        transform.position = camPos;
        transform.rotation = camRot;
        MoveLayers();

        prevCamPos = camPos;
    }

    private void MoveLayers()
    {
        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            for (int j = 0; j < backgroundLayers[i].Count; j++)
            {
                Vector2 newPos = 
                    backgroundLayers[i][j].localPosition
                    - (Vector3)(cameraVelocityRelative * (ScrollSpeeds[i] * Time.deltaTime));
                float width = backgroundLayers[i][j].rect.width;
                float height = backgroundLayers[i][j].rect.height;

                // Horizontal looping
                if (newPos.x >= backgroundLayerOffsets[i][j].x + (width / 2))
                {
                    newPos.x -= width;
                }
                if (newPos.x <= backgroundLayerOffsets[i][j].x - (width / 2))
                {
                    newPos.x += width;
                }

                // Vertical looping
                if (newPos.y >= backgroundLayerOffsets[i][j].y + (height / 2))
                {
                    newPos.y -= height;
                }
                if (newPos.y <= backgroundLayerOffsets[i][j].y - (height / 2))
                {
                    newPos.y += height;
                }

                backgroundLayers[i][j].localPosition = newPos;
            }
        }
    }
}
