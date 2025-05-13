using System;
using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform selectionAreaRectTransform;
    [SerializeField] private Canvas canvas;
    
    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += OnSelectionAreaStart;     
        UnitSelectionManager.Instance.OnSelectionAreaEnd += OnSelectionAreaEnd;
        
        selectionAreaRectTransform.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (selectionAreaRectTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }
    
    private void OnSelectionAreaStart(object sender, EventArgs e)
    {
        selectionAreaRectTransform.gameObject.SetActive(true);
        UpdateVisual();
    }
    
    private void OnSelectionAreaEnd(object sender, EventArgs e)
    {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();
        float canvasScaleFactor = canvas.transform.localScale.x;
        
        selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScaleFactor;
        selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScaleFactor;
    }

    private void OnDestroy()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart -= OnSelectionAreaStart;     
        UnitSelectionManager.Instance.OnSelectionAreaEnd -= OnSelectionAreaEnd;
    }
}
