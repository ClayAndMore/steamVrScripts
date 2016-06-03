using UnityEngine;
using System.Collections;
 
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class SteamVR_TestThrow : MonoBehaviour
{
        //预设,用于投掷的物体
        public GameObject prefab;
        //位于手柄上的刚体,也就是预设物体出现的地方
        public Rigidbody attachPoint;
 
        //追踪的设备,这里是我们的手柄
        SteamVR_TrackedObject trackedObj;
        //固定关节
        FixedJoint joint;
 
        void Awake()
        {
                //获取追踪的设备,即手柄
                trackedObj = GetComponent<SteamVR_TrackedObject>();
        }
 
        void FixedUpdate()
        {
                //获取手柄的输入,也就是用户的输入
                var device = SteamVR_Controller.Input((int)trackedObj.index);
                //如果关节为空 且 用户按下扳机
                if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                        //把预设实例化并设置其位置在手柄上的指定位置,这个指定位置就是手柄上的那个圈圈
                        var go = GameObject.Instantiate(prefab);
                        go.transform.position = attachPoint.transform.position;
 
                        //把关节组件添加到实例化的对象上,链接的位置就是这个刚体,添加这个组件的目的就是为了当你松开Trigger的时候分开手柄和预设物体
                        //这个FixedJoint组件实际上就是一个关节,作用是链接两个物体
                        joint = go.AddComponent<FixedJoint>();
                        joint.connectedBody = attachPoint;
                }
                //又如果关节不为空 且 手柄上的扳机Trigger松开的时候
                else if (joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                        //获取关节上的游戏对象,获取其刚体
                        var go = joint.gameObject;
                        var rigidbody = go.GetComponent<Rigidbody>();
                        //立即摧毁关节,并置为空
                        Object.DestroyImmediate(joint);
                        joint = null;
                        //15秒后摧毁该对象
                        Object.Destroy(go, 15.0f);
 
                        // We should probably apply the offset between trackedObj.transform.position
                        // and device.transform.pos to insert into the physics sim at the correct
                        // location, however, we would then want to predict ahead the visual representation
                        // by the same amount we are predicting our render poses.
                        //大概意思是:我们也许应该在正确的位置应用trackedObj.transform.position和device.transform.pos之间的偏移量到物理模拟中去
                        //然而,如果那样的话我们就想要预测和渲染动作同样数量的视觉位置
 
                        //原始位置有的话就是原始位置,没有的话取其父类
                        var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
                        if (origin != null)
                        {
                                //取其速度和角度
                                rigidbody.velocity = origin.TransformVector(device.velocity);
                                rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
                        }
                        else
                        {
                                rigidbody.velocity = device.velocity;
                                rigidbody.angularVelocity = device.angularVelocity;
                        }
                        //最大角速度
                        rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
                }
        }
}
