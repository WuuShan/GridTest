using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品数据
/// </summary>
[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    /// <summary>
    /// 物品宽度
    /// </summary>
    public int width = 1;
    /// <summary>
    /// 物品高度
    /// </summary>
    public int height = 1;

    /// <summary>
    /// 物品图标
    /// </summary>
    public Sprite itemIcon;
}
