using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenLevelPart : MonoBehaviour
{
    public enum PartType
    {
        Room,
        Hallway
    }

    [SerializeField] private LayerMask roomsLayerMask;
    [SerializeField] private PartType partType;
    [SerializeField] private GameObject fillerWall;
    public List<Transform> entrypoints;
    public new Collider collider;

    public bool HasAvailableEntrypoint(out Transform entrypoint)
    {
        Transform resultingEntry = null;
        bool result = false;

        int totalRetries = 100;
        int retryIndex = 0;

        if (entrypoints.Count == 1)
        {
            Transform entry = entrypoints[0];
            if (entry.TryGetComponent<EntryPoint>(out EntryPoint res))
            {
                if (res.IsOccupied())
                {
                    result = false;
                    resultingEntry = null;
                }
                else
                {
                    result = true;
                    resultingEntry = entry;
                    res.SetOccupied();
                }

                entrypoint = resultingEntry;
                return result;
            }
        }

        while (resultingEntry == null && retryIndex < totalRetries)
        {
            int randomEntryIndex = Random.Range(0, entrypoints.Count);
            Transform entry = entrypoints[randomEntryIndex];

            if (entry.TryGetComponent<EntryPoint>(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    resultingEntry = entry;
                    result = true;
                    entryPoint.SetOccupied();
                    break;
                }
            }

            retryIndex++;
        }

        entrypoint = resultingEntry;
        return result;
    }

    public void UnuseEntrypoint(Transform entrypoint)
    {
        if (entrypoint.TryGetComponent(out EntryPoint entry))
        {
            entry.SetOccupied(false);
        }
    }

    public void FillEmptyDoors()
    {
        entrypoints.ForEach((entry) =>
        {
            if (entry.TryGetComponent(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    GameObject wall = Instantiate(fillerWall);
                    //wall.GetComponent<NetworkObject>().Spawn(true);
                    wall.transform.position = entry.transform.position;
                    wall.transform.rotation = entry.transform.rotation;
                }
            }
        });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }

   /* public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }*/
}