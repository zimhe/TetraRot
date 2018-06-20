using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetraEdge : MonoBehaviour
{
    public readonly Transform _start;
    public readonly Transform _end;

    private Transform _center;


    public TetraEdge(Transform start,Transform end)
    {
        _start = start;
        _end = end;

        _center = Instantiate(start);
        _center.parent = start.parent;

        _center.position = 0.5f * (start.position + end.position);
    }

    public List<Vector3> VertPositions()
    {
        return new List<Vector3>(){_start.position,_end.position};
    }

    public List<Transform> GetPoints()
    {
        return new List<Transform>(){_start,_end};
    }

    public Vector3 GetCenter()
    {
        return 0.5f * (_start.position + _end.position);
    }

    public Transform GetCenterXform()
    {
        //_center.parent=
        return _center;
    }

    public Vector3 GetDirection()
    {
        return _end.position - _start.position;
    }

   
}
