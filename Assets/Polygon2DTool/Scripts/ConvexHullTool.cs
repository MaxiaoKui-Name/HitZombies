using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _01_ConvexHull
{
    public class ConvexHullTool : MonoBehaviour
    {
        PolygonCollider2D polygonCollider2D;
        [ContextMenu("Computer")]
        public void Computer()
        {
            List<Point> points = new List<Point>();
            foreach (Transform trans in transform)
            {
                GetPoint(points, trans);
            }
            List<Point> grahamScan_Points = ConvexHull.GrahamScan(points);
            List<Vector2> result = new List<Vector2>();
            Vector3 pos1;
            Vector3 pos2;
            for (int i = 1; i < grahamScan_Points.Count; i++)
            {
                pos1 = new Vector3((float)grahamScan_Points[i].X, (float)grahamScan_Points[i].Y, 0);
                pos2 = new Vector3((float)grahamScan_Points[i - 1].X, (float)grahamScan_Points[i - 1].Y, 0);
                result.Add(pos2);
                //Debug.DrawLine(pos1, pos2, Color.red, 3f);
            }
            pos1 = new Vector3((float)grahamScan_Points[0].X, (float)grahamScan_Points[0].Y, 0);
            pos2 = new Vector3((float)grahamScan_Points[grahamScan_Points.Count - 1].X, (float)grahamScan_Points[grahamScan_Points.Count - 1].Y, 0);
            result.Add(pos2);
            //Debug.DrawLine(pos1, pos2, Color.red, 3f);
            if (transform.TryGetComponent<PolygonCollider2D>(out polygonCollider2D))
                polygonCollider2D = transform.GetComponent<PolygonCollider2D>();
            else
                polygonCollider2D = transform.gameObject.AddComponent<PolygonCollider2D>();

            for (int i = 0; i < result.Count; i++)
                result[i] = transform.InverseTransformPoint(result[i]);
            polygonCollider2D.points = result.ToArray();
        }

        public void GetPoint(List<Point> points, Transform trans)
        {
            if (trans.TryGetComponent(out MeshFilter mesh2))
            {
                List<Vector3> uniqueNodes = new List<Vector3>(); // 不重复节点列表
                List<Vector3> _pointList = new List<Vector3>();
                mesh2.sharedMesh.GetVertices(_pointList);
                // 遍历原始列表中的每个节点
                foreach (Vector3 node in _pointList)
                {
                    // 检查节点是否已经存在于新列表中
                    if (!uniqueNodes.Contains(node))
                    {
                        // 如果节点不存在于新列表中，将其添加到新列表中
                        uniqueNodes.Add(node);
                    }
                }
                for (int i = 0; i < uniqueNodes.Count; i++)
                {
                    Vector3 point = trans.TransformPoint(uniqueNodes[i]);
                    Point point1 = new Point(point.x, point.y, point.z);
                    points.Add(point1);
                }
            }
        }
    }
}
