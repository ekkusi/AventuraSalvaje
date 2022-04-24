using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] GameObject tile;

    [SerializeField] uint seed;
    [SerializeField] int chunkSize = 32;
    [SerializeField] float tileSize = 1f;
    [SerializeField] int maxChunks = 3;
    int maxTiles;
    Map map;
    
    void Start() {
        maxTiles = chunkSize * maxChunks;
        map = new Map();
        GenerateMap();
    }
 
    private void GenerateMap() {
        for (int x = 0; x < maxTiles; x++) {
            for (int y = 0; y < maxTiles; y++) {
                GameObject newTile = Instantiate(tile, transform);
                float posX = (x * tileSize + y * tileSize) / 2f;
                float posY = (x * tileSize - y * tileSize) / 4f;
                Biome biome = map.getBiome(x/maxTiles, y/maxTiles);
                
                newTile.transform.position = new Vector2(posX, posY);
                newTile.name = x + " , " + y;
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
