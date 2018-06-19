using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetraBot : MonoBehaviour
{
   
    private Mesh _mesh;

    private List<Transform> _vertexs=new List<Transform>();


    Dictionary<TetraBot,List<TetraEdge>> _sharedBotEdgeDic=new Dictionary<TetraBot, List<TetraEdge>>(4);

    public TetraEdge GetSharedEdge(TetraBot nb, int edgeIndex)
    {
        int i = Mathf.Clamp(edgeIndex,0, _sharedBotEdgeDic[nb].Count - 1);

        return _sharedBotEdgeDic[nb][i];
    }

   public  List<Transform>MovablePoints=new List<Transform>();


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

        var axis =transform.InverseTransformDirection( e.GetDirection());

        var angle = GetAngle(fromPoint, toPoint, e);

        var center = e.GetCenter();

        var from = fromPoint.position - center;
        var to = toPoint.position - center;

       // var q = transform.rotation.eulerAngles + Quaternion.AngleAxis(angle, axis).eulerAngles;
   
       //transform.SetPositionAndRotation();


        transform.Rotate(axis,angle,Space.Self);
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

    bool CloseEnough(Vector3 v0, Vector3 v1)
    {
        if (Vector3.Distance(v0, v1) < 0.0001f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EvaluateNeighbor(TetraBot nb)
    {
        foreach (var e in Edges)
        {
            foreach (var en in nb.GetEdges())
            {

                var es = e._start.position;
                var ee = e._end.position;

                var ns = en._start.position;
                var ne = en._end.position;


                if (CloseEnough(es,ns)&&CloseEnough(ee,ne)||CloseEnough(es,ne)&&CloseEnough(ee,ns))
                {
                    AddSharedEdge(nb,e);
                    nb.AddSharedEdge(this,en);

                }
                if (!CloseEnough(es, ns) && !CloseEnough(ee, ne) && !CloseEnough(es, ne) && !CloseEnough(ee, ns))
                {

                    //if (_sharedBotEdgeDic.ContainsKey(nb))
                    //{
                    //    if (_sharedBotEdgeDic[nb].Contains(e))
                    //    {
                    //        _sharedBotEdgeDic[nb].Remove(e);
                    //        print("remove");
                    //    }
                    //}
                    //if (nb._sharedBotEdgeDic.ContainsKey(this))
                    //{
                    //    if (nb._sharedBotEdgeDic[this].Contains(en))
                    //    {
                    //        nb._sharedBotEdgeDic[this].Remove(en);
                    //        print("remove nb");
                    //    }
                    //}

                    if (MovablePoints.Contains(e._start) || MovablePoints.Contains(e._end))
                    {
                        continue;
                    }
                    else
                    {
                        MovablePoints.AddRange(e.GetPoints());
                    }

                    nb.MovablePoints.AddRange(en.GetPoints());
                }

            }

            if (_sharedBotEdgeDic.ContainsKey(nb))
            {
                if (_sharedBotEdgeDic[nb].Count == 0)
                {
                    _sharedBotEdgeDic.Remove(nb);
                }
            }

            if (nb._sharedBotEdgeDic.ContainsKey(this))
            {
                if (nb._sharedBotEdgeDic[this].Count == 0)
                {
                    _sharedBotEdgeDic.Remove(this);
                }
            }
        }
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




}
