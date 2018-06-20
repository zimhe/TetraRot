using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetraBot : MonoBehaviour
{
   
    private Mesh _mesh;

    private List<Transform> _vertexs=new List<Transform>();


    public readonly Dictionary<TetraBot,List<TetraEdge>> _sharedBotEdgeDic=new Dictionary<TetraBot, List<TetraEdge>>(4);
    public readonly Dictionary<TetraBot,List<Transform>> _movablePoints=new Dictionary<TetraBot, List<Transform>>();

    public TetraEdge GetSharedEdge(TetraBot nb, int edgeIndex)
    {
        int i = Mathf.Clamp(edgeIndex,0, _sharedBotEdgeDic[nb].Count - 1);

        return _sharedBotEdgeDic[nb][i];
    }

   


        List<TetraEdge> Edges=new List<TetraEdge>();

    public List<TetraEdge> GetEdges()
    {
        return Edges;
    }


    public void initialize(Vector3 p, Transform vertexPfb)
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        transform.localPosition = p;

        foreach (var v in _mesh.vertices)
        {
            var vert = Instantiate(vertexPfb,transform);
            vert.transform.localScale = 0.05f * Vector3.one;
            vert.transform.localPosition = v;
            _vertexs.Add(vert);
        }

        print(_vertexs.Count);

        SetEdge();

        print(Edges.Count);
    }

    void SetEdge()
    {
        foreach (var v0 in _vertexs)
        {
            foreach (var v1 in _vertexs)
            {
                if (v0 != v1&&!EdgeRepeat(v0,v1))
                {
                    Edges.Add(new TetraEdge(v0, v1));
                }
            }
        }
    }

    bool EdgeRepeat(Transform v0, Transform v1)
    {
        bool repeat = false;

        foreach (var e in Edges)
        {
            if (e.GetPoints().Contains(v0) && e.GetPoints().Contains(v1))
            {
                repeat= true;
                break;
            }
        }

        return repeat;
    }

    public List<Transform> GetVerts()
    {
        return _vertexs;
    }

    public Vector3 GetAxis(int v0, int v1)
    {
        return _vertexs[v0].localPosition - _vertexs[v1].localPosition;
    }

    public void Rotate(TetraBot nb,int edge, Transform fromPoint,Transform toPoint)
    {
        var e = nb._sharedBotEdgeDic[this][edge];

        var axis = e.GetDirection();

        var angle = GetAngle(fromPoint, toPoint, e);

        var center = e.GetCenter();

        var pivot = e.GetCenterXform();

        transform.parent = pivot;

        pivot.Rotate(axis,angle);

       // var q = transform.rotation.eulerAngles + Quaternion.AngleAxis(angle, axis).eulerAngles;
   
       //transform.SetPositionAndRotation();

        transform.parent = pivot.parent.parent;

    }

    public Vector3 GetDirection(int face, int vert)
    {
        var fi = _mesh.GetTriangles(face);

        var c = GetCenter(fi[0], fi[1], fi[2]);

        return _mesh.vertices[vert] - c;

    }

    public Vector3 GetCenter(int v0, int v1, int v2)
    {
        var ver = _vertexs;

        return (ver[v0].localPosition + ver[v1].localPosition + ver[v2].localPosition) / 3;
    }

    public float GetEdgeLength()
    {
        var v = GetComponent<MeshFilter>().mesh.vertices;


        return (v[0] - v[1]).magnitude;
    }

    public float GetAngle(Transform from, Transform to, TetraEdge edge)
    {
        var v0 = edge.GetCenter();

        var F = from.position - v0;
        var T = to.position - v0;


        return Vector3.Angle(F, T);
    }

    //public List<int> GetEdge(int index)
    //{
    //    int vc = _vertexs.Count;
    //    return V
    //}

    bool CloseEnoughEdge(Vector3 d0, Vector3 d1)
    {
        bool close;

        if (Vector3.Angle(d0,d1) < 0.0001f)
        {
            close= true;
        }
        else
        {
            close= false;
        }

        return close;
    }

    bool CloseEnoughPoint(Transform v0, Transform v1)
    {
        bool close;
        if (Vector3.Distance(v0.position, v1.position) < 0.0001f)
        {
            close= true;
        }
        else
        {
            close= false;
        }
        return close;
    }

    public void EvaluateNeighbor(TetraBot nb)
    {
        if (_sharedBotEdgeDic.ContainsKey(nb) && nb._sharedBotEdgeDic.ContainsKey(this))
        {
            _sharedBotEdgeDic[nb] = new List<TetraEdge>();
            nb._sharedBotEdgeDic[this] = new List<TetraEdge>();
            _movablePoints[nb] = new List<Transform>();
            nb._movablePoints[this] = new List<Transform>();
        }
    

        foreach (var e in Edges)
        {
            foreach (var en in nb.GetEdges())
            {
                var ec = e.GetCenterXform();
                var nc = en.GetCenterXform();

                if (CloseEnoughPoint(ec,nc))
                {
                    print("share");
                    AddSharedEdge(nb,e);
                    nb.AddSharedEdge(this,en);
                }
            }
        }
        foreach (var v in _vertexs)
        {
            foreach (var vn in nb._vertexs)
            {
                AddMovablePoints(nb, v);
                nb.AddMovablePoints(this, vn);
            }
        }
        foreach (var v in _vertexs)
        {
            foreach (var vn in nb._vertexs)
            {
                if (CloseEnoughPoint(v, vn))
                {
                    _movablePoints[nb].Remove(v);
                    nb._movablePoints[this].Remove(vn);
                }
            }
        }
        print(_movablePoints[nb].Count);
    }

   



    public void AddSharedEdge(TetraBot nb, TetraEdge EdgeDir)
    {
        if (!_sharedBotEdgeDic.ContainsKey(nb))
        {
            _sharedBotEdgeDic.Add(nb, new List<TetraEdge>() { EdgeDir });
        }
        else
        {
            if(!_sharedBotEdgeDic[nb].Contains(EdgeDir))
            _sharedBotEdgeDic[nb].Add(EdgeDir);
        }
      
    }

    public void AddMovablePoints(TetraBot nb, Transform point)
    {
        if (!_movablePoints.ContainsKey(nb))
        {
            _movablePoints.Add(nb,new List<Transform>(){point});
        }
        else
        {
            if (!_movablePoints[nb].Contains(point))
            {
                _movablePoints[nb].Add(point);
            }
        }
    }



}
