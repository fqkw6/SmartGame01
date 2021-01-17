using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 面板基类
/// </summary>
public abstract class UIPanelBase : Mediator
{

    /// <summary>
    /// 面板类型枚举，不同类型面板，会放置到不同的画板上
    /// </summary>
    public enum PanelType
    {
        /// <summary>
        /// 无效面板
        /// </summary>
        None,
        /// <summary>
        /// HUD面板
        /// </summary>
        Hud,
        /// <summary>
        /// 普通面板
        /// </summary>
        Normal,
        /// <summary>
        /// 通知类面板
        /// </summary>
        Notice,
		/// <summary>
		/// 对话框面板
		/// </summary>
		Dialugue
	}

    /// <summary>
    /// <see cref="GetAssetAddress()"/>
    /// </summary>
    protected string m_AssetAddress;

    /// <summary>
    /// <see cref="GetPanelType()"/>
    /// </summary>
    private PanelType m_PanelType = PanelType.None;

    /// <summary>
    /// <see cref="GetGameObject()"/>
    /// </summary>
    private GameObject m_GameObject;

    /// <summary>
    /// <see cref="GetTransform()"/>
    /// </summary>
    private Transform m_Transform;

    /// <summary>
    /// 热键的自动ID
    /// </summary>
    private int m_HotkeyAutoID = 0;
    /// <summary>
    /// 关闭时
    /// </summary>
    public UnityAction OnClosed;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="panelName">面板名称（枚举）</param>
    /// <param name="assetAddress">面板资源地址</param>
    public UIPanelBase(PanelName panelName, string assetAddress, PanelType panelType) : base(panelName)
    {
        m_AssetAddress = assetAddress;
        m_PanelType = panelType;
    }

    /// <summary>
    /// 是否为常驻窗口
    /// </summary>
    public bool IsPermanent;

    /// <summary>
    /// 获取面板资源地址
    /// </summary>
    public string GetAssetAddress()
    {
        return m_AssetAddress;
    }

    /// <summary>
    /// 获取面板类型
    /// </summary>
    public PanelType GetPanelType()
    {
        return m_PanelType;
    }

    /// <summary>
    /// 获取面板GameObject
    /// </summary>
    public GameObject GetGameObject()
    {
        return m_GameObject;
    }

    /// <summary>
    /// 设置面板GameObject，同时设置Transform
    /// </summary>
    /// <param name="go">GameObject</param>
    public void SetGameObjectAndTransform(GameObject go)
    {
        m_GameObject = go;
        m_Transform = m_GameObject ? m_GameObject.transform : null;
    }

    /// <summary>
    /// 获取面板Transform
    /// </summary>
    public Transform GetTransform()
    {
        return m_Transform;
    }

    /// <summary>
    /// 查找面板内组件
    /// </summary>
    /// <param name="path">面板内相对路径</param>
    /// <returns>对应组件</returns>
    protected T FindComponent<T>(string path) where T : Component
    {
        Transform result = m_Transform.Find(path);
        if (result == null)
        {
            return null;
        }
        return result.GetComponent<T>();
    }

    /// <summary>
    /// 查找指定节点下的组件
    /// </summary>
    /// <param name="node">节点</param>
    /// <param name="path">相对节点的相对路径</param>
    /// <returns>对应组件</returns>
    protected T FindComponent<T>(Transform node, string path)
    {
        Transform result = node.Find(path);
        if (result)
        {
            return result.GetComponent<T>();
        }
        return default;
    }

    /// <summary>
    /// 面板初始化，通常初始化该面板相关组件
    /// </summary>
    public virtual void Initialize()
    {

    }

    /// <summary>
    /// 刷新面板内容
    /// </summary>
    /// <param name="msg">消息传递</param>
    public virtual void OnRefresh(object msg)
    {

    }

    /// <summary>
    /// 面板显示时调用，通常注册消息监听
    /// </summary>
    /// <param name="msg">消息传递</param>
    public virtual void OnShow(object msg)
    {
        m_GameObject.SetActive(true);
        Facade.RegisterMediator(this);

       // UIManager.Instance.SetAllCanvasInteractable(true);
    }
    
    /// <summary>
    /// 面板隐藏时调用，通常注销消息监听
    /// </summary>
    /// <param name="msg">消息传递</param>
    public virtual void OnHide(object msg)
    {
        StopUpdate();
      
        Facade.RemoveMediator(Name);
        m_GameObject.SetActive(false);

    }

    /// <summary>
    /// 消息监听列表
    /// </summary>
    public override NotificationName[] ListNotificationInterests()
    {
        return base.ListNotificationInterests();
    }

    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="notification">消息</param>
    public override void HandleNotification(PureMVC.Interfaces.INotification notification)
    {

    }


    #region 协程处理

    /// <summary>
    /// 开始更新
    /// </summary>
    protected void StartUpdate()
    {
        StopUpdate();

      //  UIManager.Instance.OnUpdate += Update;
    }

    /// <summary>
    /// 停止更新
    /// </summary>
    protected void StopUpdate()
    {
      //  UIManager.Instance.OnUpdate -= Update;
    }

    /// <summary>
    /// 更新函数
    /// </summary>
    protected virtual void Update()
    {

    }

    #endregion

 
}