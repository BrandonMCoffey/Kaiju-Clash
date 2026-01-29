using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class KaijuManager : MonoBehaviour
{
    [SerializeField] private List<DestroyPoint> _destroyPoints;
    [SerializeField] private LayerMask _destroyLayer;
    [SerializeField] private float _heightSpacingBetweenParticles = 8f;

    [Title("Camera")]
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _cameraAim;

    private Vector3 _cameraAimPosOffset;
    private Vector3 _cameraAimRotOffset;

    private Collider[] _raycastHits;

    private void Start()
    {
        _cameraAimPosOffset = _cameraAim.position - _head.position;
        _cameraAimRotOffset = _cameraAim.eulerAngles - new Vector3(0f, _head.eulerAngles.y, 0f);
        _raycastHits = new Collider[3];
    }

    private void Update()
    {
        var rot = Quaternion.Euler(_cameraAimRotOffset + new Vector3(0f, _head.eulerAngles.y, 0f));
        _cameraAim.SetPositionAndRotation(_head.position + _cameraAimPosOffset, rot);
    }

    private void FixedUpdate()
    {
        foreach (var point in _destroyPoints)
        {
            int hits = Physics.OverlapSphereNonAlloc(point.Source.TransformPoint(point.Offset), point.Radius, _raycastHits, _destroyLayer);
            for (int i = 0; i < hits; i++)
            {
                var destructible = _raycastHits[i].transform;
                int count = Mathf.RoundToInt(destructible.localScale.y / _heightSpacingBetweenParticles);
                for (int j = 0; j < count; j++)
                {
                    EffectsManager.Explode(destructible.position + (j + 0.5f) * _heightSpacingBetweenParticles * Vector3.up);
                }
                Destroy(destructible.gameObject);
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var point in _destroyPoints)
        {
            if (point.Source != null) Gizmos.DrawWireSphere(point.Source.TransformPoint(point.Offset), point.Radius);
        }
    }

    [System.Serializable]
    private class DestroyPoint
    {
        public Transform Source;
        public Vector3 Offset;
        public float Radius = 5;
    }
}
