using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingScript : MonoBehaviour
{
    [SerializeField]
    private GameObject AimMarker;

    private GameObject _Marker;

    private int _LayerMask;

    void Start()
    {
        _LayerMask = LayerMask.GetMask("Terrain");
        //AimMarker.GetComponent<Collider>().enabled = false;
        _Marker = Instantiate(AimMarker);
        _Marker.transform.localScale = Vector3.one / 2;
    }

    void Update()
    {
        this.RayCastFromCamera();
    }


    private void RayCastFromCamera()
    {
        Ray ray = new Ray(this.transform.position, this.transform.position + transform.forward * 1000f);

        if (Physics.Raycast(ray, out RaycastHit hit, 10000f, _LayerMask))
        {
            _Marker.transform.position = hit.point;
            _Marker.transform.position = new Vector3(hit.point.x, hit.point.y + 0.3f, hit.point.z);
        }
    }
}
