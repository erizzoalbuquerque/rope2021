using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CenterOfMassSetup : MonoBehaviour
{
    [SerializeField] Vector2 _centerOfMassPosition = Vector2.zero;
    [SerializeField] Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
            _rb.centerOfMass = _centerOfMassPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(this.transform.TransformPoint(_centerOfMassPosition),0.1f);
    }
}
