﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMeshGen : MonoBehaviour
{
    public SquareGrid squareGrid;
    /// <summary>
    /// List of vertices for mesh rendering.
    /// </summary>
    List<Vector3> vertices;
    /// <summary>
    /// List of triangles for mesh rendering.
    /// </summary>
    List<int> triangles;
    /// <summary>
    /// A dictionary which contains lists of triangle which share a common vertex.
    /// </summary>
    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();
    public MeshFilter walls;
    /// <summary>
    /// Takes in the values from DungeonMapGen and passes them into the SquareGrid constructor.
    /// </summary>
    /// <param name="map">The int array m_map which holds the data of whether coordinates are air or walls.</param>
    /// <param name="squareSize">The size of the squares.</param>
    public void GenerateMesh(int[,] map, float squareSize)
    {
        triangleDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();
        squareGrid = new SquareGrid(map, squareSize);
        /// Initialises vertices list.
        vertices = new List<Vector3>();
        /// Initialises triangles list.
        triangles = new List<int>();
        /// Loops through every square in squareGrid.
        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }
        ///Creates a new mesh for the map
        Mesh mesh = new Mesh();
        ///Assigns the newly created mesh to the mesh component of MeshFilter.
        GetComponent<MeshFilter>().mesh = mesh;
        ///Converts the list of vertices to an array and assigns it to the mesh.
        mesh.vertices = vertices.ToArray();
        ///Converts the list of triangles to an array and assigns it to the mesh.
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        CreateWallMesh();
    }
    void CreateWallMesh()
    {

        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 5;

        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]); // left
                wallVertices.Add(vertices[outline[i + 1]]); // right
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
                wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        walls.mesh = wallMesh;
    }

    void TriangulateSquare(Square square)
    {
        /// Switch statement based on square configuration (which control nodes are active).
        switch (square.configuration)
        {
            /// cases if 1 control point is active.
            case 0:
                break;

            case 1:
                MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
                break;

            /// cases if 2 control points are active.
            case 3:
                MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 6:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                break;
            case 5:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            /// cases if 3 control points are active.
            case 7:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            /// case if 4 control points are active.
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                /// Forms a solid wall so will not be outline vertices.
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
        }
    }
    /// <summary>
    /// Creates the correct number of triangles based on how many nodes are active in the square.
    /// </summary>
    /// <param name="points">The array of points passed in based on the configuration of the square.</param>
    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);
        ///Checks how many points are needed for each possible configuration. 3 points need 1 trianlge, 4 points need 2 triangles, 5 points need 3 triangles and so on.
        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);
    }
    /// <summary>
    /// Assigns the vertex index of the node based on how many vertices have already been assigned. Also adds the position variable of the node to the vertices list.
    /// </summary>
    /// <param name="points">The array of points passed in based on the configuration of the square.</param>
    void AssignVertices(Node[] points)
    {
        ///loops through the points of the configuration for the square and assigns each vertex to a list.
        for (int i = 0; i < points.Length; i++)
        {
            /// Checks for points not yet initialised.
            if (points[i].vertexIndex == -1)
            {
                /// Assigns the vertex index to the size of the vertices list. For example, the list will be initialised with 0 values so the first vertexIndex will also be 0.
                points[i].vertexIndex = vertices.Count;
                /// Adds the Vector3 component of the node to the vertices list.
                vertices.Add(points[i].position);
            }
        }
    }
    /// <summary>
    /// Creates a triangle based on the nodes passed through
    /// </summary>
    /// <param name="a">First vertex of the triangle</param>
    /// <param name="b">Second vertex of the triangle</param>
    /// <param name="c">Third vertex of the triangle</param>
    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }
    /// <summary>
    /// Creates lists of triangles which contain a given vertex.
    /// </summary>
    /// <param name="vertexIndexKey">One vertex of the triangle.</param>
    /// <param name="triangle">The triangle the vertex belongs to.</param>
    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        /// If the vertex has already been identified, it adds the new triangle to list which contains the given vertex.
        if (triangleDictionary.ContainsKey(vertexIndexKey))
        {
            triangleDictionary[vertexIndexKey].Add(triangle);
        }
        /// If the vertex has not already been identified, it adds the new triangle to a new list which contains the given vertex.
        else
        {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictionary.Add(vertexIndexKey, triangleList);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void CalculateMeshOutlines()
    {

        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if (!checkedVertices.Contains(vertexIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1)
                {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }
    void FollowOutline(int vertexIndex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1)
        {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }
    /// <summary>
    /// Finds all triangles containing a given vertex then loops through these to see if any of the connected vertices form an edge to the map.
    /// </summary>
    /// <param name="vertexIndex">The triangle vertex being tested.</param>
    /// <returns>Returns the corresponding vertex if it forms an edge or returns -1 if it doesn't.</returns>
    int GetConnectedOutlineVertex(int vertexIndex)
    {
        /// Creates a list of triangle which contain the passed in vertex.
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];
        /// Loops through every triangle which contains the passed in vertex.
        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            /// A triangle in the list above.
            Triangle triangle = trianglesContainingVertex[i];
            /// Loops through each vertex in the triangle to see if they are shared by any other triangles and, in turn, whether they form an edge.
            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];
                /// Check to avoid comparing the vertex against itself.
                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
                {
                    if (IsOutlineEdge(vertexIndex, vertexB))
                    {
                        /// Returns the vertex which forms an outline edge with the passed in vertex.
                        return vertexB;
                    }
                }
            }
        }
        /// Returns -1 if no outline edge is found.
        return -1;
    }
    /// <summary>
    /// Finds every triangle which shares a vertex with vertexA then checks whether any of these triangles also contain vertexB.
    /// </summary>
    /// <param name="vertexA">One vertex of the line being checked.</param>
    /// <param name="vertexB">The second vertex of the line being checked.</param>
    /// <returns>Returns a bool stating whether or not the two parameters form an outline edge.</returns>
    bool IsOutlineEdge(int vertexA, int vertexB)
    {
        /// List of triangles containing vertexA;
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
        /// How many triangles share the given vertices.
        int sharedTriangleCount = 0;
        /// Loops through every triangle which contains vertexA.
        for (int i = 0; i < trianglesContainingVertexA.Count; i++)
        {
            /// checks whether the triangles which contain vertexA also contains vertexB.
            if (trianglesContainingVertexA[i].Contains(vertexB))
            {
                /// Adds 1 to the count if a triangle shares both vertices.
                sharedTriangleCount++;
                /// If the vertices are shared by more than one triangle, the line they form is not an edge.
                if (sharedTriangleCount > 1)
                {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }
    struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;

        public Triangle(int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;
            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }
        public int this[int i]
        {
            get
            {
                return vertices[i];
            }
        }
        public bool Contains(int vertexIndex)
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }
    /// <summary>
    /// Class and constructor for an array of Squares.
    /// </summary>
    /// 
    public class SquareGrid
    {
        public Square[,] squares;
        /// <summary>
        /// Constructor for SquareGrid.
        /// </summary>
        /// <param name="map">The int array m_map which holds the data of whether coordinates are air or walls.</param>
        /// <param name="squareSize">The size of the squares.</param>
        public SquareGrid(int[,] map, float squareSize)
        {
            /// The node count for x and y are equal to the length of the respective row/column in the m_map matrix.
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            /// The size of the generated map will be equal to the number of squares that will be generated multiplied by the chosen square size.
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;
            /// 2D array of controlNodes declared based on the width and height of m_map. 
            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];
            /// Loops through every node
            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    /// Every node's position is generated based on the square size.
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    /// The array of ControlNodes is populated using the position defined above, whether or not the node represents a wall and finally the size of the square.
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }
            /// 2D array of squares declared. The number of squares along the width of the map is equal to the number of Xnodes - 1 and the number of squares along the height of the map is equal to Ynodes - 1.
            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            /// Loops through every node apart from the last one as this would generate a square which would be off the map.
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    /// Populates the squares array with squares generated by adjacent controlNodes.
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }
    }
    /// <summary>
    /// Class and constructor for Squares. These squares are equivelent to the map tiles but are shrunken anchored on the map tile's centre coordinate.
    /// </summary>
    public class Square
    {
        /// <summary>
        /// The control nodes monitor whether the tile in each of the neighbouring diagonal tiles are a wall.
        /// </summary>
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        /// <summary>
        /// The nodes of each orthogonal direction from the centre of the square.
        /// </summary>
        public Node centreTop, centreRight, centreBottom, centreLeft;
        /// <summary>
        /// Configuration of mesh around each point.
        /// </summary>
        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

            centreTop = topLeft.right;
            centreRight = bottomRight.above;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;

            /// Configuration is set dependent on which surrounding nodes are active.
            if (topLeft.active)
                configuration += 8;
            if (topRight.active)
                configuration += 4;
            if (bottomRight.active)
                configuration += 2;
            if (bottomLeft.active)
                configuration += 1;
        }
    }
    /// <summary>
    /// Class and constructor for nodes surrounding map tiles.
    /// </summary>
    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos)
        {
            position = _pos;
        }
    }
    /// <summary>
    /// Class and constructor for Control nodes surrounding map tiles. These nodes are active if they diagonally border a tile which is a wall.
    /// </summary>
    public class ControlNode : Node
    {
        /// <summary>
        /// Whether or not the Control Node borders a wall.
        /// </summary>
        public bool active;
        /// <summary>
        /// The two nodes the controller node controls.
        /// </summary>
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }

    }
}
