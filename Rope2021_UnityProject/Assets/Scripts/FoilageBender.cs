using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FoilageBender : MonoBehaviour
{
    [SerializeField] float _maxBending = 0.75f;
    [SerializeField] float _duration = 1.5f;
    [SerializeField] float _maxBodySpeed = 20f;

    Material _material;
    bool _isBending = false;

    void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
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

        Vector4 bending = new Vector4(bendingAmount, 0f, 0f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => _material.GetVector("_Bending"), x => _material.SetVector("_Bending", x), bending, 0.3f).SetEase(Ease.OutSine));
        seq.Append(DOTween.To(() => _material.GetVector("_Bending"), x => _material.SetVector("_Bending", x), Vector4.zero, _duration).SetEase(Ease.OutElastic));
        seq.Play();
        seq.OnComplete(Reset);
    }

    void Reset()
    {
        //Do nothing about the material for now. Delete Instance if needed in the future.

        _isBending = false;
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
