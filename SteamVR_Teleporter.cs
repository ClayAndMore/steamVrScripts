using UnityEngine;
using System.Collections;

public class SteamVR_Teleporter : MonoBehaviour
{
    //枚举传送类型
    public enum TeleportType
    {
        TeleportTypeUseTerrain,
        TeleportTypeUseCollider,
        TeleportTypeUseZeroY
    }

    //是否点击传送
    public bool teleportOnClick = false;
    //默认传送类型
    public TeleportType teleportType = TeleportType.TeleportTypeUseZeroY;

    //参照物
    Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }

    void Start()
    {
        //获取SteamVR_TrackedController追踪控制器组件
        var trackedController = GetComponent<SteamVR_TrackedController>();
        //没有则添加之
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        //添加扳机点击事件句柄
        trackedController.TriggerClicked += new ClickedEventHandler(DoClick);

        //如果是地域传送
        if (teleportType == TeleportType.TeleportTypeUseTerrain)
        {
            // Start the player at the level of the terrain
            //将玩家传送到该地域
            var t = reference;
            if (t != null)
                t.position = new Vector3(t.position.x, Terrain.activeTerrain.SampleHeight(t.position), t.position.z);
        }
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    void DoClick(object sender, ClickedEventArgs e)
    {
        if (teleportOnClick)
        {
            var t = reference;
            if (t == null)
                return;

            float refY = t.position.y;

            //平面和射线
            Plane plane = new Plane(Vector3.up, -refY);
            Ray ray = new Ray(this.transform.position, transform.forward);

            //是否是目标地 距离
            bool hasGroundTarget = false;
            float dist = 0f;
            //如果传送类型为地域传送
            if (teleportType == TeleportType.TeleportTypeUseTerrain)
            {
                RaycastHit hitInfo;
                TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
                //该目标是否在传送范围内
                hasGroundTarget = tc.Raycast(ray, out hitInfo, 1000f);
                dist = hitInfo.distance;
            }
            else if (teleportType == TeleportType.TeleportTypeUseCollider)
            {
                RaycastHit hitInfo;
                Physics.Raycast(ray, out hitInfo);
                dist = hitInfo.distance;
            }
            else
            {
                hasGroundTarget = plane.Raycast(ray, out dist);
            }

            //传送后重新校正位置
            if (hasGroundTarget)
            {
                Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
                t.position = ray.origin + ray.direction * dist - new Vector3(t.GetChild(0).localPosition.x, 0f, t.GetChild(0).localPosition.z) - headPosOnGround;
            }
        }
    }
}
