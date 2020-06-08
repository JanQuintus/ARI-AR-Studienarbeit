using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{

    public Vector3 Center;

    #region Private Variables
    private Transform _cameraTrans;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _cameraTrans = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetDirection = (transform.position + Center) - _cameraTrans.position;
        transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + Center, .02f);
    }
}
