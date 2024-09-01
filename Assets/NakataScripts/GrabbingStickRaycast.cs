using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System;

public class GrabbingStickRaycast : MonoBehaviour
{
    [SerializeField] Transform thumbProximal;
    [SerializeField] Transform thumbTip;
    [SerializeField] Transform thumbMiddle;
    [SerializeField] Transform rightHandCenter;
    [SerializeField] Transform leftHandCenter;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] Transform[] normalPostions;

    [SerializeField] float calibrationDistance = 2f;
    [SerializeField] float detectionRange = 0.5f; // New: Range within the tip of the ray to detect objects

    public Action<GameObject, Vector3> onStickHit;
    public bool isDebugMode;

    void Update()
    {
        // Calculate normal direction
        Vector3 normal = CalculatePlaneNormal(normalPostions[0].position, normalPostions[1].position, normalPostions[2].position);
        Vector3 rayStart = thumbMiddle.position;
        Vector3 rayEnd = thumbMiddle.position + normal * calibrationDistance;

        if (isDebugMode)
        {
            lineRenderer.SetPosition(0, normalPostions[0].position);
            lineRenderer.SetPosition(1, normalPostions[1].position);
            lineRenderer.SetPosition(2, normalPostions[2].position);
            lineRenderer.SetPosition(3, rayStart);
            lineRenderer.SetPosition(4, rayEnd);
        }

        // Perform raycast
        if (Physics.Raycast(rayStart, normal, out RaycastHit hit, calibrationDistance))
        {
            float hitDistance = Vector3.Distance(rayStart, hit.point);
            if (hitDistance >= calibrationDistance - detectionRange)
            {
                textMeshProUGUI.SetText(hit.collider.gameObject.name);
                onStickHit?.Invoke(hit.collider.gameObject, normal * calibrationDistance);
            }
        }
    }

    Vector3 CalculatePlaneNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Calculate vectors from the points
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;

        // Calculate the normal using the cross product
        return Vector3.Cross(v2, v1).normalized;
    }
}
