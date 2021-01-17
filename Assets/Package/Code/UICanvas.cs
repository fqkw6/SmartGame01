using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas基类
/// </summary>
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(CanvasGroup))]
public class UICanvas : MonoBehaviour
{

    /// <summary>
    /// 默认Canvas宽
    /// </summary>
    private const int RESOLUTION_X = 1920;

    /// <summary>
    /// 默认Canvas高
    /// </summary>
    private const int RESOLUTION_Y = 1080;

    /// <summary>
    /// Canvas组件
    /// </summary>
    private Canvas m_Canvas;

    /// <summary>
    /// CanvasScaler组件
    /// </summary>
    private CanvasScaler m_CanvasScaler;

    /// <summary>
    /// GraphicRaycaster组件
    /// </summary>
    private GraphicRaycaster m_GraphicRaycaster;

    /// <summary>
    /// CanvasGroup组件
    /// </summary>
    private CanvasGroup m_CanvasGroup;

    void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
        m_CanvasScaler = GetComponent<CanvasScaler>();
        m_GraphicRaycaster = GetComponent<GraphicRaycaster>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 设置Canvas属性
    /// </summary>
    /// <param name="renderMode">渲染模式</param>
    /// <param name="camera">摄像机</param>
    /// <param name="order">排序值</param>
    public void SetCanvas(RenderMode renderMode, Camera camera, int order)
    {
        m_Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        m_Canvas.worldCamera = camera;
        m_Canvas.sortingOrder = order;
    }

    /// <summary>
    /// 设置CanvasScaler属性
    /// </summary>
    /// <param name="scaleMode">缩放模式</param>
    /// <param name="screenMatchMode">屏幕匹配模式</param>
    /// <param name="matchWidthOrHeight">宽高权重</param>
    public void SetCanvasScaler(CanvasScaler.ScaleMode scaleMode, CanvasScaler.ScreenMatchMode screenMatchMode, float matchWidthOrHeight)
    {
        m_CanvasScaler.uiScaleMode = scaleMode;
        m_CanvasScaler.screenMatchMode = screenMatchMode;
        m_CanvasScaler.matchWidthOrHeight = matchWidthOrHeight;

        m_CanvasScaler.referenceResolution = new Vector2(RESOLUTION_X, RESOLUTION_Y);
    }

    /// <summary>
    /// 设置Canvas层级
    /// </summary>
    /// <param name="layerName">层级名称</param>
    public void SetLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    /// <summary>
    /// 显示Canvas及其所有下属面板
    /// </summary>
    public void Show()
    {
        m_CanvasGroup.interactable = true;
        m_CanvasGroup.alpha = 1;
    }

    /// <summary>
    /// 隐藏Canvas及其所有下属面板
    /// </summary>
    public void Hide()
    {
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.alpha = 0;
    }

}