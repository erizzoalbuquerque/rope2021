using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class BirdsManager : MonoBehaviour {

    Bounds _outerBound;
    Bounds _innerBound;

    bool _ranAway = false;

    public float _vOuterBoundSize = 10f , _hOuterBoundSize = 10f;
    public float _vInnerBoundSize = 5f, _hInnerBoundSize = 5f;

    Protagonist _player;

    List<Bird> _birds = new List<Bird>();

    public float _totalVolume = 1f;

    public AudioClip _birdFlappingSFX;

    // Use this for initialization
    void Start () {
        _outerBound = new Bounds(transform.position, new Vector3(_hOuterBoundSize, _vOuterBoundSize, 100f));
        _innerBound = new Bounds(transform.position, new Vector3(_hInnerBoundSize, _vInnerBoundSize, 100f));

        _player = FindObjectOfType<Protagonist>();

        _birds.Clear();
        _birds.AddRange(GetComponentsInChildren<Bird>());
    }
	
	// Update is called once per frame
	void LateUpdate () {
        _outerBound.size = new Vector3(_hOuterBoundSize, _vOuterBoundSize, 100f);
        _innerBound.size = new Vector3(_hInnerBoundSize, _vInnerBoundSize, 100f);

        

        if(_outerBound.Contains(_player.transform.position) == false)
        {
            //print("something fora " + _outerBound.min.x + " " + _outerBound.max.x + " " + _player.transform.position.x);
            _ranAway = false;
            foreach (Bird b in _birds)
            {
                b.state = Bird.BirdState.IDLE;
            }
        }
        else if (_outerBound.Contains(_player.transform.position) == true && _innerBound.Contains(_player.transform.position) == false )
        {
            //print("something meio");
            foreach (Bird b in _birds)
            {
                b.state = Bird.BirdState.JUMPING;
            }
        }
        else
        {
            //print("dentro");
            if (_ranAway == false)
            {
                _ranAway = true;
                var sortedBirds = _birds.OrderBy(o => - Mathf.Abs(o.transform.position.x - _player.transform.position.x));

                float minAngle = 20f;
                float maxAngle = 60f;
                

                float angle = minAngle;
                float delta = (maxAngle - minAngle) / ((float)_birds.Count + 1);

                foreach (Bird b in sortedBirds)
                {
                    float signal;

                    if (_player.transform.position.x < b .transform.position.x)
                    {
                        signal = 1f;
                    }
                    else
                    {
                        signal = -1f;
                    }

                    angle += delta;
                    //Debug.Log(angle);
                    b.SetRunAway(new Vector3( signal * Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f));
                    b.state = Bird.BirdState.RUNNING;
                }

                GetComponent<AudioSource>().PlayOneShot(_birdFlappingSFX, _totalVolume);
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, _outerBound.size);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(this.transform.position, _innerBound.size);
    }
}
