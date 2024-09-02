using UnityEngine;

public class ObjectAttachment : MonoBehaviour
{
    public Transform Hand_Index1Transform;
    public Transform Hand_Index2Transform;
    public Transform Hand_Index3Transform;
    public Transform Hand_Middle3Transform;
    public GameObject SpawnedObject;

    public float attachmentDistance = 0.2f;
    public float objectOffsetFromHand = 0.15f;

    private bool isAttached = false;
    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        // 示例条件检查：手指是否弯曲等
        bool isMiddleFingerBent = true;
        bool isRingFingerBent = true;
        bool isPinkyFingerBent = true;

        if (isMiddleFingerBent && isRingFingerBent && isPinkyFingerBent && !isAttached)
        {
            // 计算手和对象之间的距离
            float distanceToHand = Vector3.Distance(Hand_Middle3Transform.position, SpawnedObject.transform.position);

            // 获取对象的 Collider
            MeshCollider meshCollider = SpawnedObject.GetComponent<MeshCollider>();

            // 计算食指平面的法线
            Vector3 v1 = Hand_Index2Transform.position - Hand_Index1Transform.position;
            Vector3 v2 = Hand_Index3Transform.position - Hand_Index2Transform.position;
            Vector3 normalDirection = Vector3.Cross(v1, v2).normalized;

            // 选择食指上的一个点作为平面上的一点
            Vector3 planePoint = Hand_Index2Transform.position;

            // 计算对象中心到平面的距离
            float distanceToPlane = Mathf.Abs(Vector3.Dot(normalDirection, SpawnedObject.transform.position - planePoint));

            if (meshCollider != null)
            {
                // 使用 Mesh Collider 的边界盒尺寸来近似对象的半径
                float objectRadius = meshCollider.bounds.extents.magnitude;

                // 计算对象表面到平面的距离
                distanceToPlane = Mathf.Max(0, distanceToPlane - objectRadius);
            }
            else
            {
                Debug.LogWarning("Object does not have a Collider component.");
            }

            if (distanceToPlane <= attachmentDistance)
            {
                isAttached = true;

                // 计算目标位置：距离平面15cm
                Vector3 targetPosition = planePoint + normalDirection * 0.15f;

                // 将对象附加到手部对象上
                SpawnedObject.transform.SetParent(Hand_Middle3Transform);

                // 将对象的局部位置设置为平面上的目标位置
                SpawnedObject.transform.position = Vector3.SmoothDamp(SpawnedObject.transform.position, targetPosition, ref velocity, 0.2f);

                // 重置对象的局部旋转
                SpawnedObject.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Hand_Index1Transform == null || Hand_Index2Transform == null || Hand_Index3Transform == null)
            return;

        // 计算食指平面的法线
        Vector3 v1 = Hand_Index2Transform.position - Hand_Index1Transform.position;
        Vector3 v2 = Hand_Index3Transform.position - Hand_Index2Transform.position;
        Vector3 normalDirection = Vector3.Cross(v1, v2).normalized;

        // 选择食指上的一个点作为平面上的一点
        Vector3 planePoint = Hand_Index2Transform.position;

        // 绘制法线
        Gizmos.color = Color.red;
        Gizmos.DrawLine(planePoint, planePoint + normalDirection * 0.2f); // 0.2f 是法线的长度，可以调整

        // 绘制平面的四个顶点，形成一个矩形
        Vector3 planeCorner1 = planePoint + Vector3.right * 0.1f;
        Vector3 planeCorner2 = planePoint + Vector3.forward * 0.1f;
        Vector3 planeCorner3 = planePoint + Vector3.right * -0.1f;
        Vector3 planeCorner4 = planePoint + Vector3.forward * -0.1f;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(planeCorner1, planeCorner2);
        Gizmos.DrawLine(planeCorner2, planeCorner3);
        Gizmos.DrawLine(planeCorner3, planeCorner4);
        Gizmos.DrawLine(planeCorner4, planeCorner1);
    }
}
