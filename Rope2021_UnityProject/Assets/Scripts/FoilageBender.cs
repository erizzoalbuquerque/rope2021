using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FoilageBender : MonoBehaviour
{
    [SerializeField] float _maxBending = 0.75f;
    [SerializeField] float _duration = 1.5f;
    [SerializeField] float _maxBodySpeed = 20f;

    Material _sharedMaterial;
    Material _instanceMaterial;
    SpriteRenderer _sr;
    bool _isBending = false;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sharedMaterial = _sr.sharedMaterial;
    }

    private void Start()
    {
        _isBending = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        
    void Bend(float bendingAmount)
    {
        if (_isBending)
            return;

        _isBending = true;

        _instanceMaterial = _sr.material;

        Vector4 bending = new Vector4(bendingAmount, 0f, 0f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => _instanceMaterial.GetVector("_Bending"), x => _instanceMaterial.SetVector("_Bending", x), bending, 0.3f).SetEase(Ease.OutSine));
        seq.Append(DOTween.To(() => _instanceMaterial.GetVector("_Bending"), x => _instanceMaterial.SetVector("_Bending", x), Vector4.zero, _duration).SetEase(Ease.OutElastic));
        seq.Play();
        seq.OnComplete(Reset);
    }

    void Reset()
    {
        _sr.material = _sharedMaterial;
        _instanceMaterial = null;
        _isBending = false;
    }

    void CreateNewInstanceMaterial()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D body = collision.attachedRigidbody;
        if (body != null)
        {
            float direction;
            if (body.velocity.x >= 0) direction = 1f;
            else direction = -1;

            Bend(Mathf.Clamp01(body.velocity.magnitude / _maxBodySpeed) * _maxBending *  direction);
        }
    }

    [ContextMenu("Bend")]
    void BendTest()
    {
        print("Bend Test");
        Bend(_maxBending);
    }
}
