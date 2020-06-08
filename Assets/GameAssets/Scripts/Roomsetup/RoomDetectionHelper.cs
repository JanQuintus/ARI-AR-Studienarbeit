using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RoomDetectionHelper : MonoBehaviour
{

    #region Private Variables
    private ParticleSystem _ps;
    private float _progress = 0;
    private Transform _cameraTrans;
    #endregion

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        _cameraTrans = Camera.main.transform;
    }

    private void Update()
    {
        var main = _ps.main;
        main.startColor = Color.Lerp(Color.red, Color.green, _progress);

        transform.LookAt(_cameraTrans);

        float normalizedAngle = (180f - Vector3.Angle(transform.forward, _cameraTrans.forward)) / 180f;
        float distance = Vector3.Distance(transform.position, _cameraTrans.position);
        if (normalizedAngle < .1f && distance <= 10f)
        {
            distance /= 10f;
            _progress += Time.deltaTime * (1f - normalizedAngle) * (1.25f - distance) * 0.1f;
        }
    }

    public bool IsCompleted() => _progress >= 1f;
}
