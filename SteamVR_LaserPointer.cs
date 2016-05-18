using UnityEngine;
using System.Collections;
 
//镭射光标事件结构体
public struct PointerEventArgs
{
    //控制器索引
    public uint controllerIndex;
    //标记
    public uint flags;
    //距离
    public float distance;
    //目标
    public Transform target;
}
 
//光标事件代理
public delegate void PointerEventHandler(object sender, PointerEventArgs e);
 
 
public class SteamVR_LaserPointer : MonoBehaviour
{
    //是否激活
    public bool active = true;
    //颜色
    public Color color;
    //厚度
    public float thickness = 0.002f;
    //载体
    public GameObject holder;
    //光标
    public GameObject pointer;
    bool isActive = false;
    //是否添加刚体
    public bool addRigidBody = false;
    //参考
    public Transform reference;
    //出入事件
    public event PointerEventHandler PointerIn;
    public event PointerEventHandler PointerOut;
 
    //之前接触的对象
    Transform previousContact = null;
 
    // Use this for initialization
    void Start ()
    {
        //初始化载体
        holder = new GameObject();
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
 
       //初始化光标: 正方体原型 父物体为载体 大小 位置 碰撞体 刚体 材质 颜色
        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if(collider)
            {
                Object.Destroy(collider);
            }
        }
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;
    }
 
    /// <summary>
    /// 引发光标进入事件
    /// </summary>
    /// <param name="e">E.</param>
    public virtual void OnPointerIn(PointerEventArgs e)
    {
        if (PointerIn != null)
            PointerIn(this, e);
    }
 
    /// <summary>
    /// 引发光标离开事件
    /// </summary>
    /// <param name="e">E.</param>
    public virtual void OnPointerOut(PointerEventArgs e)
    {
        if (PointerOut != null)
            PointerOut(this, e);
    }
 
 
    // Update is called once per frame
    void Update ()
    {
        //如果没有激活则激活之
        if (!isActive)
        {
            isActive = true;
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
 
        //距离
        float dist = 100f;
 
        //获取追踪控制器
        SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();
 
        //发射一条正前方向的射线 获取击中与否
        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);
 
        //之前有击中对象 且 不是之前击中的对象  引发离开之前击中对象事件
        if(previousContact && previousContact != hit.transform)
        {
            PointerEventArgs args = new PointerEventArgs();
            if (controller != null)
            {
                args.controllerIndex = controller.controllerIndex;
            }
            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        //击中 且不是之前击中的对象  引发光标进入当前对象事件
        if(bHit && previousContact != hit.transform)
        {
            PointerEventArgs argsIn = new PointerEventArgs();
            if (controller != null)
            {
                argsIn.controllerIndex = controller.controllerIndex;
            }
            argsIn.distance = hit.distance;
            argsIn.flags = 0;
            argsIn.target = hit.transform;
            OnPointerIn(argsIn);
            previousContact = hit.transform;
        }
        //没有击中
        if(!bHit)
        {
            previousContact = null;
        }
        //击中 且在有效范围
        if (bHit && hit.distance < 100f)
        {
            dist = hit.distance;
        }
 
        //控制器非空 且 控制器扳机按下 加粗  / 反之 不加粗
        if (controller != null && controller.triggerPressed)
        {
            pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
        }
        else
        {
            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
        }
        //更新位置
        pointer.transform.localPosition = new Vector3(0f, 0f, dist/2f);
    }
}
