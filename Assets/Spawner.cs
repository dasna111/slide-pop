using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public static Spawner Instance;
    public GameObject[] blocks;

    public Transform[,] grid;
    public ArrayList occupiedPos = new ArrayList();
    private const float startPos = -2.5f;

    public int[,] data = null;

    public int width = 6;
    public int height = 11;
    [SerializeField] private int startHeight = 6;

    //private const float startPos = -2.5f;

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

        for (int y = 0; y < startHeight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = GenerateBlock(x, y, previous);
                previous = data[y, x];
                transform.position += new Vector3(1.0f, 0, 0);
            }
            transform.position = new Vector3(startPos, transform.position.y + 1, transform.position.z);
        }

        for (int y = startHeight; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = -1;
            }
        }

        Match();
    }



    // Update is called once per frame
    void Update()
    {
        SpawnerTimer = SpawnerTimer + Time.deltaTime;
        if (SpawnerTimer >= SpawnEvery || Input.GetKeyDown(KeyCode.Z))
        {
            SpawnNewLine();
            SpawnerTimer = SpawnerTimer - SpawnEvery;

        }
        /* if (Input.GetKeyDown(KeyCode.Space))
         {
             SwapBlocks(CursorControls.leftX, CursorControls.rightX, CursorControls.cursorY);
         }
         */
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
        int combo = 0;
        RemoveMatchs(matchs);
        FallDown();
        if(combo > 1)
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
                    for (int z = y - 1; z < height - 1; z++)
                        data[z, x] = data[z + 1, x];

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
    }
    #endregion


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

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y > 0; y--)
            {
                data[y, x] = data[y - 1, x];
            }
        }
    }

    public void GenerateRow(int row)
    {
        transform.position = new Vector3(startPos, 0, 0);

        for (int x = 0; x < width; x++)
        {
            data[row, x] = GenerateBlock(x, row, -1);

            transform.position += new Vector3(1.0f, 0, 0);
        }
        Match();
        transform.position = new Vector3(startPos, transform.position.y + 1, transform.position.z);
    }

    public int GenerateBlock(int x, int y, int avoidIndex)
    {
        int i = -1;
        do
        {
            i = UnityEngine.Random.Range(0, blocks.Length);
            Instantiate(blocks[i], transform.position, Quaternion.identity);
        }
        while (i == -1 || i == avoidIndex);

        return i;
    }
    private void GarbageBlocks(List<Vector2Int> matchs, int combo)
    {
        int GarbageBlockCount = matchs.Count * combo;

        if(GarbageBlockCount > 24)
        {

        }
        else if(GarbageBlockCount > 18)
        {

        }
        else if (GarbageBlockCount > 12)
        {

        }
        else if (GarbageBlockCount > 6)
        {

        }
        else if (GarbageBlockCount > 0)
        {

        }
    }

    /*public void SwapBlocks(int leftX, int rightX, int cursorY)
    {
        Transform leftBlock = grid[leftX, cursorY],
                  rightBlock = grid[rightX, cursorY];

        if (leftBlock == null && rightBlock == null)
        {
            Match();
            return;
        }
        Vector3 leftPos = new Vector3((float)leftX - 2.5f, (float)cursorY, 0),
                rightPos = new Vector3((float)rightX - 2.5f, (float)cursorY, 0);

        if (leftBlock == null)
        {
            grid[leftX, cursorY] = rightBlock;
            rightBlock.position = leftPos;
            grid[rightX, cursorY] = null;
        }
        else if (rightBlock == null)
        {
            grid[rightX, cursorY] = leftBlock;
            leftBlock.position = rightPos;
            grid[leftX, cursorY] = null;
        }
        else
        {
            grid[leftX, cursorY].position = rightPos;
            grid[rightX, cursorY].position = leftPos;
            grid[leftX, cursorY] = rightBlock;
            grid[rightX, cursorY] = leftBlock;
        }

        Match();
    }*/
}
