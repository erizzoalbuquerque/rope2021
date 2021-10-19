using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rope : MonoBehaviour {

    Rigidbody2D rigidBody;
    
    DistanceJoint2D distanceJoint2d;

    LineRenderer lineRenderer;

    public GameObject objectAttached;
	public Rigidbody2D connectedBody;
	public Vector3 localAnchorPoint;
	public Vector2 anchorPoint;

	public float maxDistance = 100f;
	public float maxSearchAngle;
	public float angleDiscretization;

	public float tangentBoost;
    public float ropeDistanceChangeConst;
    	
	Vector2 aimingDirection; 

	public bool ropeIsActive = false;
	
	bool showAimingDirection;

    public LayerMask canCollideWithRopeLayers;
    public LayerMask GrappableObjectLayers;

	Queue<Vector2> anchorsList;
    
    AudioSource audioSource;
    public AudioClip hookedClip;

    // Use this for initialization
    void Start () {

		distanceJoint2d = this.gameObject.GetComponent<DistanceJoint2D> ();

		lineRenderer = gameObject.GetComponent<LineRenderer> ();
		if (lineRenderer == null) {
			lineRenderer = gameObject.AddComponent<LineRenderer>();
		}

        rigidBody = this.gameObject.GetComponent<Rigidbody2D>();

		audioSource = GetComponent<AudioSource> ();

        anchorsList = new Queue<Vector2>();
    }

    void OnEnable()
    {


    }

	void OnDisable()
    {
        DisableRope();
    }

	// Update is called once per frame
	void Update () {

        bool joystickConnected = false;
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetJoystickNames()[i] != "")
            {
                joystickConnected = true;
                break;
            }
        }

        if (Input.GetButtonDown("Fire2"))
		{
            //lineRenderer.SetColors(Color.black, Color.black);
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;

            StopCoroutine ("DisplayFailedRope");

			if (ropeIsActive == false)
			{				 
				if (joystickConnected) {
					aimingDirection = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
				} else 
				{
                    //print(Input.mousePosition + "   " + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z))));
					aimingDirection = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z))) - rigidBody.transform.position;				
				}

                RaycastHit2D hit = new RaycastHit2D();
                bool firstRaycastBlockedBySomething = false;
                Vector2 firstPointHit = Vector2.zero;

				Vector2 searchDirection = aimingDirection;
				for (float searchAngle = 0.0f; searchAngle < maxSearchAngle; searchAngle += angleDiscretization)
				{
                    searchDirection = new Vector2(aimingDirection.x * Mathf.Cos(Mathf.Deg2Rad * searchAngle) - aimingDirection.y * Mathf.Sin(Mathf.Deg2Rad * searchAngle),
					                              aimingDirection.x * Mathf.Sin(Mathf.Deg2Rad * searchAngle) + aimingDirection.y * Mathf.Cos(Mathf.Deg2Rad * searchAngle));
					
					hit = Physics2D.Raycast(this.GetComponent<Rigidbody2D>().position, searchDirection, maxDistance, canCollideWithRopeLayers.value);
					if (hit.collider != null && ((GrappableObjectLayers | (1 << hit.collider.gameObject.layer)) == GrappableObjectLayers))
                    {
                        break;
                    }

                    searchDirection = new Vector2(aimingDirection.x * Mathf.Cos(Mathf.Deg2Rad * -searchAngle) - aimingDirection.y * Mathf.Sin(Mathf.Deg2Rad * -searchAngle),
					                              aimingDirection.x * Mathf.Sin(Mathf.Deg2Rad * -searchAngle) + aimingDirection.y * Mathf.Cos(Mathf.Deg2Rad * -searchAngle));
					
					hit = Physics2D.Raycast(this.GetComponent<Rigidbody2D>().position, searchDirection, maxDistance, canCollideWithRopeLayers);

                    if (hit.collider != null && ((GrappableObjectLayers | (1 << hit.collider.gameObject.layer)) == GrappableObjectLayers))
                    {
                        break;
                    }

                    if (searchAngle == 0f && hit.collider != null)
                    {
                        firstRaycastBlockedBySomething = true;
                        firstPointHit = hit.point; //Use it in case rope doesn't grab to anything and we need to show fail feedback
                    }

                }

				if (hit.collider != null && ((GrappableObjectLayers | (1 << hit.collider.gameObject.layer)) == GrappableObjectLayers)) {
                    objectAttached = hit.collider.gameObject;

                    connectedBody = objectAttached.GetComponent<Rigidbody2D> ();
					if (connectedBody != null) {
						distanceJoint2d.connectedBody = connectedBody;
						distanceJoint2d.anchor = Vector2.zero;
						distanceJoint2d.connectedAnchor = connectedBody.gameObject.transform.InverseTransformPoint (hit.point);

						distanceJoint2d.distance = ((Vector2)gameObject.transform.position - (Vector2)objectAttached.transform.TransformPoint (distanceJoint2d.connectedAnchor)).magnitude;
					} else {
						distanceJoint2d.anchor = Vector2.zero;
						distanceJoint2d.connectedAnchor = hit.point;
						distanceJoint2d.distance = ((Vector2)gameObject.transform.position - (Vector2)distanceJoint2d.connectedAnchor).magnitude;
					}

					ropeIsActive = true;
					distanceJoint2d.enabled = true;
					lineRenderer.enabled = true;

					audioSource.PlayOneShot (hookedClip, 2f);
				} else 
				{
					if (firstRaycastBlockedBySomething == false)
						lineRenderer.SetPosition(0,this.gameObject.transform.position +  ((Vector3) aimingDirection).normalized * maxDistance);
					else
						lineRenderer.SetPosition(0,firstPointHit);
						
					StartCoroutine ("DisplayFailedRope");
                }
			}
		}
		
		if (Input.GetButtonUp("Fire2"))
		{
			if (ropeIsActive == true)
			{
                DisableRope();
			}
		}
		
		if (ropeIsActive == true)
		{
			//CanPlayerSeeTarget ();
			if (connectedBody != null)
				anchorPoint = objectAttached.transform.TransformPoint(distanceJoint2d.connectedAnchor);
			else
				anchorPoint = distanceJoint2d.connectedAnchor;

			float distance = ((Vector2) gameObject.transform.position - anchorPoint).magnitude;
			if (distance < distanceJoint2d.distance)
				distanceJoint2d.distance = distance;

			lineRenderer.SetPosition(0,anchorPoint);
			lineRenderer.SetPosition(1,this.gameObject.transform.position);

			Vector2 inputVector = new Vector2 (Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
			Vector2 radialVersor = ((Vector2)gameObject.transform.position - anchorPoint).normalized;
			Vector2 tangencialVersor = new Vector2(radialVersor.y,-radialVersor.x);

            rigidBody.AddForce(tangencialVersor*(Vector2.Dot(inputVector,tangencialVersor))*tangentBoost *Time.deltaTime);

            //Protótipo de recolhimento manual de corda
            if (Mathf.Abs(Vector2.Dot(inputVector, radialVersor)) > Mathf.Cos(Mathf.Deg2Rad * 60f)) // Se angulo entre corda e input menor que ângulo...
            {
                //distanceJoint2d.distance += ropeDistanceChangeConst * Vector2.Dot(inputVector, radialVersor);
                rigidBody.AddForce(radialVersor * (Vector2.Dot(inputVector, radialVersor)) * ropeDistanceChangeConst * Time.deltaTime); // tentativa bem bundona para controlar melhor corda.
            }
        }
		
	}

    void DisableRope()
    {
        ropeIsActive = false;
        distanceJoint2d.enabled = false;
        distanceJoint2d.connectedBody = null;
        lineRenderer.enabled = false;
    }
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 gizmoPos3D = gameObject.transform.position + new Vector3(aimingDirection.x, aimingDirection.y, 0.0f).normalized * maxDistance;
        Gizmos.DrawLine(gameObject.transform.position, gizmoPos3D);

        if (ropeIsActive)
        {
            if (CanPlayerSeeTarget())
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(rigidBody.position, distanceJoint2d.connectedAnchor);
        }
	}

	IEnumerator DisplayFailedRope()
	{
        float secondsToFade = 0.3f;
        float timeWhenStarted = Time.time;

		lineRenderer.enabled = true;
		float alpha = 0.2f;

		Color color = new Color (1, 1, 1, alpha);

		while (Time.time - timeWhenStarted < secondsToFade) 
		{
			color = new Color (1f, 1f, 1f, alpha * (1f - (float)(Time.time - timeWhenStarted) / (float)secondsToFade));  
			//lineRenderer.SetColors (color, color);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            lineRenderer.SetPosition(1,this.gameObject.transform.position);
			yield return null;
		}

        lineRenderer.enabled = false;
	}

	bool CanPlayerSeeTarget()
	{
		RaycastHit2D hit = Physics2D.Raycast(rigidBody.position, distanceJoint2d.connectedAnchor - rigidBody.position, (distanceJoint2d.connectedAnchor - rigidBody.position).magnitude - 0.1f);

		if (hit.collider != null)
		{
			//Debug.Log ("I CANT SEE THE ROPE. Hit " + hit.collider.gameObject.name);
			return false;
		}
		else
		{
			//Debug.Log ("I CAN SEE THE ROPE");
			return true;
		}
	}
}
