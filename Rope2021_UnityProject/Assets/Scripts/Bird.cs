using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

    public enum BirdState { IDLE, JUMPING, RUNNING }
    public BirdState state = BirdState.IDLE;

    public Vector3 gravity = new Vector3(0f,-10f,0f);
    public Vector3 _runAwayDirection = new Vector3(1f, 1f, 0f).normalized;

    SpriteRenderer _spriteRenderer;

    bool _canJump = true;
    bool _ranAway = false;

    float _nextJumpTimer = 1f;

    public float _maxTimeBetweenJump = 4f;
    public float _jumpRandomnessConstant = 3f;
    public float _maxHeight = 0.1f;
    public float _horizontalSpeed = 0.1f;
    public float _runSpeed = 1f;

    Vector3 _startPosition;

    // Use this for initialization
    void Start () {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startPosition = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case BirdState.IDLE:
                if (_ranAway)
                {
                    Reset();
                }
                break;
            case BirdState.JUMPING:
                if (!_ranAway)
                {
                    if (!_canJump)
                        return;

                    if (_nextJumpTimer <= 0)
                    {
                        StartCoroutine("Jump");
                        _nextJumpTimer = Mathf.Pow(Random.Range(0f, 1f), _jumpRandomnessConstant) * _maxTimeBetweenJump;
                    }
                    else
                    {
                        _nextJumpTimer -= Time.deltaTime;
                    }
                }
                break;
            case BirdState.RUNNING:
                if (!_ranAway)
                {
                    _ranAway = true;
                    StopCoroutine("Jump");
                    StartCoroutine("Run");
                }
                break;
        }      	
	}

    void Reset()
    {
        transform.position = _startPosition;
        _canJump = true;
        _ranAway = false;
        _nextJumpTimer = 1f;
        _spriteRenderer.enabled = true;
        StopAllCoroutines();
    }

    IEnumerator Jump()
    {
        _canJump = false;

        float horizontalSpeed;

        float dice = Random.Range(0f, 1f);

        if (dice > 0.66f)
            horizontalSpeed = _horizontalSpeed;
        else if (dice > 0.33)
            horizontalSpeed = -_horizontalSpeed;
        else
            horizontalSpeed = 0f;

        Vector3 startPosition = transform.position;
        
        Vector3 v0 = new Vector3(horizontalSpeed, Mathf.Sqrt( - 2 * gravity.y * _maxHeight ), 0f);
        Vector3 v = v0;

        v = v0;
        transform.position += v * Time.deltaTime;

        yield return null;

        while (transform.position.y >= startPosition.y)
        {
            v += gravity*Time.deltaTime;
            transform.position += v * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);

        _canJump = true;
        yield return null;
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.3f));

        Vector3 v = Vector3.zero;

        while (_spriteRenderer.isVisible)
        {
            v = 0.95f * v + 0.05f * _runAwayDirection * _runSpeed;

            transform.position += v;

            yield return null;
        }

        _spriteRenderer.enabled = false;

        yield return null;
    }

    public void SetRunAway(Vector3 diretion)
    {
        _runAwayDirection = diretion.normalized;
    }
}
