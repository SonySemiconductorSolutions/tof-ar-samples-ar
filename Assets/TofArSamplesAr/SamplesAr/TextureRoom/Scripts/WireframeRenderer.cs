/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArARSamples.TextureRoom
{
    /// <summary>
    /// This class is used to implement a wireframe representation by modifying the mesh vertex structure.
    /// You can change the fireframe by attaching it to the meshobject you wish to display.
    /// </summary>
    public class WireframeRenderer : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;

        private int[] meshTriangles;
        private Vector3[] meshVertices;
        private Vector3[] meshNormals;
        private int meshTrianglesLength;

        private Vector3[] processedVertices;
        private Vector2[] processedUVs;
        private int[] processedTriangles;
        private Vector3[] processedNormals;

        private Vector2 uv00;
        private Vector2 uv01;
        private Vector2 uv10;

        [SerializeField]
        private MeshRenderer meshRenderer;
        private Mesh mesh;

        void Start()
        {
            uv00 = new Vector2(0, 0);
            uv01 = new Vector2(0, 1);
            uv10 = new Vector2(1, 0);
        }

        void LateUpdate()
        {
            MeshUpdate();
        }

        /// <summary>
        /// This function modifies the mesh vertex information.
        /// </summary>
        void MeshUpdate()
        {
            mesh = meshFilter.sharedMesh;
            if (mesh == null) return;

            meshTriangles = mesh.triangles;
            meshVertices = mesh.vertices;
            meshNormals = mesh.normals;
            meshTrianglesLength = meshTriangles.Length;
            if (meshTrianglesLength == 0) return;

            processedVertices = new Vector3[meshTrianglesLength];
            processedUVs = new Vector2[meshTrianglesLength];
            processedTriangles = new int[meshTrianglesLength];
            processedNormals = new Vector3[meshTrianglesLength];


            for (int i = 0; i < meshTrianglesLength; i += 3)
            {
                processedVertices[i] = meshVertices[meshTriangles[i]];
                processedVertices[i + 1] = meshVertices[meshTriangles[i + 1]];
                processedVertices[i + 2] = meshVertices[meshTriangles[i + 2]];

                processedUVs[i] = uv00;
                processedUVs[i + 1] = uv10;
                processedUVs[i + 2] = uv01;

                processedTriangles[i] = i;
                processedTriangles[i + 1] = i + 1;
                processedTriangles[i + 2] = i + 2;

                processedNormals[i] = meshNormals[meshTriangles[i]];
                processedNormals[i + 1] = meshNormals[meshTriangles[i + 1]];
                processedNormals[i + 2] = meshNormals[meshTriangles[i + 2]];
            }

            mesh.Clear();
            mesh.vertices = processedVertices;
            mesh.uv = processedUVs;
            mesh.triangles = processedTriangles;
            mesh.normals = processedNormals;
        }
    }
}
