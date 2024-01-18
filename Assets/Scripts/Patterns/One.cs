using System.ComponentModel;
using UnityEngine;

public class One : BasePattern
{
    private readonly Vector2 rippleCountMinMax = new Vector2(1f, 3f);
    private readonly Vector2 rippleSpeedMinMax = new Vector2(1.5f, 4f);
    private readonly Vector2 rippleStrengthMinMax = new Vector2(0.1f, 1f);
    private readonly Vector2 rippleCenterMinMax = new Vector2(0, 1);
    
    private static readonly int RippleCenter = Shader.PropertyToID("_RippleCenter");
    private static readonly int RippleCount = Shader.PropertyToID("_RippleCount");
    private static readonly int RippleSpeed = Shader.PropertyToID("_RippleSpeed");
    private static readonly int RippleStrength = Shader.PropertyToID("_RippleStrength");

    public override void OnStart(Instance[] instances, Material[] materials)
    {
        base.OnStart(instances, materials);
        foreach (var instance in instances)
        {
            instance.spriteRenderer.material = materials[0];
        }
    }

    public override bool GetShouldHandleInstanceBounds()
    {
        return false;
    }

    public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed)
    {
        // waveState = Mathf.PingPong(Time.time * movementSpeed.magnitude, 1);
        // // waveState = (Mathf.Sin(waveState + Time.deltaTime * 10) + 1) /2;
        // foreach (var instance in instances)
        // {
        //     instance.spriteRenderer.material.SetFloat(WaveState, waveState);
        // }
    }

    public override Sizes GetSizes()
    {
        var sizes = new Sizes(ResizeListener.screenSizeInWorldCoords.magnitude * 1.2f, 0);
        return sizes;
    }

    public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid)
    {
        foreach (var instance in instances)
        {
            instance.spriteRenderer.material.SetVector(RippleCenter, new Vector2(Random.Range(rippleCenterMinMax.x, rippleCenterMinMax.y), Random.Range(rippleCenterMinMax.x, rippleCenterMinMax.y)));
            instance.spriteRenderer.material.SetFloat(RippleCount, Random.Range(rippleCountMinMax.x, rippleCenterMinMax.y));
            instance.spriteRenderer.material.SetFloat(RippleSpeed, Random.Range(rippleSpeedMinMax.x, rippleCenterMinMax.y));
            instance.spriteRenderer.material.SetFloat(RippleStrength, Random.Range(rippleStrengthMinMax.x, rippleCenterMinMax.y));
        }
    }
    public override void OnBeforeChange(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed, Material defaultMaterial)
    {
        base.OnBeforeChange(instances, grid, colRow, movementSpeed,defaultMaterial);
        foreach (var instance in instances)
        {
            instance.spriteRenderer.material = defaultMaterial;
        }
    }

    public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize)
    {
        return new Vector2Int(1, 1);;
    }

    
    public override bool IsReadyForPatternChange(Instance[] instances, Grid grid, Vector2Int colRow,
        Vector2 movementSpeed)
    {
        return true;
    }

    public override void Update(Transform t, Instance instance, int curCol, int curRow, int index,
        float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed)
    {
        t.position = Vector3.zero;
    }
}