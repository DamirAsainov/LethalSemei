using System;
using System.Collections.Generic;
using ItemSystem;
using UnityEditor;
using UnityEngine;

namespace Transports
{
    public class TransportInside: MonoBehaviour
    {
        private List<GameObject> playersInside = new List<GameObject>();
        private List<GameObject> itemsInside = new List<GameObject>();

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player")
            {
                playersInside.Add(col.gameObject);
            }

            Item item = col.gameObject.GetComponent<Item>();
            if (item != null)
            {
                itemsInside.Add(item.gameObject);
            }

        }

        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.tag == "Player")
            {
                playersInside.Remove(col.gameObject);
            }
            Item item = col.gameObject.GetComponent<Item>();
            if (item != null)
            {
                itemsInside.Add(item.gameObject);
            }
        }

        public List<GameObject> GetPlayersInside()
        {
            return playersInside;
        }

        public List<GameObject> GetItemsInside()
        {
            return itemsInside;
        }
    }
}