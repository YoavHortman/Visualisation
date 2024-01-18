using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ForceFromPoint : BasePattern
{
    private float _step = 0;
    private Vector2 center;

    public override bool GetShouldHandleInstanceBounds()
    {
        return false;
    }

    public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed)
    {
        _step += (Time.deltaTime * movementSpeed.magnitude) * 2;
    }

    public override Sizes GetSizes()
    {
        return new Sizes(Random.Range(0.08f, 0.2f), Random.Range(0f, 0f));
    }

    public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid)
    {
        // pick a random cell in the grid
        var randomCell = new Vector3Int(Random.Range(0, colRow.x), Random.Range(0, colRow.y), 0);
        center = grid.GetCellCenterWorld(randomCell);
    }

    public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize)
    {
        var nextColCount = Mathf.CeilToInt(screenSizeInWorldCoords.x * 2 / fullSize);
        var nextRowCount = Mathf.CeilToInt(screenSizeInWorldCoords.y * 2 / fullSize);
        return new Vector2Int(nextColCount, nextRowCount);
    }

    public override void OnBeforeChange(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed, Material defaultMaterial)
    {
        base.OnBeforeChange(instances, grid, colRow, movementSpeed,defaultMaterial);
        _step = 0;
    }

    public override bool IsReadyForPatternChange(Instance[] instances, Grid grid, Vector2Int colRow,
        Vector2 movementSpeed)
    {
        return true;
    }

    Vector3 newPosition = Vector3.zero;
    public override void Update(Transform t, Instance instance, int curCol, int curRow, int index,
        float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed)
    {
        Vector2 cellCenter = grid.GetCellCenterWorld(new Vector3Int(curCol, curRow, 0));
        float distance = Vector2.Distance(cellCenter, center) + 0.001f;

        Vector2 direction = (center - cellCenter).normalized;

        float oscillationX;
        float oscillationY;
        if (MainMenuBackground.seed >= 5)
        {
            oscillationX = Mathf.Sin(_step + distance) * (fullSize / 2);
            oscillationY = Mathf.Cos(_step + distance) * (fullSize / 2);
        }
        else
        {
            oscillationX = Mathf.Sin(_step * distance) * (fullSize / 2);
            oscillationY = Mathf.Cos(_step * distance) * (fullSize / 2);
        }
        
        if (MainMenuBackground.seed % 2 == 0)
        {
            newPosition = cellCenter + direction * new Vector2(oscillationX, oscillationY);
        }
        else
        {
            newPosition = cellCenter + direction * new Vector2(oscillationX, oscillationX);    
        }
        
        t.position = newPosition;
    }
}