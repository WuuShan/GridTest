using UnityEngine;

/// <summary>
/// 处理物品网格逻辑相关
/// </summary>
public class ItemGrid : MonoBehaviour
{
    /// <summary>
    /// 瓦片的宽度
    /// </summary>
    public const float tileSizeWidth = 32;

    /// <summary>
    /// 瓦片的高度
    /// </summary>
    public const float tileSizeHeight = 32;

    /// <summary>
    /// 库存物品格子数组
    /// </summary>
    private InventoryItem[,] inventoryItemSlot;

    /// <summary>
    /// 矩形变换组件
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// 网格宽度
    /// </summary>
    [SerializeField] private int gridSizeWidth = 20;

    /// <summary>
    /// 网格高度
    /// </summary>
    [SerializeField] private int gridSizeHeight = 10;

    /// <summary>
    /// 鼠标在网格上的位置
    /// </summary>
    private Vector2 positionOnTheGrid = new Vector2();

    /// <summary>
    /// 瓦片网格坐标
    /// </summary>
    private Vector2Int tileGridPosition = new Vector2Int();

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    /// <summary>
    /// 初始化库存网格
    /// </summary>
    /// <param name="width">网格宽度</param>
    /// <param name="height">网格高度</param>
    private void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    /// <summary>
    /// 获得瓦片在网格上的位置
    /// </summary>
    /// <param name="mousePosition">鼠标位置</param>
    /// <returns>瓦片坐标</returns>
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        // 计算鼠标位置相对于网格左上角的偏移量
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        // 计算瓦片在网格中的位置
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        // 返回瓦片坐标
        return tileGridPosition;
    }

    /// <summary>
    /// 判断所选物品可否放置到指定的位置
    /// </summary>
    /// <param name="item">要放置的物品</param>
    /// <param name="posX">位置X轴</param>
    /// <param name="posY">位置Y轴</param>
    /// <param name="overlapItem">重叠物品</param>
    /// <returns></returns>
    public bool PlaceItem(InventoryItem item, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (BoundryCheck(posX, posY, item.WIDTH, item.HEIGHT) == false) return false;

        if (OverlapCheck(posX, posY, item.WIDTH, item.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            ClearGridReference(overlapItem);
        }

        PlaceItem(item, posX, posY);

        return true;
    }

    /// <summary>
    /// 放置物品到指定位置上
    /// </summary>
    /// <param name="item">要放置的物品</param>
    /// <param name="posX">位置X轴</param>
    /// <param name="posY">位置Y轴</param>
    public void PlaceItem(InventoryItem item, int posX, int posY)
    {
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        // 将该物品添加到库存物品数组中
        for (int x = 0; x < item.WIDTH; x++)
        {
            for (int y = 0; y < item.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = item;
            }
        }

        item.onGridPositionX = posX;
        item.onGridPositionY = posY;

        Vector2 position = CalculatePositionOnGrid(item, posX, posY);

        rectTransform.localPosition = position;
    }

    /// <summary>
    /// 计算物品在网格上的位置
    /// </summary>
    /// <param name="item">参考的物品</param>
    /// <param name="posX">位置X轴</param>
    /// <param name="posY">位置Y轴</param>
    /// <returns>物品在网格上的位置</returns>
    public Vector2 CalculatePositionOnGrid(InventoryItem item, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * item.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * item.HEIGHT / 2);
        return position;
    }

    /// <summary>
    /// 检查是否物品重叠
    /// </summary>
    /// <param name="posX">位置X轴</param>
    /// <param name="posY">位置Y轴</param>
    /// <param name="width">物品宽度</param>
    /// <param name="height">物品高度</param>
    /// <param name="overlapItem">要检查的重叠物品</param>
    /// <returns></returns>
    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 检查物品能否有空间放下
    /// </summary>
    /// <param name="posX">位置X轴</param>
    /// <param name="posY">位置Y轴</param>
    /// <param name="width">物品宽度</param>
    /// <param name="height">物品高度</param>
    /// <returns></returns>
    private bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 拾起指定网格坐标中的物品
    /// </summary>
    /// <param name="x">网格宽度</param>
    /// <param name="y">网格高度</param>
    /// <returns>拾起的物品</returns>
    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null) return null;

        ClearGridReference(toReturn);

        return toReturn;
    }

    /// <summary>
    /// 将该物品从库存物品数组中移除
    /// </summary>
    /// <param name="item">要移除的物品</param>
    private void ClearGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    /// <summary>
    /// 检查坐标是否在网格内
    /// </summary>
    /// <param name="posX">X坐标</param>
    /// <param name="posY">Y坐标</param>
    /// <returns></returns>
    private bool PositionCheck(int posX, int posY)
    {
        if (posX < 0 || posY < 0) return false;
        if (posX >= gridSizeWidth || posY >= gridSizeHeight) return false;
        return true;
    }

    /// <summary>
    /// 检查物品是否在网格内
    /// </summary>
    /// <param name="posX">X坐标</param>
    /// <param name="posY">Y坐标</param>
    /// <param name="width">物品宽度</param>
    /// <param name="height">物品高度</param>
    /// <returns></returns>
    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if (PositionCheck(posX, posY) == false) return false;

        posX += width - 1;
        posY += height - 1;

        if (PositionCheck(posX, posY) == false) return false;

        return true;
    }

    /// <summary>
    /// 获得指定行列的物品
    /// </summary>
    /// <param name="x">行</param>
    /// <param name="y">列</param>
    /// <returns>指定物品</returns>
    public InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    /// <summary>
    /// 获得可插入物品的空间位置
    /// </summary>
    /// <param name="itemToInsert">要插入的物品</param>
    /// <returns>可插入物品的空间位置</returns>
    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }
}