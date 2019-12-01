using UnityEngine;

public class AimingScript : MonoBehaviour
{
    [SerializeField]
    private GameObject AimMarker;

    private GameObject _OriginalMarker;

    private GameObject _CurrentMarker;

    private int _LayerMask;

    void Start()
    {
        _LayerMask = LayerMask.GetMask("Terrain");
        _CurrentMarker = Instantiate(AimMarker);
        _CurrentMarker.transform.localScale = Vector3.one / 2;
        _OriginalMarker = _CurrentMarker;
    }

    void Update()
    {
        this.RayCastFromCamera();
    }

    public void ChangeMarker(GameObject newmarker)
    {
        _OriginalMarker.gameObject.SetActive(false);

        _CurrentMarker = newmarker;
    }

    public void ChangeToOriginalMarker()
    {
        Destroy(_CurrentMarker);

        _CurrentMarker = _OriginalMarker;

        _OriginalMarker.SetActive(true);
    }

    private void RayCastFromCamera()
    {
        Ray ray = new Ray(this.transform.position, this.transform.position + transform.forward * 1000f);

        if (Physics.Raycast(ray, out RaycastHit hit, 10000f, _LayerMask))
        {
            _CurrentMarker.transform.position = hit.point;
            _CurrentMarker.transform.position = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
        }
    }
}
