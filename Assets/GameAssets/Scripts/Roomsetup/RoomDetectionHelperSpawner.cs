using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetectionHelperSpawner : MonoBehaviour
{
    #region Public Variables
    public RoomDetectionHelper RoomDetectionHelperPref;
    #endregion

    #region Private Variables
    private RoomDetectionHelper _currentHelper;
    private Vector3 _initialOffset;
    private Vector3[] _positions = new Vector3[] {
        new Vector3(5f, -2f, 0), new Vector3(-5f, -2f, 0), new Vector3(0, -2f, 5f), new Vector3(0, -2f, -5f), new Vector3(0, -2f, 0),
        new Vector3(5f, 0f, 0), new Vector3(-5f, 0f, 0), new Vector3(0, 0f, 5f), new Vector3(0, 0f, -5f),
        new Vector3(5f, 2f, 0), new Vector3(-5f, 2f, 0), new Vector3(0, 2f, 5f), new Vector3(0, 2f, -5f), new Vector3(0, 2, 0)
    };
    private int _curPosition = 0;
    #endregion

    private void Awake()
    {
        _initialOffset = Camera.main.transform.position;
    }

    private void Start()
    {
        StartCoroutine(StartHelperSpawner());
    }

    IEnumerator StartHelperSpawner()
    {
        yield return new WaitForSeconds(5);
        SpawnNextRoomDetectionHelper();
    }

    private void Update()
    {
        if (_currentHelper == null)
            return;

        if (_currentHelper.IsCompleted())
        {
            Destroy(_currentHelper.gameObject);
            SpawnNextRoomDetectionHelper();
        }
    }

    private void SpawnNextRoomDetectionHelper()
    {
        Vector3 position = _initialOffset + _positions[_curPosition];
        Quaternion rotation = Quaternion.LookRotation(_initialOffset - position);

        Debug.DrawRay(position, (_initialOffset - position) * 3f, Color.red, 100);

        if(Physics.Raycast(_initialOffset, position - _initialOffset, out RaycastHit hit, 10f))
        {
            position = hit.point + hit.normal * 0.2f;
        }

        RoomDetectionHelper instance = Instantiate(RoomDetectionHelperPref, position, rotation, transform);
        _currentHelper = instance;

        _curPosition++;
        if (_curPosition == _positions.Length)
        {
            FindObjectOfType<RoomMapping>()?.ProceedToGroundSelection();
        }
    }
}
