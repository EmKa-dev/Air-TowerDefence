using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    [SerializeField]
    private Transform _Target;

    [SerializeField]
    private Vector3 _OffsetPosition;

    [SerializeField]
    private bool _Damping;
    [SerializeField]
    private float _DampTime;

    private Vector3 _DesiredPosition;

    private void Start()
    {
        if (_Target == null)
        {
            Debug.LogWarning("Missing target ref !", this);

            return;
        }
    }

    void Update()
    {
        CalculatePosition();
    }

    private void LateUpdate()
    {
        MoveToPosition();
    }

    private void MoveToPosition()
    {
        transform.position = _DesiredPosition;
    }

    public void CalculatePosition()
    {
        // compute position
        Quaternion rot = Quaternion.Euler(_Target.transform.eulerAngles.x, _Target.transform.eulerAngles.y, 0);

        if (_Damping)
        {
            _DesiredPosition = Vector3.MoveTowards(transform.position, _Target.transform.position + (rot * _OffsetPosition), _DampTime * Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, _Target.transform.position + (rot * _OffsetPosition), _LerpTime * Time.deltaTime);
        }
        else
        {
            _DesiredPosition = _Target.transform.position + (rot * _OffsetPosition);
        }

        // compute rotation

        transform.rotation = _Target.rotation;

        
    }
}
