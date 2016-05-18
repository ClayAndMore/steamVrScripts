using UnityEngine;
using System.Collections;
 
//结构体  距离
public struct GazeEventArgs
{
    public float distance;
}
 
//公共代理 凝视事件句柄
public delegate void GazeEventHandler(object sender, GazeEventArgs e);
 
public class SteamVR_GazeTracker : MonoBehaviour
{
    //是否凝视
    public bool isInGaze = false;
    //句柄开关
    public event GazeEventHandler GazeOn;
    public event GazeEventHandler GazeOff;
    //视距
    public float gazeInCutoff = 0.15f;
    public float gazeOutCutoff = 0.4f;
 
    // Contains a HMD tracked object that we can use to find the user's gaze
    //包含一个我们可以用来寻找用户凝视的头显追踪器
    Transform hmdTrackedObject = null;
 
    // Use this for initialization
    void Start ()
    {
     
    }
 
    /// <summary>
    /// 引发正在凝视事件
    /// </summary>
    /// <param name="e">事件参数E,即上面的结构体距离.</param>
    public virtual void OnGazeOn(GazeEventArgs e)
    {
        //句柄不为空时添加该事件代理
        if (GazeOn != null)
            GazeOn(this, e);
    }
 
    /// <summary>
    /// 引发凝视结束事件
    /// </summary>
    /// <param name="e">E.</param>
    public virtual void OnGazeOff(GazeEventArgs e)
    {
        if (GazeOff != null)
            GazeOff(this, e);
    }
 
    // Update is called once per frame
    void Update ()
    {
        // If we haven't set up hmdTrackedObject find what the user is looking at
        //如果我们还没有设置用来寻找用户正在看什么的头显追踪器
        if (hmdTrackedObject == null)
        {
            //设置头显追踪器
            SteamVR_TrackedObject[] trackedObjects = FindObjectsOfType<SteamVR_TrackedObject>();
            foreach (SteamVR_TrackedObject tracked in trackedObjects)
            {
                if (tracked.index == SteamVR_TrackedObject.EIndex.Hmd)
                {
                    hmdTrackedObject = tracked.transform;
                    break;
                }
            }
        }
 
        //如果已经设置好了
        if (hmdTrackedObject)
        {
            //发射一条正前方向的射线
            Ray r = new Ray(hmdTrackedObject.position, hmdTrackedObject.forward);
            // 从被凝视的物体上新建一个正面向头显的平面
            Plane p = new Plane(hmdTrackedObject.forward, transform.position);
 
            //下面这一段是讲:如果从头显上发射的射线穿过平面,那么就是正在凝视,将凝视的距离赋值,并引发凝视事件
            float enter = 0.0f;
            if (p.Raycast(r, out enter))
            {
                Vector3 intersect = hmdTrackedObject.position + hmdTrackedObject.forward * enter;
                float dist = Vector3.Distance(intersect, transform.position);
                //Debug.Log("Gaze dist = " + dist);
                if (dist < gazeInCutoff && !isInGaze)
                {
                    isInGaze = true;
                    GazeEventArgs e;
                    e.distance = dist;
                    OnGazeOn(e);
                }
                else if (dist >= gazeOutCutoff && isInGaze)
                {
                    isInGaze = false;
                    GazeEventArgs e;
                    e.distance = dist;
                    OnGazeOff(e);
                }
            }
 
        }
 
    }
}
