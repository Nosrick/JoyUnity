using System;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class ResizeGridLayoutCells : MonoBehaviour
    {
        [SerializeField] protected GameObject ChildPrefab;
        
        protected int Columns { get; set; }
        protected int Rows { get; set; }

        public void OnEnable()
        {
            GridLayoutGroup grid = this.GetComponent<GridLayoutGroup>();
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            RectTransform childRect = this.ChildPrefab.GetComponent<RectTransform>();
            float thisAspect = rectTransform.rect.width / rectTransform.rect.height;
            float childAspect = (grid.spacing.x + grid.cellSize.x) / (grid.spacing.y + grid.cellSize.y);
            float newSize = 0;

            float columnWidth = 0;
            float rowHeight = 0;
            if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                this.Columns = grid.constraintCount;
                columnWidth = (rectTransform.rect.width - (grid.spacing.x * this.Columns)) / this.Columns;
                this.Rows = (this.transform.childCount / this.Columns); 
                rowHeight = (rectTransform.rect.height - (grid.spacing.y * this.Rows)) / this.Rows;
                newSize = Math.Min(rowHeight, columnWidth);
                grid.cellSize = new Vector2(newSize, newSize);
            }
            else if (grid.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                this.Rows = grid.constraintCount;
                rowHeight = (rectTransform.rect.height - (grid.spacing.y * this.Rows)) / this.Rows;
                this.Columns = (this.transform.childCount / this.Rows);
                columnWidth = (rectTransform.rect.width - (grid.spacing.x * this.Columns)) / this.Columns;
                newSize = Math.Min(rowHeight, columnWidth);
                grid.cellSize = new Vector2(newSize, newSize);
            }
        }
    }
}