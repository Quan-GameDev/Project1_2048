using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;
    private TileGrid grid;
    private List<Tile> tiles;
    private List<Tile> tempTile = new List<Tile>(16);

    private bool waiting;

    public int a;
    public int b;

    static int loadCount = 0;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile(){
        Tile tile;
        if (tempTile.Count==0){
            tile = Instantiate(tilePrefab, grid.transform);
            tile.SetState(tileStates[(int)Math.Log(2,2)-1], 2);
            tile.Spawn(grid.GetRandomEmptyCell()); 
            tiles.Add(tile);
            return;
        }
        else{
            tile = tempTile[0];
            // tile.transform.position
            tempTile.Remove(tile);
            tile.enabled = true;
            tile.SetState(tileStates[(int)Math.Log(2,2)-1], 2);
            TileCell tileCellTemp = grid.GetRandomEmptyCell();
            tile.Spawn(tileCellTemp); 
            tiles.Add(tile);
        }
        // else{
        //     tile = Instantiate(tilePrefab, grid.transform);
        // }
             
    }

    public void CreateTile(int number, TileCell tileCell){
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[(int)Math.Log(number,2)-1], number);
        tile.Spawn(tileCell); 
        tiles.Add(tile); 
           
    }

    public void CreateTile(int number, Tile tile){
        // Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[(int)Math.Log(number,2)-1], number);
        tile.Spawn(grid.GetRandomEmptyCell()); 
        tiles.Add(tile);
    }

    public TileGrid getGrid(){
        return this.grid;
    }
    
    public void Update()
    {
      if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                Move(Vector2Int.up, 0, 1, 1, 1);
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                Move(Vector2Int.left, 1, 1, 0, 1);
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                Move(Vector2Int.down, 0, 1, grid.height - 2, -1);
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                Move(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
            PlayerPrefs.Save();
        }
        
    }

    public void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.occupied) {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed) {
            StartCoroutine(WaitForChanges());
        }   
        // print(); 
        Debug.Log(tempTile.Count);    
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
        
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.locked && b.number != 4096;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        a.enabled = false;
        tempTile.Add(a);
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;

        b.SetState(tileStates[index], number);

        // a.SetState(tileStates[0], 2);

        gameManager.IncreaseScore(number);
        CheckForContinue(number);
        CheckForWin(number);

    }

    private void print(){
        string res = "";
        foreach (var tile in tiles){
            res+=tile.number.ToString()+" ";
        }
        Debug.Log(res);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        foreach (var tile in tiles) {
            tile.locked = false;
        }

        if (tiles.Count != grid.size) {
            CreateTile();
        }

        if (CheckForGameOver(b)) {
            gameManager.GameOver();
        }

        if(CheckForContinue(a)){
            gameManager.Continue();
        }

        if(CheckForWin(b)){
            if (checkWinable()) gameManager.Win();
            else gameManager.GameOver();
        }
    }

    

    public bool CheckForGameOver(int b)
    {
        if (b != 2048 || tiles.Count != grid.size) {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
    }

    public bool CheckForContinue(int a){
        
        this.a = a;
        if(a != 2048 || loadCount > 1){
            return false;
            
        }
        loadCount++;
        return true;
        
    }
    
    public bool CheckForWin(int b){
        this.b = b;
        if(tiles.Count != grid.size){
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
        
    }

    public bool checkWinable(){
        int count4096 = 0;
        int count2048 = 0;
        foreach (var tile in tiles){
            if (tile.number==4096){
                count4096++;
            }
            if (tile.number==2048){
                count2048++;
            }
        }
        if (count4096==16 && count2048==0){
            return true;
        }
        return false;
    }

}
