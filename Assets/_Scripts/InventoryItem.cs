using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 库存物品逻辑
/// </summary>
public class InventoryItem : MonoBehaviour
{
    /// <summary>
    /// 物品数据
    /// </summary>
    public ItemData data;

    /// <summary>
    /// 物品高度
    /// </summary>
    public int HEIGHT
    {
        get
        {
            if (!rotated)
            {
                return data.height;
            }
            return data.width;
        }
    }

    /// <summary>
    /// 物品宽度
    /// </summary>
    public int WIDTH
    {
        get
        {
            if (!rotated)
            {
                return data.width;
            }
            return data.height;
        }
    }

    /// <summary>
    /// 物品在网格的X轴位置
    /// </summary>
    public int onGridPositionX;

    /// <summary>
    /// 物品在网格的Y轴位置
    /// </summary>
    public int onGridPositionY;

    /// <summary>
    /// 物品是否旋转过
    /// </summary>
    public bool rotated = false;

    /// <summary>
    /// 设置物品数据
    /// </summary>
    /// <param name="itemData">物品数据</param>
    public void Set(ItemData itemData)
    {
        data = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width * ItemGrid.tileSizeWidth;
        size.y = itemData.height * ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    /// <summary>
    /// 旋转物品
    /// </summary>
    public void Rotate()
    {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, rotated ? 90f : 0f);
    }
}