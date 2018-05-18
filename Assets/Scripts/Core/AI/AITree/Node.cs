using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node Struction for AI Tree
/// We just use binary tree because it's enough and simple:)
/// </summary>
public class Node<T> :MonoBehaviour{
    private T data;
    private Node<T> lChild;
    private Node<T> rChild;

    public Node(T data, Node<T> ln, Node<T> rn)
    {
        this.data = data;
        this.lChild = ln;
        this.rChild = rn;
    }

    public Node(Node<T> ln, Node<T> rn)
    {
        this.data = default(T);
        this.lChild = ln;
        this.rChild = rn;
    }

    public Node(T data)
    {
        this.data = data;
        lChild = null;
        rChild = null;
    }

    public Node()
    {
        data = default(T);
        lChild = null;
        rChild = null;
    }

    public T Data
    {
        get { return this.data; }
        set { this.data = value; }
    }

    public Node<T> LChild
    {
        get { return this.lChild; }
        set { this.lChild = value; }
    }

    public Node<T> RChild
    {
        get { return this.rChild; }
        set { this.rChild = value; }
    }

}
