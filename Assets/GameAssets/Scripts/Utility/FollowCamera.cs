using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowCamera : MonoBehaviour
{
    #region Public Variables
    public float Distance = 2f;
    public bool ResetOnFollowEnd = false;
    public float FollowSpeed = 5f;
    public Vector3 Offset = Vector3.zero;
    public bool FollowOnLoad = false;
    #endregion

    #region Private Variables
    private Vector3 _initialPosition;
    private bool _isFollowing = false;
    private Transform _cameraTransform;
    #endregion

    private void Awake()
    {
        _initialPosition = transform.position;
        _cameraTransform = Camera.main.transform;
        _isFollowing = FollowOnLoad;
    }

    private void Update()
    {
        if (!_isFollowing) return;

        float speed = Time.deltaTime * FollowSpeed;

        Vector3 pos = _cameraTransform.position + (_cameraTransform.forward * Distance) + Offset;
        gameObject.transform.position = Vector3.SlerpUnclamped(gameObject.transform.position, pos, speed);

        Quaternion rot = Quaternion.LookRotation(gameObject.transform.position - _cameraTransform.position);
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rot, speed);
    }

    public void StartFollow()
    {
        _isFollowing = true;
    }

    public void EndFollow()
    {
        _isFollowing = false;
        if (ResetOnFollowEnd)
        {
            transform.DOMove(_initialPosition, 0.5f);
            transform.position = _initialPosition;
        }
    }


}
