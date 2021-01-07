using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceMovement : MonoBehaviour {

    public class Movable
    {
        public GameObject GO { get; private set; }
        public float RestAngle { get; private set; }
        public float Speed { get;set; }

        public Movable(GameObject go, float restAngle = 0f)
        {
            GO = go;
            RestAngle = restAngle;
        }
    }

	List<Movable> _movables = new List<Movable>();

    public List<GameObject> _movableParts = new List<GameObject>();

    public float _spring = 1000f;
    public float _disspation = 2f;
    public float _minSpeedToCollide = 1f;
    public float _maxSpeedOnColission = 10f;

    bool _isSleeping;
    

	// Use this for initialization
	void Start () {

        _isSleeping = true;

        if (_movableParts.Count == 0)
            _movables.Add(new Movable(this.gameObject));
        else
        {
            foreach (GameObject go in _movableParts)
                _movables.Add(new Movable(go));
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (_isSleeping)
            return;

        for (int i = 0; i < _movables.Count; i++)
        {
            if (Mathf.Abs(_movables[i].Speed) < 0.01f && (Mathf.Abs(_movables[i].GO.transform.localRotation.z - _movables[i].RestAngle) < 0.01))
            {
                _movables[i].Speed = 0f;
                _movables[i].GO.transform.localRotation = Quaternion.Euler(0f, 0f, _movables[i].RestAngle);
                _isSleeping = true;
            }
            else
            {
                _movables[i].Speed += (-_spring * _movables[i].GO.transform.localRotation.z - _disspation * _movables[i].Speed) * Time.deltaTime;
                _movables[i].GO.transform.Rotate(Vector3.forward, _movables[i].Speed * Time.deltaTime,Space.Self);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float collisionSpeed = collision.attachedRigidbody.velocity.magnitude;
        float signal = (collision.attachedRigidbody.velocity.x > 0f)? 1 : -1;

        if (collisionSpeed > _minSpeedToCollide)
        {
            collisionSpeed *= Random.Range(0.4f, 1f);

            for (int i = 0; i < _movables.Count; i++)
                _movables[i].Speed += signal * Mathf.Max(collisionSpeed, _maxSpeedOnColission);

            _isSleeping = false;
        }
    }
}
