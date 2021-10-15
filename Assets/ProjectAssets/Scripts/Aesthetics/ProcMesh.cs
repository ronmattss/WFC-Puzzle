using System;
using System.Collections.Generic;
using ProjectAssets.Scripts.Aesthetics.Audio;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace ProjectAssets.Scripts.Aesthetics
{
    [RequireComponent((typeof(MeshFilter)))]
    public class ProcMesh : MonoBehaviour
    {
        private Mesh mesh;

        private Vector3[] vertices;
        private int[] triangles;
        private Color[] colors;

        private int[] randIndex;
        private int[] randIndexSecond;
        private List<int> vertIndexes = new List<int>();


        public int xSize = 20;
        public int zSize = 20;
        public Gradient gradient;

        private float minTerrainHeight;
        private float maxTerrainHeight;

        public AudioVertexHeight audioResponse;
        public AudioVertexHeight audioResponseSecond;


        void Start()
        {

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            CreateShape();
            UpdateMesh();

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                vertIndexes.Add(i);
                
            }

            var bias = UnityEngine.Random.Range(100, 500);
            var biasSecond = UnityEngine.Random.Range(10, 500);

            randIndex =  new int[bias];
            randIndex = SelectRandomVertices(bias);
            randIndexSecond = new int[biasSecond];
            randIndexSecond = SelectRandomVertices(biasSecond);

        }



        int[] SelectRandomVertices(int bias)
        {
            Vector3[] verts = mesh.vertices;

           
            int[] verticeIndex = new int[bias];
            Vector3[] newVerts = new Vector3[verticeIndex.Length];
            if (vertIndexes.Count == 0) return null;
            for (int i = 0; i < bias; i++)
            {
                verticeIndex[i] = vertIndexes[ UnityEngine.Random.Range(0, vertIndexes.Count)];
                vertIndexes.RemoveAt(i);
            }



            return verticeIndex;
        }

        void AdjustScaleVertex(AudioVertexHeight adjuster,int[] vertsIndecises)
        {

            if(vertsIndecises == null) return;
            int[] vertIndex = vertsIndecises;
            Vector3[] verts = mesh.vertices;
            for (int i = 0; i < vertIndex.Length; i++)
            {
                float height = (float) adjuster.GetScale().y;
                verts[vertIndex[i]] = new Vector3(vertices[vertIndex[i]].x,height,vertices[vertIndex[i]].z);
                if (height > maxTerrainHeight)
                    maxTerrainHeight = height;
                if (height < minTerrainHeight)
                    minTerrainHeight = height;
            }
            
            triangles = new int[xSize * zSize * 6];
            int vert = 0;
            int tris = 0;
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + xSize + 1;
                    triangles[tris + 5] = vert + xSize + 2;
                    vert++;
                    tris += 6;
                }

                vert++;
            }
            
            colors = new Color[vertices.Length];
            for (int z = 0, i = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    float height =Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight, verts[i].y);
                    colors[i] = gradient.Evaluate(height);
                    i++;
                }
            }
            //
            //
            // for (int z = 0, i = 0; z <= zSize; z++)
            // {
            //     for (int x = 0; x <= xSize; x++)
            //     {
            //         // float height = (float) Mathf.PerlinNoise(x * .3f, z *.3f) *2f;
            //         //
            //         // vertices[i] = new Vector3(x, height, z);

            //         verts[i] = new Vector3(x,Mathf.Sin(Time.time),z);
            //
            //         i++;
            //     }
            // }
            //
            //
            
            mesh.vertices = verts;
            mesh.triangles = triangles;
            mesh.colors = colors;

        }

        private void Update()
        {
            AdjustScaleVertex(audioResponse,randIndex);
            AdjustScaleVertex(audioResponseSecond,randIndexSecond);

            //  Vector3[] verts = mesh.vertices;
            // // Vector3[] normals = mesh.normals;
            // //
            // // Color[] colors = mesh.colors;
            // //
            // //
            // for (int i = 0; i < vertices.Length; i++)
            // {
            //    // float height = (float) Mathf.PerlinNoise(vertices[i].x * .3f, vertices[i].z *.3f) *2f;
            //    float height = (float) audioResponse.GetScale().y;
            //
            //     //vertices[i] = new Vector3(x, height, z);
            //    // verts[i] = new Vector3(vertices[i].x,Mathf.Sin(Time.time * height *.9f),vertices[i].z);
            //    int index = UnityEngine.Random.Range(0, vertices.Length);
            //    verts[index] = new Vector3(vertices[i].x,height,vertices[i].z);
            //
            //    if (height > maxTerrainHeight)
            //         maxTerrainHeight = height;
            //     if (height < minTerrainHeight)
            //         minTerrainHeight = height;
            //    // verts[i] +=verts[i].y * Mathf.Sin(Time.time);
            // }
            //
            // // float heightY = (float) audioResponse.GetScale().y;
            // // int index = UnityEngine.Random.Range(0, vertices.Length);
            // // verts[index] = new Vector3(vertices[index].x,heightY,vertices[index].z);
            // // if (heightY > maxTerrainHeight)
            // //      maxTerrainHeight = heightY;
            // //  if (heightY < minTerrainHeight)
            // //      minTerrainHeight = heightY;
            //
            // triangles = new int[xSize * zSize * 6];
            // int vert = 0;
            // int tris = 0;
            // for (int z = 0; z < zSize; z++)
            // {
            //     for (int x = 0; x < xSize; x++)
            //     {
            //         triangles[tris + 0] = vert + 0;
            //         triangles[tris + 1] = vert + xSize + 1;
            //         triangles[tris + 2] = vert + 1;
            //         triangles[tris + 3] = vert + 1;
            //         triangles[tris + 4] = vert + xSize + 1;
            //         triangles[tris + 5] = vert + xSize + 2;
            //         vert++;
            //         tris += 6;
            //     }
            //
            //     vert++;
            // }
            //
            // colors = new Color[vertices.Length];
            // for (int z = 0, i = 0; z <= zSize; z++)
            // {
            //     for (int x = 0; x <= xSize; x++)
            //     {
            //         float height =Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight, verts[i].y);
            //         colors[i] = gradient.Evaluate(height);
            //         i++;
            //     }
            // }
            // //
            // //
            // // for (int z = 0, i = 0; z <= zSize; z++)
            // // {
            // //     for (int x = 0; x <= xSize; x++)
            // //     {
            // //         // float height = (float) Mathf.PerlinNoise(x * .3f, z *.3f) *2f;
            // //         //
            // //         // vertices[i] = new Vector3(x, height, z);
            //
            // //         verts[i] = new Vector3(x,Mathf.Sin(Time.time),z);
            // //
            // //         i++;
            // //     }
            // // }
            // //
            // //
            //
            // mesh.vertices = verts;
            // mesh.triangles = triangles;
            // mesh.colors = colors;
        }

        void UpdateColors(Mesh mesh)
        {
            colors = new Color[vertices.Length];
            for (int z = 0, i = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    float height =Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight, vertices[i].y);
                    colors[i] = gradient.Evaluate(height);
                    i++;
                }
            }
        }

        void CreateShape()
        {
            vertices = new Vector3[(xSize + 1) * (zSize + 1)];
            for (int z = 0, i = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    float height = (float) Mathf.PerlinNoise(x * .3f, z *.3f) *2f;
                    
                    vertices[i] = new Vector3(x, height, z);
                    if (height > maxTerrainHeight)
                        maxTerrainHeight = height;
                    if (height < minTerrainHeight)
                        minTerrainHeight = height;
                    
                    i++;
                }
            }

            triangles = new int[xSize * zSize * 6];
            int vert = 0;
            int tris = 0;
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + xSize + 1;
                    triangles[tris + 5] = vert + xSize + 2;
                    vert++;
                    tris += 6;
                }

                vert++;
            }
            
            
            colors = new Color[vertices.Length];
            for (int z = 0, i = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    float height =Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight, vertices[i].y);
                    colors[i] = gradient.Evaluate(height);
                    i++;
                }
            }
            // for UVs
            // uvs = new Vector2[vertices.Length];
            // for (int z = 0, i = 0; z <= zSize; z++)
            // {
            //     for (int x = 0; x <= xSize; x++)
            //     {
            //         uvs[i] = new Vector2((float)x / xSize,(float)z /zSize);
            //         i++;
            //     }
            // }
            
        }

        void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.RecalculateNormals();
        }


    }
}