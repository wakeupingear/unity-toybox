using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 100)]
    int resolution = 10;
    private int lastResolution = 0;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    private List<Transform> points = new List<Transform>();

    private float duration;

    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }

        if (lastResolution != resolution)
            CreatePoints();

        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }

    private void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }

    private void UpdateFunction()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float time = Time.time;
        float step = 2f / resolution;
        for (int i = 0, x = 0, z = 0; i < points.Count; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
            }
            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, time);
        }
    }

    void UpdateFunctionTransition()
    {
        FunctionLibrary.Function
            from = FunctionLibrary.GetFunction(transitionFunction),
            to = FunctionLibrary.GetFunction(function);
        float progress = duration / transitionDuration;
        float time = Time.time;
        float step = 2f / resolution;
        for (int i = 0, x = 0, z = 0; i < points.Count; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
            }
            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;
            points[i].localPosition = FunctionLibrary.Morph(
                u, v, time, from, to, progress
            );
        }
    }

    private void CreatePoints()
    {
        int numPoints = resolution * resolution;
        while (points.Count < numPoints)
        {
            var point = Instantiate(pointPrefab);
            point.SetParent(transform, false);
            points.Add(point);
        }
        while (points.Count > numPoints)
        {
            var point = points[points.Count - 1];
            points.RemoveAt(points.Count - 1);
            Destroy(point.gameObject);
        }

        float scale = 2f / resolution;
        foreach (var point in points)
        {
            point.localScale = Vector3.one * scale;
        }

        lastResolution = resolution;
    }
}
