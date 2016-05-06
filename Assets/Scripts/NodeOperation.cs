using System;
using UnityEngine;
using System.Collections;
using UnityEditor;


public class NodeOperation : MonoBehaviour
{
    public float WindowTimeout;
    private float _lineWidth;

    private Node _data;

    private GameObject _nodeManager;
    private Vector3 _offset;
    public void SetNode(Node n, float size)
    {
        _data = n;
        _nodeManager = n.NodeManager;
        _offset = _nodeManager.transform.position;
        _lineWidth = size;
        BuildLines();
    }

    void BuildLines()
    {
        var line = gameObject.AddComponent<LineRenderer>();
        line.SetWidth(_lineWidth, _lineWidth);
        //Debug.Log(_data.GlobalId + " Has " +  _data.Connections.Count);
        line.SetVertexCount(_data.Connections.Count * 2 + 1);
        line.SetPosition(0, transform.position);     
        for (var i = 0; i < _data.Connections.Count; i++)
        {
            line.SetPosition(i * 2 + 1, _data.Connections[i].Position + _offset);
            line.SetPosition(i * 2 + 2, transform.position);
        }
        
    }


    bool IsLastRow(Node n)
    {
        return n.TotalNodes - n.GlobalId < n.NodesInRow;
    }

    // Use this for initialization
    void Start ()
	{
       
	}


	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnMouseUpAsButton()
    {
        _data.IsVisited = true;
        GetComponent<Renderer>().material.color = Color.red;
        Debug.Log(_data);
    }

    void OnMouseEnter()
    {
        if (!_data.IsVisited)
        {
            _data.FeedBackWindow.SetActive(true);
            _data.FeedBackWindow.GetComponent<FeedBackManager>().ResetTime();
            var scale = _data.FeedBackWindow.transform.localScale;
            _data.FeedBackWindow.transform.position = transform.position - new Vector3(scale.x / -2, scale.y / 2, 0);
        }

    }
}
