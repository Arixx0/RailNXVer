using System.Collections.Generic;
using Attributes;
using UnityEngine;

namespace Environments
{
    public class Stage : MonoBehaviour
    {
        [SerializeField]
        private RailPath railPath;

        [SerializeField, Disabled]
        private List<Room> roomData;

        [SerializeField, Disabled]
        private List<MapSector> mapSectors;
        
        public RailPath RailPath => railPath;
        
        public List<Room> RoomData => roomData;

        public void CacheRoomData(List<Room> rooms)
        {
            roomData = rooms;
        }
    }
}