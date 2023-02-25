using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 处理与网格交互逻辑处理
/// </summary>
[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// 库存控制器
    /// </summary>
    private InventoryController inventoryController;
    /// <summary>
    /// 当前物品网格
    /// </summary>
    private ItemGrid itemGrid;

    private void Awake()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        itemGrid = GetComponent<ItemGrid>();
    }

    /// <summary>
    /// 注册鼠标进入UI事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = itemGrid;
    }

    /// <summary>
    /// 注册鼠标离开UI事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = null;
    }
}
