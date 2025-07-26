using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleCity.AI
{
    public class AiDirector : MonoBehaviour
    {
        public PlacementManager placementManager;
        public GameObject[] pedestrianPrefabs;

        AdjacencyGraph graph = new AdjacencyGraph();

        public void SpawnAllAagents()
        {
            var houses = placementManager.GetAllHouses();
            int maxAgents = houses.Count;

            // Підрахунок існуючих агентів на сцені
            int currentAgentsCount = FindObjectsOfType<AiAgent>().Length;

            int agentsToSpawn = maxAgents - currentAgentsCount;
            if (agentsToSpawn <= 0)
                return;

            int spawnedAgents = 0;

            foreach (var house in houses)
            {
                if (spawnedAgents >= agentsToSpawn)
                    break;

                TrySpawningAnAgent(house, placementManager.GetRandomSpecialStrucutre());
                spawnedAgents++;
            }
        }


        private void TrySpawningAnAgent(StructureModel startStructure, StructureModel endStructure)
        {
            if (startStructure == null || endStructure == null)
            {
                Debug.LogWarning("Start or End structure is null!");
                return;
            }

            var startPosition = ((INeedingRoad)startStructure).RoadPosition;
            var endPosition = ((INeedingRoad)endStructure).RoadPosition;

            var startStructureAt = placementManager.GetStructureAt(startPosition);
            var endStructureAt = placementManager.GetStructureAt(endPosition);

            if (startStructureAt == null || endStructureAt == null)
            {
                return;
                Debug.LogWarning("Start or End structure at road position is null!");
                
            }

            var startMarkerPosition = startStructureAt.GetPedestrianSpawnMarker(startStructure.transform.position);
            var endMarkerPosition = endStructureAt.GetNearestMarkerTo(endStructure.transform.position);

            var path = placementManager.GetPathBetween(startPosition, endPosition, true);
            if (path == null || path.Count < 2)
            {
                return;
                Debug.LogWarning($"No valid path found between {startStructure.name} and {endStructure.name}. Path count: {path?.Count ?? 0}");
                
            }

            path.Reverse();
            List<Vector3> agentPath = GetPedestrianPath(path, startMarkerPosition.Position, endMarkerPosition);

            if (agentPath == null || agentPath.Count < 2)
            {
                return;
                Debug.LogWarning("Agent path too short or null. Skipping agent spawn.");
                
            }

            var agent = Instantiate(GetRandomPedestrian(), startMarkerPosition.Position, Quaternion.identity);
            var aiAgent = agent.GetComponent<AiAgent>();
            aiAgent.Initialize(agentPath);
        }


        private List<Vector3> GetPedestrianPath(List<Vector3Int> path, Vector3 startPosition, Vector3 endPosition)
        {
            graph.ClearGraph();
            CreatAGraph(path);
            Debug.Log(graph);
            return AdjacencyGraph.AStarSearch(graph,startPosition,endPosition);
        }

        private void CreatAGraph(List<Vector3Int> path)
        {
            Dictionary<Marker, Vector3> tempDictionary = new Dictionary<Marker, Vector3>();

            for (int i = 0; i < path.Count; i++)
            {
                var currentPosition = path[i];
                var roadStructure = placementManager.GetStructureAt(currentPosition);
                var markersList = roadStructure.GetPedestrianMarkers();
                bool limitDistance = markersList.Count == 4;
                tempDictionary.Clear();
                foreach (var marker in markersList)
                {
                    graph.AddVertex(marker.Position);
                    foreach (var markerNeighbourPosition in marker.GetAdjacentPositions())
                    {
                        graph.AddEdge(marker.Position, markerNeighbourPosition);
                    }

                    if(marker.OpenForconnections && i+1 < path.Count)
                    {
                        var nextRoadStructure = placementManager.GetStructureAt(path[i + 1]);
                        if (limitDistance)
                        {
                            tempDictionary.Add(marker, nextRoadStructure.GetNearestMarkerTo(marker.Position));
                        }
                        else
                        {
                            graph.AddEdge(marker.Position, nextRoadStructure.GetNearestMarkerTo(marker.Position));
                        }
                    }
                }
                if(limitDistance && tempDictionary.Count == 4)
                {
                    var distanceSortedMarkers = tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).ToList();
                    for (int j = 0; j < 2; j++)
                    {
                        graph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                    }
                }
            }
        }

        private GameObject GetRandomPedestrian()
        {
            return pedestrianPrefabs[UnityEngine.Random.Range(0, pedestrianPrefabs.Length)];
        }

        private void Update()
        {
            foreach (var vertex in graph.GetVertices())
            {
                foreach (var vertexNeighbour in graph.GetConnectedVerticesTo(vertex))
                {
                    Debug.DrawLine(vertex.Position + Vector3.up, vertexNeighbour.Position + Vector3.up, Color.red);
                }
            }
        }

        public void NewMethod(int parameter)
        {
            Debug.Log("hello");
        }

        public void NewMethdTow(string name)
        {
            Debug.Log(name);
        }
    }
}

