using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingScript : MonoBehaviour
{
    [SerializeField]
    private GameObject AimMarker;

    void Start()
    {

        //AimMarker.GetComponent<Collider>().enabled = false;
        AimMarker.transform.localScale = Vector3.one / 3;
    }

    void Update()
    {
        this.RayCastFromCamera();
    }


    private void RayCastFromCamera()
    {
        Ray ray = new Ray(this.transform.position, this.transform.position + transform.forward * 1000f);

        if (Physics.Raycast(ray, out RaycastHit hit, 10000f, LayerMask.GetMask("Terrain")))
        {
            AimMarker.transform.position = hit.point;
            AimMarker.transform.position = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
        }
    }
}
