using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadTree<T>
{
    private int maxObjects = 10;
    private int maxLevels = 7;

    private int level;
    private List<T> objects;
    private Rect bounds;
    private QuadTree<T>[] nodes;

    public QuadTree(int level, Rect bounds)
    {
        this.level = level;
        this.bounds = bounds;
        objects = new List<T>();
        nodes = new QuadTree<T>[4];
    }

    public void Clear()
    {
        objects.Clear();

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null)
            {
                nodes[i].Clear();
                nodes[i] = null;
            }
        }
    }

    private void Split()
    {
        float subWidth = bounds.width / 2;
        float subHeight = bounds.height / 2;
        float x = bounds.x;
        float y = bounds.y;

        nodes[0] = new QuadTree<T>(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
        nodes[1] = new QuadTree<T>(level + 1, new Rect(x, y, subWidth, subHeight));
        nodes[2] = new QuadTree<T>(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
        nodes[3] = new QuadTree<T>(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
    }

    private int GetIndex(Rect objBounds)
    {
        int index = -1;
        double verticalMidpoint = bounds.x + (bounds.width / 2);
        double horizontalMidpoint = bounds.y + (bounds.height / 2);

        // Object can completely fit within the top quadrants
        bool topQuadrant = objBounds.y + objBounds.height < horizontalMidpoint;
        // Object can completely fit within the bottom quadrants
        bool bottomQuadrant = objBounds.y > horizontalMidpoint;

        // Object can completely fit within the left quadrants
        if (objBounds.x + objBounds.width < verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 1;
            }
            else if (bottomQuadrant)
            {
                index = 2;
            }
        }
        // Object can completely fit within the right quadrants
        else if (objBounds.x > verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 0;
            }
            else if (bottomQuadrant)
            {
                index = 3;
            }
        }

        return index;
    }

    public void Insert(T obj, Rect objBounds)
    {
        if (nodes[0] != null)
        {
            int index = GetIndex(objBounds);

            if (index != -1)
            {
                nodes[index].Insert(obj, objBounds);
                return;
            }
        }

        objects.Add(obj);

        if (objects.Count > maxObjects && level < maxLevels)
        {
            if (nodes[0] == null)
            {
                Split();
            }

            int i = 0;
            while (i < objects.Count)
            {
                int index = GetIndex(objBounds);
                if (index != -1)
                {
                    nodes[index].Insert(objects[i], objBounds);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public List<T> Retrieve(List<T> returnObjects, Rect objBounds)
    {
        int index = GetIndex(objBounds);
        if (index != -1 && nodes[0] != null)
        {
            nodes[index].Retrieve(returnObjects, objBounds);
        }

        returnObjects.AddRange(objects);

        return returnObjects;
    }
}