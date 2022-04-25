using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Biome {
    Forest = 1,
    Desert = 2,
    RedLand = 3,
    DeadForest = 4,
    Snow = 5,

}
//Function to get random number
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tile;

    [SerializeField] private uint seed;
    [SerializeField] private int chunkSize = 32;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private int maxChunks = 3;
    [SerializeField] private GameObject map;
    [SerializeField] private Tile tileNew;
    [SerializeField] private Tilemap tileMap;
    public int maxTiles { get => chunkSize * maxChunks; }
    private Map mapManager = new Map();
    
    void Start() {
        GenerateMap();
        Tile tileType = Resources.Load<Tile>("redtile");
    }
 
    public void GenerateMap() {
        ClearMap();
        Debug.Log("Generating map of " + maxTiles + " tiles");
        for (int x = 0; x < maxTiles; x++) {
            for (int y = 0; y < maxTiles; y++) {
                // ### Map generation with gameobjects as tiles ###
                // GameObject newTile = Instantiate(tile);
                // newTile.transform.SetParent(map.transform);
                // float posX = (x * tileSize + y * tileSize) / 2f;
                // float posY = (x * tileSize - y * tileSize) / 4f;
                // newTile.transform.position = new Vector2(posX, posY);
                // newTile.name = x + " , " + y;
                Biome biome = mapManager.getBiome(x/maxTiles, y/maxTiles);

                // ### Map generation with tilemap ###
                Vector3Int pos = new Vector3Int(x, y);
                tileMap.SetTile(pos, tileNew);
            }
        }
    }
    
    public void ClearMap() {
      Debug.Log("Deleting " + maxTiles + " tiles");
      // ### Clearing for tiles as gameobjects ###
      // int tileCount = map.transform.childCount;
      // for (int i = tileCount - 1; i >= 0; i--) {
      //   GameObject childObj = map.transform.GetChild(i).gameObject;
      //   // DestroyImmediate to work with executing in editor with "Generate map" and "Clear map" buttons
      //   DestroyImmediate(childObj);
      // }
      // ### Clearing for tilemap ### 
      for (int x = 0; x < maxTiles; x++) {
        for (int y = 0; y < maxTiles; y++) {
            Vector3Int pos = new Vector3Int(x, y);
            tileMap.SetTile(pos, null);
        }
      }
    }
}

public class Map {
    //Offsetilla määritellään että satunnaisuus uniikkia eri layereilla, eli offset pitää olla eri.
    // Scalella määritellään kuvion tiheys.
    int humidityOffsetX;
    int humidityOffsetY;
    int temperatureOffsetX;
    int temperatureOffsetY;

    private float humidityScale = 20f;
    private float temperatureScale = 20f;

    // TODO: Add seed as parameter to construct the map.     
    public Map() {
        // TODO: Set seed to random to control the randomness and add random values as offset.
        humidityOffsetX = 0;
        humidityOffsetY = 0;
        temperatureOffsetX = 10000;
        temperatureOffsetY = 10000;
    }

    public Biome getBiome(int x, int y) {
        float humidity = Mathf.PerlinNoise(x * humidityScale + humidityOffsetX, y * humidityScale + humidityOffsetY);
        float temperature = Mathf.PerlinNoise(x * temperatureScale + temperatureOffsetX, y * temperatureScale + temperatureOffsetY);
        Biome biome = Biome.Forest;
        if (temperature > 0.3) {
            if (humidity > 0.3) {
                biome = Biome.Desert;
            }
            else {
                biome = Biome.RedLand;
            }
        }
        else if (temperature < -0.3) {
            if (humidity > 0) {
                biome = Biome.DeadForest;
            } else {
                biome = Biome.Snow;
            }
        }
        return biome;
    }
}

public class Chunk {
    private int chunkX;
    private int chunkY;

    public Chunk(int x, int y) {
        chunkX = x;
        chunkY = y;
    }


}
