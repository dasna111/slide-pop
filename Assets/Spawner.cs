using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    public GameObject[] blocks;

    [SerializeField] private Transform startAnchor;
    public Sprite[] GarbageBlock;

    //public Transform[,] grid;
    //public ArrayList occupiedPos = new ArrayList();
    

    public int[,] data = null;
    public GameObject[,] cubes = null;


    public int width = 6;
    public int height = 11;
    [SerializeField] private int startHeight = 6;

    private const float startPos = -2.5f;

    private int previous = -1;

    private float SpawnerTimer = 0;
    public float SpawnEvery;


    //   private int? prev = null;
    //private int? curr = null;

    //private int prevCount = 0;
    //   private object prevBlock;

    //   private bool Finished;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Use this for initialization
    public void Start()
    {
        //data = new int[11, 6]
        //{
        //    {-1, 1, 1 ,3 ,4 ,0},
        //    {1, -1, -1 ,3 ,4 ,0},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //};


        data = new int[height, width];
        cubes = new GameObject[height, width];

        for (int y = 0; y < startHeight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = GenerateBlock(y, x, previous);
                cubes[y, x] = Instantiate(blocks[data[y, x]], GetPosition(y, x), Quaternion.identity);
                previous = data[y, x];
            }
        }

        for (int y = startHeight; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = -1;
                cubes[y, x] = null;
            }
        }

        LoopMatch();
    }

    private void LoopMatch()
    {
        int count = Match();
        while (count > 0) count = Match();
    }

    void Update()
    {
        SpawnNewLineLoop();
    }

    private void SpawnNewLineLoop()
    {
        SpawnerTimer += Time.deltaTime;
        if (SpawnerTimer < SpawnEvery && !Input.GetKeyDown(KeyCode.Z))
            return;

        SpawnNewLine();
        SpawnerTimer = 0;
    }

    private void SpawnNewLine()
    {
        PushUp();
        GenerateRow(0);
    }

    #region Match
    private int Match()
    {
        List<Vector2Int> matchs = FindAllMatchs();
        int combo = 1;
        RemoveMatchs(matchs);
        FallDown();
        if(matchs.Count > 3) // matchs
            GarbageBlocks(matchs, combo);
        return matchs.Count;
    }

    private List<Vector2Int> FindAllMatchs()
    {
        List<Vector2Int> matchs = new List<Vector2Int>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (data[y, x] == -1)
                    continue;

                if (matchs.Contains(new Vector2Int(y, x)))
                    continue;

                matchs.AddRange(FindMatchForCell(y, x));
            }
        }

        return matchs;
    }

    private void RemoveMatchs(List<Vector2Int> matchs)
    {
        for (int i = 0; i < matchs.Count; i++)
        {
            var coords = matchs[i];
            data[coords.x, coords.y] = -1;
            Destroy(cubes[coords.x, coords.y]);
        }
    }

    private void FallDown()
    {
        for (int y = 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (data[y - 1, x] == -1)
                {
                    for (int z = y - 1; z < height - 1; z++) { 
                        data[z, x] = data[z + 1, x];
                        cubes[z, x] = cubes[z + 1, x];

                        if (cubes[z, x])
                            cubes[z, x].transform.position = GetPosition(z, x);
                    }
                }
            }
        }
    }

    private List<Vector2Int> FindMatchForCell(int y, int x)
    {
        //Debug.Log("Looking in  => " + y + " | " + x);
        List<Vector2Int> matching = new List<Vector2Int>();

        List<Vector2Int> row = FindRow(y, x);
        List<Vector2Int> column = FindColumn(y, x);

        if (row.Count > 2) matching.AddRange(row);
        if (column.Count > 2) matching.AddRange(column);

        return matching.Distinct().ToList();
    }

    private List<Vector2Int> FindColumn(int y, int x)
    {
        List<Vector2Int> matching = new List<Vector2Int>();
        matching.Add(new Vector2Int(y, x));

        for (int _y = y + 1; _y < height; _y++) // Right side
        {
            if (data[_y, x] == data[y, x])
                matching.Add(new Vector2Int(_y, x));
            else
                break;
        }

        for (int _y = y - 1; _y >= 0; _y--) // left side
        {
            if (data[_y, x] == data[y, x])
                matching.Add(new Vector2Int(_y, x));
            else
                break;
        }
        return matching;
    }

    private List<Vector2Int> FindRow(int y, int x)
    {
        List<Vector2Int> matching = new List<Vector2Int>();
        matching.Add(new Vector2Int(y, x));

        for (int _x = x + 1; _x < width; _x++) // Downward side
        {
            if (data[y, _x] == data[y, x])
                matching.Add(new Vector2Int(y, _x));
            else
                break;
        }

        for (int _x = x - 1; _x >= 0; _x--) // upward side
        {
            if (data[y, _x] == data[y, x])
                matching.Add(new Vector2Int(y, _x));
            else
                break;
        }

        return matching;
    }

    /*
    private List<Vector2Int> FindCellNeighbor(int x, int y, List<Vector2Int> matching)
    {
        matching.Add(new Vector2Int(y, x));

        for (int _y = y + 1; _y < height; _y++) // Right side
        {
            if (data[_y, x] == data[y, x])
                matching.Add(new Vector2Int(_y, x));
            else
                break;
        }

        for (int _y = y - 1; _y >= 0; _y--) // left side
        {
            if (data[_y, x] == data[y, x])
                matching.Add(new Vector2Int(_y, x));
            else
                break;
        }



        for (int _x = x + 1; _x < width; _x++) // Downward side
        {
            if (data[y, _x] == data[y, x])
                matching.Add(new Vector2Int(y, _x));
            else
                break;
        }

        for (int _x = x - 1; _x >= 0; _x--) // upward side
        {
            if (data[y, _x] == data[y, x])
                matching.Add(new Vector2Int(y, _x));
            else
                break;
        }

        //List<Vector2Int> exist = new List<Vector2Int>();


        //if (!matching.Contains(new Vector2Int(y + 1, x)) && x >= 0 && x < width && y < height  -1 && y >= 0 && data[y + 1, x] > -1)
        //{
        //    Debug.Log("Match?");
        //    if (data[y, x] == data[y + 1, x])
        //    {
        //        matching.AddRange(FindCellNeighbor(y + 1, x, matching));
        //        //exist.Add(new Vector2Int(x, y));
        //        Debug.Log("x + 1");
        //    }
        //}

        //if (!matching.Contains(new Vector2Int(y - 1, x)) && x >= 0 && x < width && y < height && y > 0 && data[y - 1, x] > -1)
        //{
        //    if (data[y, x] == data[y - 1, x])
        //    {
        //        matching.AddRange(FindCellNeighbor(y - 1, x, matching));
        //        //exist.Add(new Vector2Int(x, y));
        //        Debug.Log("x - 1");
        //    }
        //}

        //Debug.LogFormat("{0} | {1}", x,y);
        //if (!matching.Contains(new Vector2Int(y, x + 1)) && x >= 0 && x < width - 1 && y < height && y >= 0 && data[y, x + 1] > -1)
        //{
        //    if (data[y, x] == data[y, x + 1])
        //    {
        //        matching.AddRange(FindCellNeighbor(y, x + 1, matching));
        //        //exist.Add(new Vector2Int(x, y));
        //        Debug.Log("x + 1");
        //    }
        //}

        //if (!matching.Contains(new Vector2Int(y, x - 1)) && x > 0 && x < width && y < height && y >= 0 && data[y, x - 1] > -1)
        //{
        //    if (data[y, x] == data[y, x - 1])
        //    {
        //        matching.AddRange(FindCellNeighbor(y, x - 1, matching));
        //        //exist.Add(new Vector2Int(x, y));
        //        Debug.Log("x - 1");
        //    }
        //}



        //foreach (Vector2Int n in matching)
        //{
        //    if (!exist.Contains(n))
        //    {

        //        }
        //    }
        //}
        return matching;
    }*/
    #endregion

    public void Switch(int leftX, int rightX, int cursorY)
    {
        int temp = data[cursorY, leftX];
        data[cursorY, leftX] = data[cursorY, rightX];
        data[cursorY, rightX] = temp;

        var tempGO = cubes[cursorY, leftX];
        cubes[cursorY, leftX] = cubes[cursorY, rightX];
        cubes[cursorY, rightX] = tempGO;

        if(cubes[cursorY, leftX])
            cubes[cursorY, leftX].transform.position = GetPosition(cursorY, leftX);

        if (cubes[cursorY, rightX])
            cubes[cursorY, rightX].transform.position = GetPosition(cursorY, rightX);

        LoopMatch();
    }

    /*
    private void MatchRow()
    {
		Stack matchingBlocks = new Stack();
        

        for (int y = 0; y < Grid.Instance.height; y++)
        {
			for(int x = 0; x < Grid.Instance.width; x++)
            {
		 		if(matchingBlocks.Count > 0)
                {
					if(Grid.Instance.grid[x, y] != null)
                    {
                        string prevBlock = null;


                        if (Grid.Instance.grid[(int)matchingBlocks.Peek(), y] != null)
                            prevBlock = Grid.Instance.grid[(int)matchingBlocks.Peek(), y].name.Substring(0, 1);

                        string currBlock = Grid.Instance.grid[x, y].name.Substring(0, 1);
                        if (prevBlock != null && !prevBlock.Equals(currBlock))
                        {
                            if (matchingBlocks.Count < 3)
                            {
                                matchingBlocks.Clear();
                                matchingBlocks.Push(x);
                            }
                        }
                        else
                            matchingBlocks.Push(x);
                    }
                    else if(matchingBlocks.Count < 3)
		 				matchingBlocks.Clear();		
		 		}
		 		else
					matchingBlocks.Push(x);
		 	}
		 	
			if(matchingBlocks.Count > 2)
				for(int i = matchingBlocks.Count; i > 0; i--)
					Grid.Instance.DestroyBlock((int)matchingBlocks.Pop(), y);
			else
				matchingBlocks.Clear();
		}
	
	
		ArrayList matchingCol = new ArrayList();
		int prevCount = 1;
			
		for(int y = 0; y < Grid.Instance.height; y++) {
			for(int x = 0; x < Grid.Instance.width; x++) {
				if(Grid.Instance.grid[x, y] != null) {
					Transform currBlock = Grid.Instance.grid[x, y];
                    Transform prevBlock = Grid.Instance.grid[0, 0];


                    if (prevBlock != null)
						if (!currBlock.name.Substring(0, 1).Equals(prevBlock.name.Substring(0, 1)))
                        {
							if (prevCount > 2)
								for (int i = 0; i < prevCount; i++)
									matchingCol.Add(x - i - 1);	
							
							prevCount = 1;
						}
						else
							prevCount++;
						
					prevBlock = currBlock;
				}
				else {
					if (prevCount > 2)
						for (int i = 0; i < prevCount; i++)
							matchingCol.Add(x - i - 1);	

					prevBlock = null;
					prevCount = 1;
				}
			}

			if (prevCount > 2)
				for (int i = 0; i < prevCount; i++)
					matchingCol.Add(Grid.Instance.width - i - 1);	

			foreach(int x in matchingCol)
				Grid.Instance.DestroyBlock(x, y);
				
			matchingCol = new ArrayList();
			prevBlock = null;
			prevCount = 1;
		}
    }

    private void MatchColumn() {
        ArrayList matchingCol = new ArrayList();
        GameObject prevBlock = null;
        int prevCount = 1;

		for(int x = 0; x < Grid.Instance.width; x++)
        {
			for(int y = 0; y < Grid.Instance.height; y++)
            {
                if (Grid.Instance.grid[x, y] == null)
                {
					if(prevCount > 2)
						for (int i = 0; i < prevCount; i++)
							matchingCol.Add(y - i - 1);	
					break;

				}

				GameObject currBlock = Grid.Instance.grid[x, y].gameObject;

				if(prevBlock != null)
	            	if (!currBlock.name.Substring(0, 1).Equals(prevBlock.name.Substring(0,1)))
                    {
						if (prevCount > 2)
							for (int i = 0; i < prevCount; i++)
								matchingCol.Add(y - i - 1);	

						prevCount = 1;
					}
	                else
	                	prevCount++;



                    prevBlock = currBlock;
                }

            foreach(int y in matchingCol)
				Grid.Instance.DestroyBlock(x, y);
			
			matchingCol = new ArrayList();
            prevBlock = null;
            prevCount = 1;
		}


	}
    */

    public void PushUp()
    {
        for (int y = height - 1; y > 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = data[y - 1, x];
                cubes[y, x] = cubes[y - 1, x];

                if (cubes[y, x])
                    cubes[y, x].transform.position = GetPosition(y, x);
            }
        }
    }

    public void GenerateRow(int row)
    {
        for (int x = 0; x < width; x++)
        {
            data[row, x] = GenerateBlock(row, x, -1);
            cubes[row, x] = Instantiate(blocks[data[row, x]], GetPosition(row, x), Quaternion.identity);
        }

        LoopMatch();
    }

    public int GenerateBlock(int y, int x, int avoidIndex)
    {
        int i = -1;
        do
        {
            i = Random.Range(0, blocks.Length);
        }
        while (i == -1 || i == avoidIndex);

        return i;
    }

    private Vector3 GetPosition(int y, int x)
    {
        return new Vector3(startAnchor.position.x + (x * 1f), startAnchor.position.y + (y * 1f), 0f);
    }


    public T Choose<T>(T a, T b, params T[] p)
    {
        int random = Random.Range(0, p.Length + 2);
        if (random == 0) return a;
        if (random == 1) return b;
        return p[random - 2];
    }


    private void GarbageBlocks(List<Vector2Int> matchs, int combo)
    {
        Vector2 Pos6 = new Vector2(0, 10);
        Vector2 Pos12 = new Vector2(0, 9.5f);
        Vector2 Pos18 = new Vector2(0, 9);
        Vector2 Pos24 = new Vector2(0, 8.5f);
        Vector2 Pos30 = new Vector2(0, 8);
        Vector2 RandPos5 = new Vector2(Choose<float>(-0.5f, 0.5f), 10);
        Vector2 RandPos4 = new Vector2(Choose<int>(-1, 1), 10);
        Vector2 RandPos3 = new Vector2(Choose<float>(-1.5f, 1.5f), 10);
        Vector2 RandPos2 = new Vector2(Choose<int>(-2, 2), 10);
        int GarbageBlockCount = matchs.Count * combo;
        print(GarbageBlockCount);
        while (GarbageBlockCount > 0) {
            if (GarbageBlockCount > 30)
            {
                GarbageBlockCount -= 30;
                Instantiate(GarbageBlock[0], Pos30, Quaternion.identity);
            }
            if (GarbageBlockCount > 24)
            {
                GarbageBlockCount -= 24;
                Instantiate(GarbageBlock[1], Pos24, Quaternion.identity);
            }
            if (GarbageBlockCount > 18)
            {
                GarbageBlockCount -= 18;
                Instantiate(GarbageBlock[2], Pos18, Quaternion.identity);
            }
            if (GarbageBlockCount > 12)
            {
                GarbageBlockCount -= 12;
                Instantiate(GarbageBlock[3], Pos12, Quaternion.identity);
            }
            if (GarbageBlockCount > 6)
            {
                GarbageBlockCount -= 6;
                Instantiate(GarbageBlock[4], Pos6, Quaternion.identity);
            }
            if (GarbageBlockCount > 5)
            {
                 GarbageBlockCount -= 5;
                Instantiate(GarbageBlock[5], RandPos5, Quaternion.identity);
            }
            if (GarbageBlockCount > 4)
            {
                GarbageBlockCount -= 4;
                Instantiate(GarbageBlock[6], RandPos4, Quaternion.identity);
            }
            if (GarbageBlockCount > 3)
            {
                GarbageBlockCount -= 3;
                Instantiate(GarbageBlock[7], RandPos3, Quaternion.identity);
            }
            else if (GarbageBlockCount > 2)
            {
                GarbageBlockCount -= 2;
                Instantiate(GarbageBlock[8], RandPos2, Quaternion.identity);
            }
            FallDown();
        }
        
    }
}
