using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Biome
{
  Forest = 1,
  Desert = 2,
  RedLand = 3,
  DeadForest = 4,
  Snow = 5,

}
public class MapGenerator : MonoBehaviour
{
  [SerializeField] private GameObject tile;

  [SerializeField] private uint seed;
  [SerializeField] private int chunkSize = 32;
  [SerializeField] private int maxChunks = 10000000;
  [SerializeField] private GameObject map;
  [SerializeField] private Tilemap tileMap;
  [SerializeField] private GameObject player;
  public int maxTiles { get => chunkSize * maxChunks; }

  private IDictionary<Biome, Tile> resourceDict = new Dictionary<Biome, Tile>();

  private List<Vector3Int> renderedChunks = new List<Vector3Int>();
  private List<Vector3Int> renderedTiles = new List<Vector3Int>();

  private int chunkX = 0;
  private int chunkY = 0;

  //Offsetilla määritellään että satunnaisuus uniikkia eri layereilla, eli offset pitää olla eri.
  // Scalella määritellään kuvion tiheys.
  float humidityOffsetX;
  float humidityOffsetY;
  float temperatureOffsetX;
  float temperatureOffsetY;

  private float humidityScale = 5000000f;
  private float temperatureScale = 2000000f;

  void Update()
  {
    float playerIsoX = (float)player.transform.position.y * 2 + player.transform.position.x;
    float playerIsoY = (float)player.transform.position.y * 2 - player.transform.position.x;
    int playerChunkX = (int)Math.Round(playerIsoX / chunkSize);
    int playerChunkY = (int)Math.Round(playerIsoY / chunkSize);
    if (playerChunkX != chunkX || playerChunkY != chunkY)
    {
      chunkX = playerChunkX;
      chunkY = playerChunkY;
      LoadChunks();
      RemoveUnneededChunks();
      foreach (Vector3Int chunk in renderedChunks)
      {
        Debug.Log(chunk.x + " , " + chunk.y);
      }
      Debug.Log("pos: " + chunkX + " , " + chunkY);
    }
  }
  void Start()
  {
    LoadResources();
    LoadChunks();
    SetupConfiguration();
  }
  void SetupConfiguration()
  {
    // TODO: Set seed to random to control the randomness and add random values as offset.
    humidityOffsetX = 0f;
    humidityOffsetY = 0f;
    temperatureOffsetX = 1000f;
    temperatureOffsetY = 1000f;

  }

  public Biome GetBiome(float x, float y)
  {
    float humidity = Mathf.PerlinNoise(x * humidityScale + humidityOffsetX, y * humidityScale + humidityOffsetY);
    float temperature = Mathf.PerlinNoise(x * temperatureScale + temperatureOffsetX, y * temperatureScale + temperatureOffsetY);
    Biome biome = Biome.Forest;
    if (temperature > 0.6)
    {
      if (humidity > 0.6)
      {
        biome = Biome.Desert;
      }
      else
      {
        biome = Biome.RedLand;
      }
    }
    else if (temperature < 0.35)
    {
      if (humidity > 0.5)
      {
        biome = Biome.DeadForest;
      }
      else
      {
        biome = Biome.Snow;
      }
    }
    return biome;
  }

  public void LoadResources()
  {
    resourceDict.Add(new KeyValuePair<Biome, Tile>(Biome.RedLand, Resources.Load<Tile>("red_land")));
    resourceDict.Add(new KeyValuePair<Biome, Tile>(Biome.DeadForest, Resources.Load<Tile>("dead_grass")));
    resourceDict.Add(new KeyValuePair<Biome, Tile>(Biome.Snow, Resources.Load<Tile>("snow")));
    resourceDict.Add(new KeyValuePair<Biome, Tile>(Biome.Forest, Resources.Load<Tile>("grass")));
    resourceDict.Add(new KeyValuePair<Biome, Tile>(Biome.Desert, Resources.Load<Tile>("sand")));
  }

  private void LoadChunks()
  {
    for (int x = chunkX - 2; x < chunkX + 3; x++)
    {
      for (int y = chunkY - 2; y < chunkY + 3; y++)
      {
        Vector3Int chunk = new Vector3Int(x, y);
        if (!renderedChunks.Contains(chunk))
        {
          renderedChunks.Add(chunk);
          RenderChunk(chunk);
        }
      }
    }
  }

  private void RenderChunk(Vector3Int chunk)
  {
    for (int x = -chunkSize / 2 + chunk.x * chunkSize; x < chunkSize / 2 + chunk.x * chunkSize; x++)
    {
      for (int y = -chunkSize / 2 + chunk.y * chunkSize; y < chunkSize / 2 + chunk.y * chunkSize; y++)
      {
        Biome biome = GetBiome((float)x / (float)maxTiles, (float)y / (float)maxTiles);
        Tile newTile = resourceDict[biome];
        Vector3Int pos = new Vector3Int(x, y);
        tileMap.SetTile(pos, newTile);
      }
    }
  }

  private void RemoveUnneededChunks()
  {
    List<Vector3Int> filtered = renderedChunks.FindAll(pos => pos.x < chunkX - 2 || pos.x > chunkX + 2 || pos.y < chunkY - 2 || pos.y > chunkY + 2);
    foreach (Vector3Int chunk in filtered)
    {
      renderedChunks.Remove(chunk);
      RemoveChunkTiles(chunk);
    }
  }

  private void RemoveChunkTiles(Vector3Int chunk)
  {
    for (int x = -chunkSize / 2 + chunk.x * chunkSize; x < chunkSize / 2 + chunk.x * chunkSize; x++)
    {
      for (int y = -chunkSize / 2 + chunk.y * chunkSize; y < chunkSize / 2 + chunk.y * chunkSize; y++)
      {
        Vector3Int pos = new Vector3Int(x, y);
        tileMap.SetTile(pos, null);
      }
    }
  }

  public void GenerateMap()
  {
    ClearMap();
    LoadResources();
    Debug.Log("Generating map of " + maxTiles + " tiles");
    for (int x = 0; x < maxTiles; x++)
    {
      for (int y = 0; y < maxTiles; y++)
      {
        // ### Map generation with gameobjects as tiles ###
        // GameObject newTile = Instantiate(tile);
        // newTile.transform.SetParent(map.transform);
        // float posX = (x * tileSize + y * tileSize) / 2f;
        // float posY = (x * tileSize - y * tileSize) / 4f;
        // newTile.transform.position = new Vector2(posX, posY);
        // newTile.name = x + " , " + y;
        Biome biome = GetBiome((float)x / (float)maxTiles, (float)y / (float)maxTiles);
        Tile newTile = resourceDict[biome];
        // ### Map generation with tilemap ###
        Vector3Int pos = new Vector3Int(x, y);
        tileMap.SetTile(pos, newTile);
      }
    }
  }

  public void ClearMap()
  {
    Debug.Log("Deleting " + maxTiles + " tiles");
    // ### Clearing for tiles as gameobjects ###
    // int tileCount = map.transform.childCount;
    // for (int i = tileCount - 1; i >= 0; i--) {
    //   GameObject childObj = map.transform.GetChild(i).gameObject;
    //   // DestroyImmediate to work with executing in editor with "Generate map" and "Clear map" buttons
    //   DestroyImmediate(childObj);
    // }
    // ### Clearing for tilemap ### 
    for (int x = -maxTiles; x < maxTiles; x++)
    {
      for (int y = -maxTiles; y < maxTiles; y++)
      {
        Vector3Int pos = new Vector3Int(x, y);
        tileMap.SetTile(pos, null);
      }
    }
    resourceDict.Clear();
  }
}
