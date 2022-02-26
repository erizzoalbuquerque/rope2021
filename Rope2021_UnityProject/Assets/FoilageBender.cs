using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FoilageBender : MonoBehaviour
{
    [SerializeField] float _maxBending = 0.75f;
    [SerializeField] float _duration = 1.5f;

    Material _material;

    void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        
    void Bend(float maxBending)
    {
        Vector4 bending = new Vector4(maxBending, 0f, 0f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => _material.GetVector("_Bending"), x => _material.SetVector("_Bending", x), bending, _duration/4f).SetEase(Ease.OutSine));
        seq.Append(DOTween.To(() => _material.GetVector("_Bending"), x => _material.SetVector("_Bending", x), Vector4.zero, _duration).SetEase(Ease.OutElastic));
        seq.Play();
    }

    [ContextMenu("Bend")]
    void BendTest()
    {
        print("Bend Test");
        Bend(_maxBending);
    }
}
