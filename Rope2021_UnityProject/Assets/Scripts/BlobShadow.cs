using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobShadow : MonoBehaviour
{
    [SerializeField] Transform blobShadowQaud = null;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float offsetAboveTheGround= 0.001f;
    [SerializeField] LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (blobShadowQaud != null)
        {
            RaycastHit2D rc = Physics2D.Raycast(this.transform.position, Vector2.down,maxDistance,whatIsGround.value);

            if (rc.collider != null)
            {
                blobShadowQaud.gameObject.SetActive(true);

                float distanceFromOrigin = Mathf.Abs(rc.point.y - this.transform.position.y);

                blobShadowQaud.transform.position = rc.point + (rc.normal * offsetAboveTheGround);
                blobShadowQaud.transform.rotation = Quaternion.FromToRotation(Vector3.back,rc.normal);
                blobShadowQaud.transform.localScale = Vector3.one * (1 - distanceFromOrigin / maxDistance);
            }
            else
            {
                blobShadowQaud.gameObject.SetActive(false);
            }
        }
    }
}
