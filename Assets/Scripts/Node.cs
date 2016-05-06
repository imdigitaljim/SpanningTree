using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {

    public int TotalNodes { get; set; }
    public int NodesInRow { get; set; }
    public int GlobalId { get; set; } //relative to the world
    public int LocalId { get; set; } //relative to the row
    public int Row { get; set; } //which row
    public Vector3 Position { get; set; }
    public GameObject NodeManager { get; set; }
    public GameObject FeedBackWindow { get; set; }
    public List<Node> Connections = new List<Node>();
    public List<int> DistanceTo = new List<int>();
    public bool IsVisited = false;

    public override string ToString()
    {
        return string.Format("GlobalId: {0}\tLocalId: {1}\tRow: {2}\tPosition: {3}\nVisited: {4}\tCurrent Connections: {5}", GlobalId, LocalId, Row, Position, IsVisited, Connections.Count);
    }
}
