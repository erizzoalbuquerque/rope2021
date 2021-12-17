using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollsionWithTriggerCollider : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;

    [SerializeField] float _minSpeed = 0f;
    [SerializeField] float _maxSpeed = 10f;
    
    [SerializeField] float _impactMultiplier = 0.7f;

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
            Rigidbody2D otherRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRigidbody == null)
            {
                return;
            }

            float collisionSpeed = otherRigidbody.velocity.magnitude;

            if (collisionSpeed < _minSpeed)
            {
                return;
            }

            Vector2 collisionDirection = otherRigidbody.velocity.normalized;

            _rb.velocity = _rb.velocity + collisionDirection * Mathf.Clamp(collisionSpeed,0f,_maxSpeed) * _impactMultiplier;
        }
    }
}
