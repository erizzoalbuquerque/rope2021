using System;
using System.Collections;
using UnityEngine;

public class CameraController: MonoBehaviour
{
    public PlayerManager playerManager;
    
    public Transform target;
    public float damping = 1;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;

	private Camera cameraComponent;
	[HideInInspector]
	public float startSize = 19f;
	private float sizeChangeRate = 0f;
	public float cameraChangeTime = 1.5f;
	public float currentTargetSize; 

    private float m_OffsetZ;
    private Vector3 m_LastTargetPosition;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_LookAheadPos;

    // Use this for initialization
    private void Start()
    {
        m_LastTargetPosition = target.position;
        m_OffsetZ = (transform.position - target.position).z;
        transform.parent = null;

		cameraComponent = GetComponent<Camera> ();
		startSize = target.position.z;
		currentTargetSize = startSize;
    }


	public void ChangeCameraSize(float newSize)
	{		
		//cameraComponent.orthographicSize =  Mathf.SmoothDamp (cameraComponent.orthographicSize, newSize, ref sizeChangeRate, cameraChangeTime);

        float targetZ = Mathf.SmoothDamp(target.position.z, newSize, ref sizeChangeRate, cameraChangeTime);
        target.position = new Vector3(target.position.x, target.position.y, targetZ);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        FollowTarget();
    }

	void Update()
	{
        currentTargetSize += Input.mouseScrollDelta.y;

		ChangeCameraSize (currentTargetSize);
	}

    private void FollowTarget()
    {
        if (playerManager.IsPlayerAlive())
        {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }

    IEnumerator ShakeMovement(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        Vector3 originalCamPos = Camera.main.transform.position;

        while (elapsed < duration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = UnityEngine.Random.value * 2.0f - 1.0f;
            float y = UnityEngine.Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            Camera.main.transform.position = originalCamPos + new Vector3(x, y, 0f);

            yield return null;
        }

        Camera.main.transform.position = originalCamPos;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeMovement( duration, magnitude));
    }
}
