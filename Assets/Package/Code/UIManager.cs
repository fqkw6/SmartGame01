using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AssetBundles;
/// <summary>
/// UI管理类
/// 面板打开、关闭
/// 后面功能陆续添加
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{

    /// <summary>
    /// HUD面板层
    /// </summary>
    private UICanvas m_HUDCanvas;

    /// <summary>
    /// Normal面板层
    /// </summary>
    private UICanvas m_PanelCanvas;

    /// <summary>
    /// Notice面板层
    /// </summary>
    private UICanvas m_NoticeCanvas;

    /// <summary>
    /// UI关闭回收到的节点
    /// </summary>
    private Transform m_UICacheRoot;

    /// <summary>
    /// UI摄像机
    /// </summary>
    private Camera m_UICamera;

    /// <summary>
    /// 面板列表
    /// </summary>
    private UIPanelBase[] m_Panels = new UIPanelBase[(int)PanelName.Total];

    /// <summary>
    /// 面板对应的Prefab
    /// </summary>
    /// <typeparam name="string">资源地址</typeparam>
    /// <typeparam name="GameObject">Prefab实例</typeparam>
    private Dictionary<string, GameObject> m_Prefabs = new Dictionary<string, GameObject>();

    /// <summary>
    /// HUD面板栈
    /// </summary>
    private Stack<UIPanelBase> m_HudPanels = new Stack<UIPanelBase>();

    /// <summary>
    /// Normal面板栈
    /// </summary>
    private Stack<UIPanelBase> m_NormalPanels = new Stack<UIPanelBase>();

    /// <summary>
    /// Notice面板栈
    /// </summary>
    private Stack<UIPanelBase> m_NoticePanels = new Stack<UIPanelBase>();

    /// <summary>
    /// UI管理器初始化
    /// 创建 UICamera 摄像机
    /// 创建 HUDCanvas 节点
    /// 创建 PanelCanvas 节点
    /// 创建 NoticeCanvas 节点
    /// 创建 UICacheRoot 节点
    /// </summary>
    public void Initialize()
    {
        gameObject.layer = LayerMask.NameToLayer("UI");

        CreateUICamera();

        m_HUDCanvas = CreateCanvas("HUDCanvas", 0);
        m_PanelCanvas = CreateCanvas("PanelCanvas", 1);
        m_NoticeCanvas = CreateCanvas("NoticeCanvas", 2);

        GameObject uiCacheRoot = new GameObject("UICacheRoot");
        m_UICacheRoot = uiCacheRoot.transform;
        m_UICacheRoot.SetParent(transform);

        // temp 后面要用新的输入系统，这里会进行调整
        gameObject.AddComponent<EventSystem>();
        gameObject.AddComponent<StandaloneInputModule>();
        // end temp
    }

    /// <summary>
    /// 创建 UICamera 摄像机
    /// </summary>
    private void CreateUICamera()
    {
        GameObject camera = new GameObject("UICamera");
        m_UICamera = camera.AddComponent<Camera>();
        camera.transform.SetParent(transform);

        camera.layer = LayerMask.NameToLayer("UI");
        m_UICamera.clearFlags = CameraClearFlags.Depth;
        m_UICamera.orthographic = false;
        m_UICamera.cullingMask = 1 << camera.layer;
    }

    /// <summary>
    /// 创建Canvas
    /// </summary>
    /// <param name="name">Canvas名称</param>
    /// <param name="order">Canvas排序层级</param>
    /// <returns>Canvas</returns>
    private UICanvas CreateCanvas(string name, int order)
    {
        GameObject go = new GameObject(name);
        UICanvas canvas = go.AddComponent<UICanvas>();
        go.transform.SetParent(transform);

        canvas.SetCanvas(RenderMode.ScreenSpaceCamera, m_UICamera, order);
        canvas.SetCanvasScaler(CanvasScaler.ScaleMode.ScaleWithScreenSize, CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, 0);
        canvas.SetLayer("UI");

        return canvas;
    }


    /// <summary>
    /// 打开面板
    /// </summary>
    /// <param name="panelName">面板名称（枚举）</param>
    /// <param name="msg">传递消息</param>
    public void OpenPanel(PanelName panelName, object msg = null)
    {
        int panelIndex = (int)panelName;
        UIPanelBase panel = m_Panels[panelIndex];
        if (panel == null)
        {
            Type type = Type.GetType(panelName.ToString());
            panel = m_Panels[panelIndex] = Activator.CreateInstance(type) as UIPanelBase;
        }

        UICanvas canvas = GetCanvasFromPanelType(panel.GetPanelType());
        Stack<UIPanelBase> stack = GetStackFromPanelType(panel.GetPanelType());

        if (stack.Count > 0)
        {
            UIPanelBase stackTopPanel = stack.Peek();
            if (stackTopPanel.Name.Equals(panel.Name))
            {
                Debug.LogErrorFormat("{0} already opened !", stackTopPanel.Name);
                return;
            }
        }

        if (panel.GetGameObject() == null)
        {
            GameObject uiPrefab = null;
            if (m_Prefabs.TryGetValue(panel.GetAssetAddress(), out uiPrefab))
            {
                panel.SetGameObjectAndTransform(uiPrefab);
                panel.GetTransform().SetParent(canvas.transform, false);
                panel.Initialize();
                panel.OnShow(msg);
                panel.OnRefresh(msg);

                stack.Push(panel);
            }
            else
            {
                //AssetBundleManager.Instance.LoadAssetAsync
                //AssetManager.Instantiate(panel.GetAssetAddress(), canvas.transform, false, (IAsyncOperation<GameObject> prefab) =>
                //{
                //    m_Prefabs[panel.GetAssetAddress()] = prefab.Result;

                //    panel.SetGameObjectAndTransform(prefab.Result);
                //    panel.Initialize();
                //    panel.OnShow(msg);
                //    panel.OnRefresh(msg);

                //    stack.Push(panel);
                //});
            }
        }
        else
        {
            panel.GetTransform().SetParent(canvas.transform);
            panel.OnShow(msg);
            panel.OnRefresh(msg);

            stack.Push(panel);
        }
    }

    /// <summary>
    /// 通过面板类型查找对应的Canvas层根结点
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <returns>对应Canvas</returns>
    private UICanvas GetCanvasFromPanelType(UIPanelBase.PanelType panelType)
    {
        switch (panelType)
        {
            case UIPanelBase.PanelType.Hud:
                return m_HUDCanvas;
            case UIPanelBase.PanelType.Normal:
                return m_PanelCanvas;
            case UIPanelBase.PanelType.Notice:
                return m_NoticeCanvas;
            default:
                Debug.LogErrorFormat("GetCanvasFromPanelType => {0}!", panelType);
                return null;
        }
    }

    /// <summary>
    /// 通过面板类型查找对应的Stack
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <returns>对应Stack</returns>
    private Stack<UIPanelBase> GetStackFromPanelType(UIPanelBase.PanelType panelType)
    {
        switch (panelType)
        {
            case UIPanelBase.PanelType.Hud:
                return m_HudPanels;
            case UIPanelBase.PanelType.Normal:
                return m_NormalPanels;
            case UIPanelBase.PanelType.Notice:
                return m_NoticePanels;
            default:
                Debug.LogErrorFormat("GetStackFromPanelType => {0}!", panelType);
                return null;
        }
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <param name="msg">传递消息</param>
    public void ClosePanel(UIPanelBase.PanelType panelType, object msg = null)
    {
        Stack<UIPanelBase> stack = GetStackFromPanelType(panelType);
        UIPanelBase panel = stack.Pop();
        panel.OnHide(msg);
        panel.GetTransform().SetParent(m_UICacheRoot);
    }

    public void ShowHUD()
    {
        m_HUDCanvas.Show();
    }

    public void HideHUD()
    {
        m_HUDCanvas.Hide();
    }

    /// <summary>
    /// 关闭所有HUD面板
    /// </summary>
    public void CloseAllHUDPanel()
    {
        while (m_HudPanels.Count > 0)
        {
            ClosePanel(UIPanelBase.PanelType.Hud);
        }
    }

    /// <summary>
    /// 关闭所有Normal面板
    /// </summary>
    public void CloseAllNormalPanel()
    {
        while (m_NormalPanels.Count > 0)
        {
            ClosePanel(UIPanelBase.PanelType.Normal);
        }
    }

    /// <summary>
    /// 关闭所有Notice面板
    /// </summary>
    public void CloseAllNoticePanel()
    {
        while (m_NoticePanels.Count > 0)
        {
            ClosePanel(UIPanelBase.PanelType.Notice);
        }
    }

    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAllPanel()
    {
        CloseAllNoticePanel();
        CloseAllNormalPanel();
        CloseAllHUDPanel();
    }

}