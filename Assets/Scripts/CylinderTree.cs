using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CylinderTree : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh trunkMesh;

    private List<Vector3> trunkVertices;
    private List<int> trunkIndices;
    System.Random random = new System.Random();

    public float height = 1;
    public float radius = 1;
    public int numberOfSectors = 30;
    public int numberOfVerticalComponents = 2;
    public Vector3 center = new Vector3(0, 0, 0);
    public Mesh tempMesh;

    public float basicRadiusX = 1;
    public float basicRadiusZ = 0.5f;

    public int recursionRate = 1;
    public int angle = 30;
    public int branchFactor = 4;
    public float widthForRecursion = 0.5f;
    public float heightForRecursion = 2;
    public float resizeFactor = 2.0f / 3.0f;

    public float m_1 = 4;
    public float n1_1 = 12;
    public float n2_1 = 15;
    public float n3_1 = 15;

    public float m_2 = 4;
    public float n1_2 = 12;
    public float n2_2 = 15;
    public float n3_2 = 15;
    public float step = 0.5f;

    void OnEnable ()
    {
        Debug.Log("CylinderTree.OnEnable");
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    private void OnValidate()
    {

    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        trunkIndices = new List<int>();
        trunkVertices = new List<Vector3>();       

        if (trunkMesh)
            DestroyImmediate(trunkMesh);

        trunkMesh = new Mesh();
        trunkMesh.name = "trunkMesh";

        GenerateRecursive();
        //GenerateCylinder();
        //GenerateCylinderWithBothSurfaces();
        //GenerateSuperFormula3D();      
    }

    private void GenerateRecursive()
    {
        GenerateRecursiveTree(0, center, Quaternion.identity, widthForRecursion, heightForRecursion);

        trunkMesh.SetVertices(trunkVertices);
        trunkMesh.SetIndices(trunkIndices.ToArray(), MeshTopology.Triangles, 0);

        trunkMesh.RecalculateNormals();
        trunkMesh.UploadMeshData(markNoLogerReadable: false);
        trunkMesh.Optimize();
        meshFilter.sharedMesh = trunkMesh;
    }

    private void GenerateSuperFormula3D()
    {
        SuperFormula superFormula = new SuperFormula();
        Mesh mesh = superFormula.GenerateSuperFormula(m_1, n1_1, n2_1, n3_1, m_2, n1_2, n2_2, n3_2, step);

        trunkMesh.vertices = mesh.vertices;
        trunkMesh.SetIndices(mesh.GetIndices(0), MeshTopology.Triangles, 0);
        trunkMesh.RecalculateNormals();
        trunkMesh.UploadMeshData(markNoLogerReadable: false);
        trunkMesh.Optimize();
        meshFilter.sharedMesh = trunkMesh;
    }

    private void GenerateCylinder()
    {
        for (int i = 0; i < numberOfVerticalComponents; i++)
        {
            GenerateCylinderMeshWithOneSide(height, basicRadiusX / (i + 4), basicRadiusZ / (i + 4), basicRadiusX / (i + 5), basicRadiusZ / (i + 5), i);
        }
        trunkMesh.SetVertices(trunkVertices);
        trunkMesh.SetIndices(trunkIndices.ToArray(), MeshTopology.Triangles, 0);
        trunkMesh.RecalculateNormals();
        trunkMesh.UploadMeshData(markNoLogerReadable: false);
        trunkMesh.Optimize();
        meshFilter.sharedMesh = trunkMesh;
    }

    private void GenerateCylinderWithBothSurfaces()
    {
        for (int i = 0; i < numberOfVerticalComponents; i++)
        {
            GenerateCylinderMeshWithBothSides(center + new Vector3(0,i* height,0), height, basicRadiusX/(i+2), basicRadiusZ/(i+2), basicRadiusX / (i +3), basicRadiusZ / (i + 3), i);
        }
        trunkMesh.SetVertices(trunkVertices);
        trunkMesh.SetIndices(trunkIndices.ToArray(), MeshTopology.Triangles, 0);
        trunkMesh.RecalculateNormals();
        trunkMesh.UploadMeshData(markNoLogerReadable: false);
        trunkMesh.Optimize();
        meshFilter.sharedMesh = trunkMesh;
    }

    private void GenerateRecursiveTree(int iteration, Vector3 centerCoordinate, Quaternion basicRotation, float branchWidth, float branchHeight)
    {
        int index = trunkVertices.Count;

        Branch branch = GenerateCylinderForRecursion(branchHeight, branchWidth, branchWidth, branchWidth, branchWidth, index);

        foreach (var vertice in branch.bottomVertices)
        {
            Vector3 rotatedVertice = basicRotation * vertice;
            rotatedVertice += centerCoordinate;
            trunkVertices.Add(rotatedVertice);
        }
        foreach (var vertice in branch.topVertices)
        {
            Vector3 rotatedVertice = basicRotation * vertice;
            rotatedVertice += centerCoordinate;
            trunkVertices.Add(rotatedVertice);
        }
        foreach (var indice in branch.indices)
        {
            trunkIndices.Add(indice);
        }

        if(iteration < recursionRate)
        {
            float nextBranchWidth = branchWidth * resizeFactor;
            float nextBranchHeight = branchHeight * resizeFactor;            

            int nextIteration = iteration + 1;

            Vector3 direction = basicRotation * new Vector3(0, branchHeight, 0);
            Vector3 nextCenter = direction + centerCoordinate;

            for (int i = 0; i < branchFactor; i++)
            {
                double iRandom = random.NextDouble();
                //Debug.Log("ITERATION: " + iteration);
                //Debug.Log("IRANDOM * RECURSIONRATE: " + iRandom * recursionRate);
                if (iRandom * recursionRate > iteration)
                {
                    float nextRotationY = i * 360.0f / branchFactor;
                    Quaternion rotateAroundCurrentVerticalAxis = Quaternion.AngleAxis(nextRotationY, direction);

                    Quaternion nextRotation = rotateAroundCurrentVerticalAxis * basicRotation * Quaternion.Euler(0, 0, angle + ((int)(random.NextDouble() * 20) - 10));

                    GenerateRecursiveTree(nextIteration, nextCenter, nextRotation, nextBranchWidth, nextBranchHeight);
                }
            }
        }
    }

    private Branch GenerateCylinderForRecursion(float height, float bottomRadiusX, float bottomRadiusZ, float topRadiusX, float topRadiusZ, int offset)
    {
        //int offset = index * 2 * numberOfSectors;

        List<Vector3> bottomVertices = new List<Vector3>();
        List<Vector3> topVertices = new List<Vector3>();
        List<int> indices = new List<int>();

        for (int i = offset; i < (numberOfSectors + offset); i++)
        {
            float bottomVerticeX = (float)(bottomRadiusX * Math.Sin(i * 2 * Math.PI / numberOfSectors));
            float bottomVerticeY = 0;
            float bottomVerticeZ = (float)(bottomRadiusZ * Math.Cos(i * 2 * Math.PI / numberOfSectors));

            float topVerticeX = (float)(topRadiusX * Math.Sin(i * 2 * Math.PI / numberOfSectors));
            float topVerticeY = height;
            float topVerticeZ = (float)(topRadiusZ * Math.Cos(i * 2 * Math.PI / numberOfSectors));

            bottomVertices.Add(new Vector3(bottomVerticeX, bottomVerticeY, bottomVerticeZ));
            topVertices.Add(new Vector3(topVerticeX, topVerticeY, topVerticeZ));
        }

        for (int i = offset; i < numberOfSectors + offset - 1; i++)
        {
            indices.Add(i);
            indices.Add(numberOfSectors + i + 1);
            indices.Add(numberOfSectors + i);
            indices.Add(i);
            indices.Add(i + 1);
            indices.Add(numberOfSectors + i + 1);
        }
        indices.Add(numberOfSectors + offset - 1);
        indices.Add(numberOfSectors + offset);
        indices.Add(2 * numberOfSectors + offset - 1);
        indices.Add(numberOfSectors + offset - 1);
        indices.Add(offset);
        indices.Add(numberOfSectors + offset);
        return new Branch(bottomVertices, topVertices, indices);
    }

    private void GenerateCylinderMeshWithOneSide(float height, float lowerRadiusX, float lowerRadiusZ, float higherRadiusX, float higherRadiusZ, int index)
    {
        int offset = index * 2 * numberOfSectors;

        List<Vector3> lowerVertices = new List<Vector3>();
        List<Vector3> higherVertices = new List<Vector3>();
        List<int> indices = new List<int>();

        for (int i = offset; i < (numberOfSectors + offset); i++)
        {
            float lowerVerticeX = (float)(lowerRadiusX * Math.Sin(i * 2 * Math.PI / numberOfSectors));
            float lowerVerticeY = index * height;
            /*if(index >= numberOfVerticalComponents)
                lowerVerticeY -= height * numberOfVerticalComponents;*/
            float lowerVerticeZ = (float)(lowerRadiusZ * Math.Cos(i * 2 * Math.PI / numberOfSectors));

            float higherVerticeX = (float)(higherRadiusX * Math.Sin(i * 2 * Math.PI / numberOfSectors));
            float higherVerticeY = (index + 1) * height;
            /*if (index >= numberOfVerticalComponents)
                higherVerticeY -= height * numberOfVerticalComponents;*/
            float higherVerticeZ = (float)(higherRadiusZ * Math.Cos(i * 2 * Math.PI / numberOfSectors));

            lowerVertices.Add(new Vector3(lowerVerticeX, lowerVerticeY, lowerVerticeZ));
            higherVertices.Add(new Vector3(higherVerticeX, higherVerticeY, higherVerticeZ));
        }

        for (int i = offset; i < numberOfSectors + offset - 1; i++)
        {
            indices.Add(i);
            indices.Add(numberOfSectors + i + 1);
            indices.Add(numberOfSectors + i);
            indices.Add(i);
            indices.Add(i + 1);
            indices.Add(numberOfSectors + i + 1);
        }
        indices.Add(numberOfSectors + offset - 1);
        indices.Add(numberOfSectors + offset);
        indices.Add(2 * numberOfSectors + offset - 1);
        indices.Add(numberOfSectors + offset - 1);
        indices.Add(offset);
        indices.Add(numberOfSectors + offset);

        foreach (var vertice in lowerVertices)
        {
            trunkVertices.Add(vertice);
        }
        foreach (var vertice in higherVertices)
        {
            trunkVertices.Add(vertice);
        }
        foreach (var i in indices)
        {
            trunkIndices.Add(i);
        }
    }

    private void GenerateCylinderMeshWithBothSides(Vector3 center, float height, float radiusX, float radiusZ, float upperRadiusX, float upperRadiusZ, int index)
    {
        int offset = index * 2 * numberOfSectors;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> higherVertices = new List<Vector3>();
        List<int> indices = new List<int>();

        if(offset == 0)
            vertices.Add(center);

        for (int i = offset; i < (numberOfSectors + offset); i++)
        {
            vertices.Add(new Vector3((float)(radiusX * Math.Sin(i * 2 * Math.PI / numberOfSectors)), index * height, (float)(radiusZ * Math.Cos(i * 2 * Math.PI / numberOfSectors))));
            higherVertices.Add(new Vector3((float)(upperRadiusX * Math.Sin(i * 2 * Math.PI / numberOfSectors)), (index + 1) * height, (float)(upperRadiusZ * Math.Cos(i * 2 * Math.PI / numberOfSectors))));
        }
        //higherVertices.Add(center + new Vector3(0, height, 0));

        /*//lower circle
        for (int i = 1; i < numberOfSectors; i++)
        { 
            indices.Add(0);
            indices.Add(i + 1);
            indices.Add(i);
        }
        indices.Add(0);
        indices.Add(1);
        indices.Add(numberOfSectors);

        for (int i = 1; i < numberOfSectors; i++)
        {
            indices.Add(0);
            indices.Add(i);
            indices.Add(i + 1);
        }
        indices.Add(0);
        indices.Add(numberOfSectors);
        indices.Add(1);*/

        /*foreach (var vertice in higherVertices)
        {
            vertices.Add(vertice);
        }*/

        for (int i = 1 + offset; i < numberOfSectors + offset; i++)
        {
            indices.Add(i);
            indices.Add(numberOfSectors + i + 1);
            indices.Add(numberOfSectors + i);
            indices.Add(i);
            indices.Add(i + 1);
            indices.Add(numberOfSectors + i + 1);
        }
        indices.Add(numberOfSectors + offset);
        indices.Add(numberOfSectors + 1 + offset);
        indices.Add(2 * numberOfSectors + offset);
        indices.Add(numberOfSectors + offset);
        indices.Add(1 + offset);
        indices.Add(numberOfSectors + 1 + offset);
        for (int i = 1 + offset; i < numberOfSectors + offset; i++)
        {
            indices.Add(i);
            indices.Add(numberOfSectors + i);
            indices.Add(numberOfSectors + i + 1);
            indices.Add(i);
            indices.Add(numberOfSectors + i + 1);
            indices.Add(i + 1);
        }
        indices.Add(numberOfSectors + offset);
        indices.Add(2*numberOfSectors + offset);
        indices.Add(numberOfSectors + 1 + offset);
        indices.Add(numberOfSectors + offset);
        indices.Add(numberOfSectors + 1 + offset);
        indices.Add(1 + offset);
        /*//higher circle
        for (int i = 1; i < numberOfSectors; i++)
        {
            indices.Add(0);
            indices.Add(i + 1);
            indices.Add(i);
        }
        indices.Add(0);
        indices.Add(numberOfSectors);
        indices.Add(1);*/

        /*mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();

        mesh.UploadMeshData(markNoLogerReadable: false);*/
        foreach(var vertice in vertices)
        {
            trunkVertices.Add(vertice);
        }
        foreach(var vertice in higherVertices)
        {
            trunkVertices.Add(vertice);
        }
        foreach(var i in indices)
        {
            trunkIndices.Add(i);
        }
    }

    public class Branch
    {
        public List<Vector3> bottomVertices;
        public List<Vector3> topVertices;
        public List<int> indices;

        public Branch(List<Vector3> bottomVertices, List<Vector3> topVertices, List<int> indices)
        {
            this.bottomVertices = bottomVertices;
            this.topVertices = topVertices;
            this.indices = indices;
        }
    }
}
