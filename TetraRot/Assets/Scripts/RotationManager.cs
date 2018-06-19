using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    private List<TetraBot> Bots=new List<TetraBot>();
    [SerializeField] private Transform _vertPrefab;
    [SerializeField] private TetraBot _tbpfb;
    [SerializeField] private int CountX = 5;
    [SerializeField] private int CountZ = 5;
    [SerializeField] private float Scale = 1f;

    List<Vector3> SavedPositions=new List<Vector3>();

	// Use this for initialization
	void Start ()
	{
		
        SetPosition();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Space))
	    {
            Rotate();
	    }
	}

    void Rotate()
    {
        Bots[0].EvaluateNeighbor(Bots[1]);

        Bots[0].Rotate(Bots[1],0,Bots[0].MovablePoints[0],Bots[1].MovablePoints[1]);
    }
   


   void  SetPosition()
   {
    

      
        for (int z = 0; z < CountZ; z++)
        {
            for (int x = 0; x < CountX; x++)
            {
                var tb = Instantiate(_tbpfb, transform);

                Bots.Add(tb);

                Scale = tb.GetEdgeLength();

                float a = 0.5f*Scale * (Mathf.Sqrt(3));
                Vector3 p;

                if (z % 2 == 0)
                {
                    if (x % 2 == 0)
                    {
                        p = (new Vector3(0.5f * Scale * x, 0, z * a));

                        SavedPositions.Add(p);
                        tb.transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                    else
                    {
                        p = (new Vector3(0.5f * Scale * x, 0, z * a + a / 3));

                        SavedPositions.Add(p);
                    }
                }
                else
                {
                    if (x % 2 != 0)
                    {
                        p = (new Vector3(0.5f * Scale * x, 0, z * a));

                        SavedPositions.Add(p);
                        tb.transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                    else
                    {
                        p = (new Vector3(0.5f * Scale * x, 0, z * a + a / 3));

                        SavedPositions.Add(p);
                      
                    }
                }
              
                if (z % 2 == 0||x%2==0)
                {
                   
                }

                tb.initialize(p,_vertPrefab);
            }
        }

      
    }

   

}
