using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SuperFormula
{
    private float m_1 = 4;
    private float n1_1 = 12;
    private float n2_1 = 15;
    private float n3_1 = 15;

    private float m_2 = 4;
    private float n1_2 = 12;
    private float n2_2 = 15;
    private float n3_2 = 15;

    private int a = 1;
    private int b = 1;

    public float step = 0.5f;

    private int iterations;
    private int vectorsWithinIterations;

    private void SetParameters(float m1, float n11, float n21, float n31, float m2, float n12, float n22, float n32, float step)
    {
        m_1 = m1;
        n1_1 = n11;
        n2_1 = n21;
        n3_1 = n31;
        m_2 = m2;
        n1_2 = n12;
        n2_2 = n22;
        n3_2 = n32;
    }

    public Mesh GenerateSuperFormula(float m1, float n11, float n21, float n31, float m2, float n12, float n22, float n32, float step)
    {
        SetParameters(m1, n11, n21, n31, m2, n12, n22, n32, step);
        iterations = 0;
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        for (float latitude = (float)-Math.PI / 2.0f; latitude <= (float)Math.PI/2.0f; latitude += step)
        {
            for(float longitude = (float)-Math.PI; longitude <= (float)Math.PI; longitude += step)
            {
                vertices.Add(CalculateVertice(latitude, longitude));
            }
            iterations++;
        }
        vectorsWithinIterations = vertices.Count / iterations;
        mesh.SetVertices(vertices);
        indices = CalculateIndices(vertices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        return mesh;
    }

    private List<int> CalculateIndices(List<Vector3> vertices)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < iterations - 1; i++)
        {
            for (int j = 0; j < vectorsWithinIterations - 1; j++)
            {
                    indices.Add(i * vectorsWithinIterations + j);
                indices.Add((i + 1) * vectorsWithinIterations + j);
                indices.Add((i + 1) * vectorsWithinIterations + j + 1);

                    indices.Add(i * vectorsWithinIterations + j);
                indices.Add((i + 1) * vectorsWithinIterations + j + 1);
                indices.Add(i * vectorsWithinIterations + j + 1);
            }
            indices.Add(i * vectorsWithinIterations + vectorsWithinIterations - 1);
            indices.Add((i + 1) * vectorsWithinIterations + vectorsWithinIterations - 1);
            indices.Add((i + 1) * vectorsWithinIterations);

            indices.Add(i * vectorsWithinIterations + vectorsWithinIterations - 1);
            indices.Add((i + 1) * vectorsWithinIterations);
            indices.Add(i * vectorsWithinIterations);
        }
        return indices;
    }

    private Vector3 CalculateVertice(float latitude, float longitude)
    {
        float x = CalculateRadius(longitude, m_1, n1_1, n2_1, n3_1) * (float)Math.Cos(longitude) * CalculateRadius(latitude, m_2, n1_2, n2_2, n3_2) * (float)Math.Cos(latitude);
        float y = CalculateRadius(longitude, m_1, n1_1, n2_1, n3_1) * (float)Math.Sin(longitude) * CalculateRadius(latitude, m_2, n1_2, n2_2, n3_2) * (float)Math.Cos(latitude);
        float z = CalculateRadius(latitude, m_2, n1_2, n2_2, n3_2) * (float)Math.Sin(latitude);
        return new Vector3(x,z,y);
    }

    private float CalculateRadius(float angle, float m, float n1, float n2, float n3)
    {
        float firstAbs = Math.Abs((float)Math.Cos(m * angle / 4.0f) / a);
        float secondAbs = Math.Abs((float)Math.Sin(m * angle / 4.0f) / b);

        float firstPart = (float)Math.Pow(firstAbs, n2);
        float secondPart = (float)Math.Pow(secondAbs, n3);

        return (float)Math.Pow((firstPart + secondPart), (-1.0f / n1));
    }
}
