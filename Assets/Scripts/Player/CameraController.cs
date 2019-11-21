using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private GameObject _Player;

    [SerializeField]
    private float _LerpTime;

    [SerializeField]
    private Vector3 _Offset;

    void Start () {

        _Offset = transform.position - _Player.transform.position;

	}
	
	void LateUpdate () 
    {
        transform.position = Vector3.Lerp(transform.position, _Player.transform.position + _Offset, _LerpTime * Time.deltaTime);
    }
}
