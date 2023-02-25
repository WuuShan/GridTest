using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 库存物品的高亮逻辑处理相关
/// </summary>
public class InventoryHighlight : MonoBehaviour
{
    /// <summary>
    /// 高亮的矩形变换
    /// </summary>
    [SerializeField] private RectTransform highlighter;

    /// <summary>
    /// 根据条件显示或隐藏高亮
    /// </summary>
    /// <param name="b">条件</param>
    public void Show(bool b)
    {
        highlighter.gameObject.SetActive(b);
    }

    /// <summary>
    /// 根据目标物品的大小设置高亮的大小
    /// </summary>
    /// <param name="targetItem">目标物品</param>
    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.WIDTH * ItemGrid.tileSizeWidth;
        size.y = targetItem.HEIGHT * ItemGrid.tileSizeHeight;
        highlighter.sizeDelta = size;
    }

    /// <summary>
    /// 根据目标物品的位置设置高亮的位置
    /// </summary>
    /// <param name="targetGrid"></param>
    /// <param name="targetItem"></param>
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        SetParent(targetGrid);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.onGridPositionX, targetItem.onGridPositionY);

        highlighter.localPosition = pos;
    }

    /// <summary>
    /// 设置高亮的父物体为目标网格
    /// </summary>
    /// <param name="targetGrid">目标网格</param>
    public void SetParent(ItemGrid targetGrid)
    {
        if (targetGrid == null) return;
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }

    /// <summary>
    /// 设置高亮的位置为目标网格中的目标物品位置
    /// </summary>
    /// <param name="targetGrid">目标网格</param>
    /// <param name="targetItem">目标物品</param>
    /// <param name="posX">位置X轴</param>
    /// <param name="posY">位置Y轴</param>
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highlighter.localPosition = pos;
    }
}
