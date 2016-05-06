using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Networking.Types;


public class NodeManager : MonoBehaviour
{
    public float LineDrawDistance, LineDrawWidth;
    
    public GameObject NodePrefab;
    public bool IsResettingNodes;
    public int TimeoutThreshold;
    public Vector2 TopLeft, BottomRight;

    [Range(1,10)]
    public int NodesPerRow;

    [Range(1, 50)]
    public int Rows;
    [Range(1, 20)] public float NodeDistanceFromEachOther;

    private int _totalNodes;
    private System.Random _random = new System.Random();
    private List<Node> _nodes = new List<Node>();
    private GameObject _feedback;
	// Use this for initialization
	void Start ()
	{
	    InitializeReferences();
        SetNodes();
	}
    
    // Update is called once per frame
    void Update()
    {
        if (IsResettingNodes)
        {
            SetNodes();
            IsResettingNodes = false;
        }
    }

    void InitializeReferences()
    {
        _feedback = GameObject.Find("FeedbackWindow");
        _feedback.SetActive(false);
    }

    void ResetNodes()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

    }


    void SetNodes()
    {
        _totalNodes = NodesPerRow * Rows;

        ResetNodes();

        AddNodes();

        GetDistanceVectors();
        
        CreateSpanningTree();

        InstantiateNodes();       
    }

    void InstantiateNodes()
    {
        for (var i = 0; i < _nodes.Count; ++i)
        {
            var obj = (GameObject)Instantiate(NodePrefab, gameObject.transform.position + _nodes[i].Position, Quaternion.identity);
            obj.transform.parent = gameObject.transform;
            obj.GetComponent<NodeOperation>().SetNode(_nodes[i], LineDrawWidth);
        }
    }
    void AddNodes()
    {
        _nodes.Clear();;
        for (var i = 0; i < _totalNodes; ++i)
        {
            _nodes.Add(new Node()
            {
                GlobalId = i,
                FeedBackWindow = _feedback,
                TotalNodes = _totalNodes,
                Position = AssignPosition(i),
                Row = i / NodesPerRow,
                NodeManager = gameObject,
                NodesInRow = NodesPerRow
            });
        }
    }

    void GetDistanceVectors()
    {
        for (var i = 0; i < _totalNodes; ++i)
        {
            GetDistanceVector(i);
        }
    }

    void GetDistanceVector(int id)
    {
        var list = new List<int>();
        for (var i = 0; i < _totalNodes; i++)
        {
            list.Add((int)Vector3.Distance(_nodes[i].Position, _nodes[id].Position));
        }
        _nodes[id].DistanceTo = list;
    }

    Vector3 AssignPosition(int id)
    {
        var currentRow = id/NodesPerRow;
        var distanceToNextRow = (TopLeft.y - BottomRight.y) / Rows;
        var minY = BottomRight.y + distanceToNextRow * currentRow;
        var maxY = BottomRight.y + distanceToNextRow * (currentRow + 1);
        var nodeTimeout = 0;
        var distanceTimeout = 0;
        var startDistance = NodeDistanceFromEachOther;
        while (true)
        {
            //Debug.LogFormat("{0} min and {1} max for row {2}", minY, maxY, currentRow);
            var testVector = new Vector3(UnityEngine.Random.Range(TopLeft.x, BottomRight.x), UnityEngine.Random.Range(minY, maxY), 0);
            if (IsGoodVector(testVector)) return testVector;
            if (++nodeTimeout > TimeoutThreshold)
            {
                AddNodes(); // reroll
                distanceTimeout++;
            }
            if (distanceTimeout > 5)
            {
                Debug.LogWarningFormat("Invalid Distance, Reducing {0} to {1}", NodeDistanceFromEachOther, --NodeDistanceFromEachOther);
                if (startDistance / 2 < NodeDistanceFromEachOther) throw new Exception("Invalid Distance to find nodes");
                distanceTimeout = 0;
            }      
        }
    }



    bool IsGoodVector(Vector3 v)
    {
        for (var i = 0; i < _nodes.Count; ++i)
        {
            if (VMath.DistWithinThresh(_nodes[i].Position,v, NodeDistanceFromEachOther)) return false; //if distance < threshold
        }
        return true;
    }


    class VertexNode //: IEquatable<VertexNode>
    {
        public int ConnectedNodeA { get; private set; }
        public int ConnectedNodeB { get; private set; }
        //public int Distance { get; private set; }
        public VertexNode(int a, int b, int d = int.MinValue)
        {

            ConnectedNodeA = a;
            ConnectedNodeB = b;
           // Distance = d;
        }
       
        public override int GetHashCode()
        {
            return ConnectedNodeA;
        }
    }

    void CreateSpanningTree() //Prims Minimum Spanning Tree
    {
        var vertexes = new HashSet<VertexNode>();
        var visited = new List<int>() {_random.Next(0, _nodes.Count)};//pick arbitrary root 
        var timeout = 0;
        while (visited.Count < _nodes.Count) //until all vertices reached
        {

            var min = int.MaxValue;
            var connectionId = int.MinValue;
            var nodeId = int.MinValue;
            foreach (var id in visited)
            {
                for (var i = 0; i < _totalNodes; ++i)
                {
                    if (id != i //this is self
                        && !UsedVertice(visited, i) //visited node
                        && min > _nodes[id].DistanceTo[i] // min
                        && !EdgeExists(vertexes, i, id)) // a/b vs b/a
                    {
                        min = _nodes[id].DistanceTo[i];
                        connectionId = i;
                        nodeId = id;
                    }
                }
            }
            vertexes.Add(new VertexNode(nodeId, connectionId, min));
            visited.Add(connectionId); //add new visited node (dont use this for star)
            if (timeout > TimeoutThreshold) throw new Exception("Spanning Tree Generation Error");
        }
        foreach (var v in vertexes)
        {
            //Debug.Log(v.ConnectedNodeA + " is connected to " + v.ConnectedNodeB + " by " + v.Distance);
            _nodes[v.ConnectedNodeA].Connections.Add(_nodes[v.ConnectedNodeB]);
            _nodes[v.ConnectedNodeB].Connections.Add(_nodes[v.ConnectedNodeA]);
        }

    }
    
    bool UsedVertice(List<int> set, int a)
    {
        foreach (var n in set)
        {
            if (a == n) return true;
        }
        return false;
    }
    bool EdgeExists(HashSet<VertexNode> set, int a, int b)
    {
        foreach (var v in set)
        {
            if ((v.ConnectedNodeA == a && v.ConnectedNodeB == b) 
                || (v.ConnectedNodeA == b && v.ConnectedNodeB == a))
            return true;
        }
        return false;
    }
   
}
