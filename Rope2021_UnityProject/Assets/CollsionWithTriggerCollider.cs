using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollsionWithTriggerCollider : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;

    [SerializeField] float _minSpeed = 0f;
    [SerializeField] float _maxSpeed = 1f;
    
    [SerializeField] float _forceAtMaxSpeed = 1000f;

    [SerializeField] LayerMask _canCollideLayers = default;

    
    void Awake()
    {
        if (this.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            _rb = this.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //print("Collided with: " + collision.gameObject.name);
        if (collision.isTrigger)
        {
            return;
        }

        if (_canCollideLayers == (_canCollideLayers | (1 << collision.gameObject.layer)))
        {
            //print("Applying forces.");
            Rigidbody2D collisionRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (collisionRb == null)
            {
                return;
            }

            float force = _forceAtMaxSpeed * (collisionRb.velocity.magnitude - _minSpeed) / (_maxSpeed - _minSpeed);
            force = Mathf.Clamp(force, 0f, _forceAtMaxSpeed);           

            Vector3 forceDirection = collisionRb.velocity.normalized;

            //print("force added to Rigidbody: " + (force * forceDirection * Time.deltaTime));
            _rb.AddForceAtPosition(force * forceDirection * Time.deltaTime,collisionRb.position);
        }
    }
}
