using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public GameObject SpatialMapper;
    public GameObject MeshParent;
    public Material GroundMaterial;

    [HideInInspector] public float GroundHeight = 0;
    [HideInInspector] public float CeilingHeight = 0;

    [HideInInspector] public Vector3 ProgramSpacePosition = Vector3.zero;
    [HideInInspector] public Vector3 ProgramSpaceRotation = Vector3.zero;
    [HideInInspector] public Vector3 ProgramSpaceScaleMultiplier = Vector3.zero;
    [HideInInspector] public Vector3 ARIStartPosition = Vector3.zero;
    [HideInInspector] public Vector3 ARIStartRotation = Vector3.zero;

    private GameObject _ground;
    private Mesh _groundMesh;
    private List<GameObject> _realObstacles;

    void Awake()
    {
        if (Instance) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void SetGroundMesh(Mesh groundMesh)
    {
        _ground = new GameObject("Ground");
        _ground.transform.position = Vector3.zero;
        _ground.AddComponent<MeshFilter>().mesh = groundMesh;
        _ground.AddComponent<MeshRenderer>().material = GroundMaterial;
        _ground.AddComponent<MeshCollider>().sharedMesh = groundMesh;
        _ground.transform.SetParent(transform);

        _groundMesh = groundMesh;

        SpatialMapper.SetActive(false);
        MeshParent.SetActive(false);
    }
}
