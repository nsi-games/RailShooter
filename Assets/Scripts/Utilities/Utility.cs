using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Vector3 GetPositionAtDistance(this Camera cam, float distance)
    {
        return cam.transform.position + cam.transform.forward * distance;
    }

    public static Vector3 GetSizeAtDistance(this Camera cam, float distance, float margin)
    {
        float frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * cam.aspect;

        frustumHeight -= margin;
        frustumWidth -= margin;
        
        return new Vector3(frustumWidth, frustumHeight);
    }

    public static void DrawPlane(this Camera cam, Vector3 position, Vector3 size, Color color)
    {
        Vector3 halfSize = size * .5f;
        Vector3 right = cam.transform.right;
        Vector3 up = cam.transform.up;

        Vector3 bottomLeft = position - right * halfSize.x - up * halfSize.y;
        Vector3 topRight = position + right * halfSize.x + up * halfSize.y;
        Vector3 bottomRight = position + right * halfSize.x - up * halfSize.y;
        Vector3 topLeft = position - right * halfSize.x + up * halfSize.y;

        Gizmos.color = color;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(topLeft, 1f);
        Gizmos.DrawSphere(topRight, 1f);
        Gizmos.DrawSphere(bottomRight, 1f);
        Gizmos.DrawSphere(bottomLeft, 1f);
    }

    public static void DrawFrustumPlaneAtDistance(this Camera cam, Vector3 position, float distance, float margin)
    {
        Vector3 sizeMargin = cam.GetSizeAtDistance(distance, margin);
        Vector3 sizeNoMargin = cam.GetSizeAtDistance(distance, 0f);

        DrawPlane(cam, position, sizeMargin, Color.red);
        DrawPlane(cam, position, sizeNoMargin, Color.blue);
    }

    public static void DrawFrustumPlaneAtDistance(this Camera cam, float distance, float margin)
    {
        Vector3 position = cam.GetPositionAtDistance(distance);
        Vector3 sizeMargin = cam.GetSizeAtDistance(distance, margin);
        Vector3 sizeNoMargin = cam.GetSizeAtDistance(distance, 0f);

        DrawPlane(cam, position, sizeMargin, Color.red);
        DrawPlane(cam, position, sizeNoMargin, Color.blue);
    }

    public static Vector3 PositionInBounds(this Camera cam, Vector3 position, float distance, float margin)
    {
        Vector3 localPosition = cam.transform.InverseTransformPoint(position);
        Vector3 localSize = cam.GetSizeAtDistance(distance, margin);
        Vector3 finalPos = localPosition;

        Vector3 halfSize = localSize * .5f;
        Vector3 right = Vector3.right;
        Vector3 up = Vector3.up;

        Vector3 min = -right * halfSize.x - up * halfSize.y;
        Vector3 max = right * halfSize.x + up * halfSize.y;
        
        if (finalPos.x < min.x) finalPos.x = min.x; // Check left
        if (finalPos.x > max.x) finalPos.x = max.x; // Check right
        if (finalPos.y < min.y) finalPos.y = min.y; // Check up
        if (finalPos.y > max.y) finalPos.y = max.y; // Check down

        finalPos.z = distance;
        
        //if (finalPos.z < min.z) finalPos.z = min.z; // Check back
        //if (finalPos.z > max.z) finalPos.z = max.z; // Check forward

        return cam.transform.TransformPoint(finalPos);
    }

    public static Color RGB(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}
