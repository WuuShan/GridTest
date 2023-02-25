using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 库存控制器
/// </summary>
public class InventoryController : MonoBehaviour
{
    /// <summary>
    /// 所选物品网格
    /// </summary>
    private ItemGrid selectedItemGrid;

    /// <summary>
    /// 所选物品网格
    /// </summary>
    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    /// <summary>
    /// 所选物品
    /// </summary>
    private InventoryItem selectedItem;

    /// <summary>
    /// 重叠物品
    /// </summary>
    private InventoryItem overlapItem;

    /// <summary>
    /// 所选物品的矩形变换
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// 物品数据列表
    /// </summary>
    [SerializeField] private List<ItemData> items;

    /// <summary>
    /// 物品预制体
    /// </summary>
    [SerializeField] private GameObject itemPrefab;

    /// <summary>
    /// UI画布变换
    /// </summary>
    [SerializeField] private Transform canvasTransform;

    /// <summary>
    /// 库存物品高亮逻辑相关
    /// </summary>
    private InventoryHighlight inventoryHighlight;

    /// <summary>
    /// 上次高亮的位置
    /// </summary>
    private Vector2Int oldPosition;

    /// <summary>
    /// 要高亮的物品
    /// </summary>
    private InventoryItem itemToHighlight;

    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    private void Update()
    {
        ItemIconDrag();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedItem == null)
            {
                CreateRandomItem();
                oldPosition = Vector2Int.zero;
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            InsertRandomItem();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
            oldPosition = Vector2Int.zero;
        }

        if (selectedItemGrid == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    /// <summary>
    /// 旋转所选物品
    /// </summary>
    private void RotateItem()
    {
        if (selectedItem == null) return;

        selectedItem.Rotate();
        //inventoryHighlight.SetSize(selectedItem);
        //inventoryHighlight.SetPosition(selectedItemGrid, selectedItem);
    }

    /// <summary>
    /// 往所选的网格中随机插入物品
    /// </summary>
    private void InsertRandomItem()
    {
        if (selectedItemGrid == null) return;

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    /// <summary>
    /// 插入随机生成的物品
    /// </summary>
    /// <param name="itemToInsert">要插入的物品</param>
    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) return;

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    /// <summary>
    /// 处理高亮逻辑
    /// </summary>
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        if (oldPosition == positionOnGrid) return;

        oldPosition = positionOnGrid;

        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if (itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }
        else
        {
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(
                positionOnGrid.x, positionOnGrid.y,
                selectedItem.WIDTH, selectedItem.HEIGHT));
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    /// <summary>
    /// 生成一个随机物品
    /// </summary>
    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    /// <summary>
    /// 鼠标左键按下时，库存的逻辑处理
    /// </summary>
    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();

        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    /// <summary>
    /// 获得瓦片在网格上的位置
    /// </summary>
    /// <returns>瓦片位置</returns>
    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    /// <summary>
    /// 将物品放置在指定位置上
    /// </summary>
    /// <param name="tileGridPosition">瓦片在网格上的位置</param>
    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if (complete)
        {
            selectedItem = null;
            if (overlapItem != null)    // 若是重叠物品不为空则将其设为所选物品
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    /// <summary>
    /// 拾起指定位置上的物品
    /// </summary>
    /// <param name="tileGridPosition">瓦片在网格上的位置</param>
    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// 使所选物品的图标跟随鼠标移动
    /// </summary>
    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
            rectTransform.SetParent(canvasTransform);
        }
    }
}