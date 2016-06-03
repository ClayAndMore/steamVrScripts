using UnityEngine;
using Valve.VR;
 
//结构体,点击事件参数
public struct ClickedEventArgs
{
//控制器索引
public uint controllerIndex;
//标记
public uint flags;
//控制板上的坐标
public float padX, padY;
}
 
//委托,点击事件句柄
public delegate void ClickedEventHandler(object sender, ClickedEventArgs e);
 
public class SteamVR_TrackedController : MonoBehaviour
{
//控制器索引
public uint controllerIndex;
//控制器状态
public VRControllerState_t controllerState;
//按下扳机与否
public bool triggerPressed = false;
//这个是正面最下方的按钮,对应Steam系统
public bool steamPressed = false;
//这个是最上方的菜单按钮
public bool menuPressed = false;
//这个pad控制板是中间的圆形触摸区域,功能比较多
public bool padPressed = false;
public bool padTouched = false;
//这个是负责判断是否握住了手柄
public bool gripped = false;
 
//菜单点击事件句柄
public event ClickedEventHandler MenuButtonClicked;
public event ClickedEventHandler MenuButtonUnclicked;
//扳机扣动事件句柄
public event ClickedEventHandler TriggerClicked;
public event ClickedEventHandler TriggerUnclicked;
//Steam点击事件句柄
public event ClickedEventHandler SteamClicked;
//触摸板点击事件句柄
public event ClickedEventHandler PadClicked;
public event ClickedEventHandler PadUnclicked;
//触摸板触摸事件句柄
public event ClickedEventHandler PadTouched;
public event ClickedEventHandler PadUntouched;
//抓取事件句柄
public event ClickedEventHandler Gripped;
public event ClickedEventHandler Ungripped;
 
// Use this for initialization
void Start()
{
//如果没有SteamVR_TrackedObject组件,则添加该组件
if (this.GetComponent<SteamVR_TrackedObject>() == null)
{
gameObject.AddComponent<SteamVR_TrackedObject>();
}
 
//索引赋值
this.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
//如果有SteamVR_RenderModel组件则对该组件索引进行赋值
if (this.GetComponent<SteamVR_RenderModel>() != null)
{
this.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
}
}
 
/// <summary>
/// 引发扳机按下事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnTriggerClicked(ClickedEventArgs e)
{
if (TriggerClicked != null)
TriggerClicked(this, e);
}
 
/// <summary>
/// 引发扳机松开事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnTriggerUnclicked(ClickedEventArgs e)
{
if (TriggerUnclicked != null)
TriggerUnclicked(this, e);
}
 
/// <summary>
/// 引发菜单点击事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnMenuClicked(ClickedEventArgs e)
{
if (MenuButtonClicked != null)
MenuButtonClicked(this, e);
}
 
/// <summary>
/// 引发菜单松开事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnMenuUnclicked(ClickedEventArgs e)
{
if (MenuButtonUnclicked != null)
MenuButtonUnclicked(this, e);
}
 
/// <summary>
/// 引发系统按钮点击事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnSteamClicked(ClickedEventArgs e)
{
if (SteamClicked != null)
SteamClicked(this, e);
}
 
/// <summary>
/// 引发触摸板点击事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnPadClicked(ClickedEventArgs e)
{
if (PadClicked != null)
PadClicked(this, e);
}
 
/// <summary>
/// 引发触摸板未点击事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnPadUnclicked(ClickedEventArgs e)
{
if (PadUnclicked != null)
PadUnclicked(this, e);
}
 
/// <summary>
/// 引发触摸板触摸事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnPadTouched(ClickedEventArgs e)
{
if (PadTouched != null)
PadTouched(this, e);
}
 
/// <summary>
/// 引发触摸板没有触摸事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnPadUntouched(ClickedEventArgs e)
{
if (PadUntouched != null)
PadUntouched(this, e);
}
 
/// <summary>
/// 引发握紧事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnGripped(ClickedEventArgs e)
{
if (Gripped != null)
Gripped(this, e);
}
 
/// <summary>
/// 引发未握紧事件
/// </summary>
/// <param name="e">E.</param>
public virtual void OnUngripped(ClickedEventArgs e)
{
if (Ungripped != null)
Ungripped(this, e);
}
 
// Update is called once per frame
void Update()
{
//OpenVR是在GitHub上开源的,详情见之前发的资源帖,此处引用
var system = OpenVR.System;
//系统不为空 且 成功获取手柄控制器状态
if (system != null && system.GetControllerState(controllerIndex, ref controllerState))
{
//手柄状态中的无符号64位整数按钮按下 且 扳机按钮左移一个单位(此处要参考OpenVR)
ulong trigger = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Trigger));
//针对扳机点击事件属性的一系列赋值
if (trigger > 0L && !triggerPressed)
{
triggerPressed = true;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnTriggerClicked(e);
 
}
else if (trigger == 0L && triggerPressed)
{
triggerPressed = false;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnTriggerUnclicked(e);
}
 
//同上,紧握赋值
ulong grip = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_Grip));
//同上,针对紧握事件属性的一系列赋值
if (grip > 0L && !gripped)
{
gripped = true;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnGripped(e);
 
}
else if (grip == 0L && gripped)
{
gripped = false;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnUngripped(e);
}
 
//同上,触摸板按下事件赋值
ulong pad = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
if (pad > 0L && !padPressed)
{
padPressed = true;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnPadClicked(e);
}
else if (pad == 0L && padPressed)
{
padPressed = false;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnPadUnclicked(e);
}
 
//同上,菜单赋值
ulong menu = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_ApplicationMenu));
if (menu > 0L && !menuPressed)
{
menuPressed = true;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnMenuClicked(e);
}
else if (menu == 0L && menuPressed)
{
menuPressed = false;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnMenuUnclicked(e);
}
 
//触摸板触摸事件赋值
pad = controllerState.ulButtonTouched & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
if (pad > 0L && !padTouched)
{
padTouched = true;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnPadTouched(e);
 
}
else if (pad == 0L && padTouched)
{
padTouched = false;
ClickedEventArgs e;
e.controllerIndex = controllerIndex;
e.flags = (uint)controllerState.ulButtonPressed;
e.padX = controllerState.rAxis0.x;
e.padY = controllerState.rAxis0.y;
OnPadUntouched(e);
}
}
}
}
