using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace DSA
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Maze_Backtracker : MonoBehaviour
    {
        public enum Direction
        {
            North,
            East,
            South,
            West
        };

        private class Tile
        {
            public Vector2Int       m_vCoord;
            public bool             m_bVisited;
            public bool             m_bOnStack;
            public Wall[]           m_walls = new Wall[4];
            public Tile             m_parentTile;

            #region Properties

            public Color Color
            {
                get
                {
                    return !m_bVisited && !m_bOnStack ? Color.gray :
                           m_bVisited && m_bOnStack ? new Color(1.0f, 0.5f, 0.0f) :
                           Color.white;
                }
            }

            public Rect TileRect => new Rect(m_vCoord.x, m_vCoord.y, 1.0f, 1.0f);

            #endregion
        }

        private class Wall
        {
            public Vector2          m_vCenter;
            public Tile[]           m_tiles;
            public Direction        m_direction;

            const float             WALL_THICKNESS = 0.1f;

            #region Properties

            public Rect WallRect
            {
                get
                {
                    Vector2 vForward = sm_directions[(int)m_direction];
                    Vector2 vRight = new Vector2(-vForward.y, vForward.x);

                    Vector2 v1 = m_vCenter - vRight * (0.5f + WALL_THICKNESS) - vForward * WALL_THICKNESS;
                    Vector2 v2 = m_vCenter + vRight * (0.5f + WALL_THICKNESS) + vForward * WALL_THICKNESS;

                    Vector2 vMin = new Vector2(Mathf.Min(v1.x, v2.x), Mathf.Min(v1.y, v2.y));
                    Vector2 vMax = new Vector2(Mathf.Max(v1.x, v2.x), Mathf.Max(v1.y, v2.y));

                    return new Rect(vMin, vMax - vMin);
                }
            }

            #endregion
        }

        [SerializeField]
        public Vector2Int           m_vSize = new Vector2Int(8, 6);

        private Tile[,]             m_maze;

        private Mesh                m_mesh;

        private static Vector2Int[] sm_directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        #region Properties

        private Tile this[int x, int y]
        {
            get
            {
                if (x >= 0 && y >= 0 &&
                    x < m_vSize.x && y < m_vSize.y)
                {
                    return m_maze[x, y];
                }

                return null;
            }
        }

        private IEnumerable<Tile> AllTiles
        {
            get
            {
                for (int y = 0; y < m_vSize.y; ++y)
                {
                    for (int x = 0; x < m_vSize.x; ++x)
                    {
                        yield return m_maze[x, y];
                    }
                }
            }
        }

        private IEnumerable<Wall> AllWalls
        {
            get
            {
                for (int y = 0; y < m_vSize.y; ++y)
                {
                    for (int x = 0; x < m_vSize.x; ++x)
                    {
                        Tile t = this[x, y];
                        if (t.m_walls[(int)Direction.West] != null) yield return t.m_walls[(int)Direction.West];
                        if (t.m_walls[(int)Direction.South] != null) yield return t.m_walls[(int)Direction.South];

                        if (x == m_vSize.x - 1 ||
                            y == m_vSize.y - 1)
                        {
                            if (t.m_walls[(int)Direction.East] != null) yield return t.m_walls[(int)Direction.East];
                            if (t.m_walls[(int)Direction.North] != null) yield return t.m_walls[(int)Direction.North];
                        }
                    }
                }
            }
        }

        #endregion

        private void Start()
        {
            // create mesh
            m_mesh = new Mesh();
            m_mesh.name = "Maze";
            m_mesh.hideFlags = HideFlags.DontSave;
            m_mesh.indexFormat = IndexFormat.UInt32;
            m_mesh.MarkDynamic();

            // assign mesh
            GetComponent<MeshFilter>().mesh = m_mesh;

            InitializeMaze();
            UpdateMesh();
            StartCoroutine(GenerateMaze());
        }

        protected void InitializeMaze()
        {
            // create tiles
            m_maze = new Tile[m_vSize.x, m_vSize.y];
            for (int y = 0; y < m_vSize.y; ++y)
            {
                for (int x = 0; x < m_vSize.x; ++x)
                {
                    m_maze[x, y] = new Tile { m_vCoord = new Vector2Int(x, y) };
                }
            }

            // create walls
            for (int y = 0; y <= m_vSize.y; ++y)
            {
                for (int x = 0; x <= m_vSize.x; ++x)
                {
                    Tile tile = this[x, y];
                    Tile tileWest = this[x - 1, y];
                    Tile tileSouth = this[x, y - 1];

                    if (tile != null || tileWest != null)
                    {
                        Wall wall = new Wall
                        {
                            m_vCenter = new Vector2(x, y + 0.5f),
                            m_tiles = new Tile[] { tile, tileWest },
                            m_direction = Direction.West
                        };

                        if (tile != null) tile.m_walls[(int)Direction.West] = wall;
                        if (tileWest != null) tileWest.m_walls[(int)Direction.East] = wall;
                    }

                    if (tile != null || tileSouth != null)
                    {
                        Wall wall = new Wall
                        {
                            m_vCenter = new Vector2(x + 0.5f, y),
                            m_tiles = new Tile[] { tile, tileSouth },
                            m_direction = Direction.South
                        };

                        if (tile != null) tile.m_walls[(int)Direction.South] = wall;
                        if (tileSouth != null) tileSouth.m_walls[(int)Direction.North] = wall;
                    }
                }
            }
        }

        IEnumerator GenerateMaze()
        {
            // initial tile coordinate
            Vector2Int vStart = new Vector2Int(Random.Range(0, m_vSize.x), Random.Range(0, m_vSize.y));
            Tile start = this[vStart.x, vStart.y];
            start.m_bVisited = true;

            // add start to stack
            Stack<Tile> backtrackerStack = new Stack<Tile>();
            backtrackerStack.Push(start);
            start.m_bOnStack = true;

            // while the stack is not empty 
            while (backtrackerStack.Count > 0)
            {
                // get current tile
                Tile current = backtrackerStack.Pop();
                current.m_bOnStack = false;

                // find unvisited neighbors
                List<Tile> unvisitedNeighbors = new List<Tile>();
                foreach(Vector2Int vDir in sm_directions)
                {
                    Vector2Int vNeighbor = current.m_vCoord + vDir;
                    Tile neighbor = this[vNeighbor.x, vNeighbor.y];
                    if (neighbor != null && !neighbor.m_bVisited)
                    {
                        unvisitedNeighbors.Add(neighbor);
                    }
                }

                // do we have neighbors?
                if (unvisitedNeighbors.Count > 0)
                {
                    // push current tile back on stack
                    backtrackerStack.Push(current);
                    current.m_bOnStack = true;
                    Tile nextTile = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)]; 
                    int numberOfWalls = Enum.GetValues(typeof(Direction)).Length;
                    int currentTileSharedWallIndex = -1;
                    int nextTileSharedWallIndex = -1;
                    for (int wall = 0; wall < numberOfWalls; ++wall)
                    {
                        if(current.m_walls[wall] == nextTile.m_walls[(wall+2)%numberOfWalls])
                        {
                            currentTileSharedWallIndex = wall;
                            nextTileSharedWallIndex = (wall+2)%numberOfWalls;
                            break;
                        }
                    }

                    current.m_walls[currentTileSharedWallIndex] = null;
                    nextTile.m_walls[nextTileSharedWallIndex] = null;

                    // push next tile 
                    nextTile.m_bVisited = true;
                    nextTile.m_parentTile = current;
                    backtrackerStack.Push(nextTile);
                    nextTile.m_bOnStack = true;
                }

                UpdateMesh();
                yield return null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (m_maze == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Vector3 vOffset = new Vector3(0.5f, 0.5f, -0.2f);
            foreach (Tile tile in AllTiles)
            {
                Vector3 v1 = (Vector3)(Vector2)tile.m_vCoord + vOffset;
                if (tile.m_parentTile != null)
                {
                    Vector3 v2 = (Vector3)(Vector2)tile.m_parentTile.m_vCoord + vOffset;
                    Gizmos.DrawLine(v1, v2);
                }
            }
        }

        #region Mesh Creation

        protected void UpdateMesh()
        {
            if (m_maze == null)
            {
                return;
            }

            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<int> triangles = new List<int>();

            // add floor and move floor in Z
            foreach (Tile tile in AllTiles)
            {
                AddQuad(tile.TileRect, 0.02f, tile.Color, vertices, colors, triangles);
            }

            // create walls
            foreach (Wall wall in AllWalls)
            {
                AddQuad(wall.WallRect, 0.0f, Color.black, vertices, colors, triangles);
            }

            // create mesh
            m_mesh.Clear();
            m_mesh.subMeshCount = 2;
            m_mesh.vertices = vertices.ToArray();
            m_mesh.colors = colors.ToArray();
            m_mesh.triangles = triangles.ToArray();
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
        }

        private void AddQuad(Rect r, float fZ, Color c, List<Vector3> vertices, List<Color> colors, List<int> triangles)
        {
            int iStart = vertices.Count;
            vertices.AddRange(new Vector3[]{
                    new Vector3(r.x, r.y, fZ),
                    new Vector3(r.x, r.yMax, fZ),
                    new Vector3(r.xMax, r.yMax, fZ),
                    new Vector3(r.xMax, r.y, fZ)
                });

            colors.AddRange(new Color[] { c, c, c, c });

            triangles.AddRange(new int[]{
                    iStart + 0, iStart + 1, iStart + 2,
                    iStart + 2, iStart + 3, iStart + 0
                });
        }

        #endregion
    }
}