using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

public class RoomToPrimitive : MonoBehaviour
{
    private enum STATE
    {
        SEL_CEILING_HEIGHT,
        SEL_GROUND_HEIGHT,
        FLOOD_FILL
    }

    #region Public Variables
    public int XMax = 10;
    public int ZMax = 10;
    public float YOffset = 0.1f;
    public float Step = 0.2f;
    public TMPro.TMP_Text InstructionText;
    public bool EnableDebug = true;
    public Material VisualizeOverlapBoxMat;
    public Material VisualizeOverlapBoxIntersectMat;
    #endregion

    #region Private Variables
    private MLInputController _controller;
    private STATE _state = STATE.SEL_CEILING_HEIGHT;
    private bool _triggerPressed = false;
    private Transform _cameraTrans;
    private int XGrid = 50;
    private int ZGrid = 50;
    #endregion

    private void Awake()
    {
        if (!(MLInput.Start().IsOk))
        {
            Debug.LogError("Error starting MLInput, disabling script.");
            return;
        }

        _controller = MLInput.GetController(MLInput.Hand.Left);

        _cameraTrans = Camera.main.transform;
        XGrid = Mathf.RoundToInt(Mathf.Ceil(XMax / Step));
        ZGrid = Mathf.RoundToInt(Mathf.Ceil(ZMax / Step));


        InstructionText.text = "Decke wählen";
    }

    private void OnDestroy()
    {
        if(_controller.Connected)
            MLInput.Stop();
    }

    private void Update()
    {
        if (!_controller.Connected)
            return;

        if (_state == STATE.SEL_CEILING_HEIGHT)
            SelectHeight(Vector3.down);
        if (_state == STATE.SEL_GROUND_HEIGHT)
            SelectHeight(Vector3.up);
    }

    private void SelectHeight(Vector3 direction)
    {
        if (!_triggerPressed && _controller.TriggerValue >= 0.6f)
        {
            _triggerPressed = true;
            if (Physics.Raycast(_controller.Position, _controller.Orientation * Vector3.forward, out RaycastHit hit, 10f))
            {
                if ((direction == Vector3.up && hit.point.y < _cameraTrans.position.y) 
                    || direction == Vector3.down && hit.point.y > _cameraTrans.position.y)
                {
                    if (Vector3.Angle(hit.normal, direction) < 10f)
                    {
                        if (direction == Vector3.down)
                        {
                            RoomManager.Instance.CeilingHeight = hit.point.y;
                            _state = STATE.SEL_GROUND_HEIGHT;
                            InstructionText.text = "Boden wählen";
                        }
                        if (direction == Vector3.up)
                        {
                            RoomManager.Instance.GroundHeight = hit.point.y;
                            _state = STATE.FLOOD_FILL;
                            FloodFill(hit.point);
                        }
                        _triggerPressed = false;
                    }
                }
            }
        }

        if (_triggerPressed && _controller.TriggerValue <= 0.2f)
            _triggerPressed = false;
    }

    private void FloodFill(Vector3 startPosition)
    {
        Vector3 origin = startPosition + new Vector3(0, YOffset + 0.01f, 0);

        int[,] filledGrid = new int[XGrid, ZGrid];
        FloodFillAlg(origin, filledGrid, 0, 0);

        Mesh groundMesh = GenerateMesh(startPosition, filledGrid, -1);
        groundMesh.name = "GroundMesh";

        RoomManager.Instance.SetGroundMesh(groundMesh);

        // TODO: Find Obstacles

        //SceneManager.LoadScene("ProgrammingTest");
    }

    private void FloodFillAlg(Vector3 origin, int[,] grid, int x, int z)
    {
        float roomHeight = RoomManager.Instance.CeilingHeight - RoomManager.Instance.GroundHeight;
        Vector3 position = origin + new Vector3(x * Step, roomHeight / 2f - (YOffset / 2f), z * Step);
        Vector3 halfExtends = new Vector3(Step / 2f, roomHeight / 2f - YOffset, Step / 2f);

        Collider[] colliders = Physics.OverlapBox(position, halfExtends);
        bool intersection = colliders.Length > 0;
        if(EnableDebug) VisualizeOverlapBox(position, halfExtends, intersection, 10f);

        int gridX = x + XGrid / 2;
        int gridZ = z + ZGrid / 2;

        grid[gridX, gridZ] = intersection ? 1 : -1;

        if (intersection) return;

        if (x + 1 <  XGrid / 2 && grid[gridX + 1, gridZ] == 0) FloodFillAlg(origin, grid, x + 1, z);
        if (x - 1 > -XGrid / 2 && grid[gridX - 1, gridZ] == 0) FloodFillAlg(origin, grid, x - 1, z);
        if (z + 1 <  ZGrid / 2 && grid[gridX, gridZ + 1] == 0) FloodFillAlg(origin, grid, x, z + 1);
        if (z - 1 > -ZGrid / 2 && grid[gridX, gridZ - 1] == 0) FloodFillAlg(origin, grid, x, z - 1);
    }

    private Mesh GenerateMesh(Vector3 center, int[,] grid, params int[] gridValues)
    {
        if (gridValues.Length == 0 || grid.Length == 0) return new Mesh();

        List<int> acceptValues = new List<int>(gridValues);
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        Vector3 halfExtends = new Vector3(Step / 2f, 0, Step / 2f);

        int index = 0;
        for (int x = 0; x < XGrid; x++)
        {
            for (int z = 0; z < ZGrid; z++)
            {
                if (acceptValues.Contains(grid[x, z]))
                {
                    int posX = x - XGrid / 2;
                    int posZ = z - ZGrid / 2;
                    Vector3 position = center + new Vector3(posX * Step, 0, posZ * Step);

                    Vector3[] currentFace = new Vector3[] {
                        new Vector3(position.x - halfExtends.x, position.y, position.z - halfExtends.z),
                        new Vector3(position.x + halfExtends.x, position.y, position.z - halfExtends.z),
                        new Vector3(position.x + halfExtends.x, position.y, position.z + halfExtends.z),
                        new Vector3(position.x - halfExtends.x, position.y, position.z + halfExtends.z)
                    };

                    vertices.Add(currentFace[0]);
                    vertices.Add(currentFace[1]);
                    vertices.Add(currentFace[2]);
                    vertices.Add(currentFace[3]);

                    triangles.Add(2 + index);
                    triangles.Add(1 + index);
                    triangles.Add(0 + index);
                    triangles.Add(3 + index);
                    triangles.Add(2 + index);
                    triangles.Add(0 + index);

                    index += 4;
                }
            }
        }


        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    private void VisualizeOverlapBox(Vector3 origin, Vector3 halfExtends, bool intersect, float destroyAfter)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = origin;
        cube.transform.localScale = halfExtends * 2f;
        cube.GetComponent<Renderer>().material = intersect ? VisualizeOverlapBoxIntersectMat : VisualizeOverlapBoxMat;
        DestroyImmediate(cube.GetComponent<Collider>());
        Destroy(cube, destroyAfter);
    }

}
