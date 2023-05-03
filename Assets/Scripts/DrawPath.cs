using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
    public Color LineColor;

    private List<Transform> CheckPoints = new List<Transform>();

    private void OnDrawGizmos()
    {
        Gizmos.color = LineColor;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        CheckPoints = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                CheckPoints.Add(pathTransforms[i]);
            }
        }

        for (int i = 0; i < CheckPoints.Count; i++)
        {
            Vector3 currentNode = CheckPoints[i].position;
            Vector3 previousNode = Vector3.zero;

            if (i > 0)
            {
                previousNode = CheckPoints[i - 1].position;
            }
            else if (i == 0 && CheckPoints.Count > 1)
            {
                previousNode = CheckPoints[CheckPoints.Count - 1].position;
            }

            Gizmos.DrawLine(previousNode, currentNode);
        }
    }
}
