using MarchingCubesGPUProject;
using System.Collections.Generic;
using UnityEngine;

public abstract class McBaseGenerator : GPURenderer
{
    public McVert[] _verts;

    public McBaseGenerator(BaseShaders shaders, int n, int desiredBufferSize, Material material) : base(shaders, n, desiredBufferSize)
    {
        this.material = material;
    }
    private Material material;
    public abstract McData GetDefaultData();
    public GameObject GenerateMeshes(McData data)
    {
        if (data.Values.Length != DesiredBufferSize)
            throw new System.ArgumentException("Values array should have " + DesiredBufferSize + " elements");

        if (data.Colors.Length != DesiredBufferSize)
            throw new System.ArgumentException("Colors array should have " + DesiredBufferSize + " elements");

        dataBuffer.SetData(data.Values);
        dataColorBuffer.SetData(data.Colors);

        GameObject meshesObject = new GameObject("Marching Meshes");

        CleanMeshBuffer();
        CalculateMesh(meshesObject.transform);
        CalculateNormals();

        CreateMeshes(meshesObject.transform);

        return meshesObject;
    }

    private Mesh InitMesh(Transform parent)
    {
        Mesh mesh = new Mesh();
        mesh.bounds = new Bounds(new Vector3(0, N / 2, 0), new Vector3(N, N, N)); //what is it for?

        GameObject go = new GameObject("Marching Mesh");
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = material;
        go.GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        go.transform.parent = parent;
        go.transform.SetPositionAndRotation(go.transform.parent.position, go.transform.parent.rotation);


        return mesh;
    }
    private void CreateMeshes(Transform parent)
    {
        //Get the data out of the buffer.
        meshBuffer.GetData(_verts);

        var positions = new List<Vector3>();
        var normals = new List<Vector3>();
        var colors = new List<Color>();
        List<int> indexes = new List<int>();

        int meshIdx = 0;
        int idx = 0;
        int maxVerts = 65000;

        for (int i = 0; i < Size; i++)
        {
            //If the marching cubes generated a vert for this index
            //then the position w value will be 1, not -1.
            if (_verts[i].position.w != -1)
            {
                positions.Add(_verts[i].position);
                normals.Add(_verts[i].normal);
                colors.Add(_verts[i].color);
                indexes.Add(idx++);
            }

            if (idx >= maxVerts)
            {
                Mesh mesh = InitMesh(parent);
                mesh.Clear();
                mesh.SetVertices(positions);
                mesh.SetNormals(normals);
                mesh.SetColors(colors);
                mesh.SetTriangles(indexes, 0);

                idx = 0;
                positions.Clear();
                normals.Clear();
                colors.Clear();
                indexes.Clear();

                meshIdx++;
            }
        }

        Mesh lastMesh = InitMesh(parent);
        lastMesh.Clear();
        lastMesh.SetVertices(positions);
        lastMesh.SetNormals(normals);
        lastMesh.SetColors(colors);
        lastMesh.SetTriangles(indexes, 0);

    }
}
