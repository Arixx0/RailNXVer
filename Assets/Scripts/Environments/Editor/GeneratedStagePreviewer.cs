using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Environments
{
    public class GeneratedStagePreviewer : EditorWindow
    {
        private static Rect ContentRect;

        private Dictionary<Room, int> m_RoomsLUT;

        private List<Vector2> m_ElementPositions;

        private StageGenerationSettings m_Settings;
        
        public static void ShowPreview(List<Room> rooms, StageGenerationSettings settings)
        {
            var window = GetWindow<GeneratedStagePreviewer>(true, "Generated Stage Previewer");
            window.m_RoomsLUT = rooms.Select((room, id) => (room, id)).ToDictionary(p => p.room, p => p.id);
            window.m_Settings = settings;
            
            window.CalculateLayout();
            window.CalculateElementPositions();

            var firstRoom = rooms.First(r => r.roomEventType == RoomEventType.InitialRoom);
            var searchTree = new List<int>(settings.mapHeight);
            var logBuilder = new StringBuilder();
            window.TraversePassage(firstRoom, searchTree, logBuilder);
            Debug.Log(logBuilder.ToString());
            
            window.ShowUtility();
        }

        private void OnGUI()
        {
            if (m_RoomsLUT == null || m_Settings == null)
            {
                Close();
                return;
            }
            
            Handles.BeginGUI();

            foreach (var e in m_RoomsLUT)
            {
                var elementRect = new Rect(m_ElementPositions[e.Value], ContentRect.size);

                var contentRect = new Rect(elementRect.x + 2f, elementRect.y + 2f,
                    elementRect.width - 4f, elementRect.height - 4f);
                var iconRect = new Rect(contentRect.x, contentRect.y,
                    EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                var labelRect = new Rect(iconRect.xMax + 2f, contentRect.y,
                    contentRect.width - iconRect.width - 2f, contentRect.height);

                for (var i = 0; i < e.Key.nextRoomCount; i += 1)
                {
                    Handles.DrawLine(
                        m_ElementPositions[e.Value], m_ElementPositions[m_RoomsLUT[e.Key.nextRooms[i]]]);
                }

                EditorGUI.DrawRect(elementRect, Color.gray);
                EditorGUI.DrawRect(iconRect, Color.white);
                EditorGUI.LabelField(labelRect, $"({e.Key.x},{e.Key.y})\n{e.Key.roomEventType}");
            }
            
            Handles.EndGUI();
        }

        private void CalculateLayout()
        {
            ContentRect = new Rect(0, 0, 86 + EditorGUIUtility.singleLineHeight, 4 + EditorGUIUtility.singleLineHeight * 2);
            minSize = new Vector2(60 + ContentRect.width * m_Settings.mapWidth, 80 + ContentRect.height * m_Settings.mapHeight);
        }

        private void CalculateElementPositions()
        {
            m_ElementPositions = new List<Vector2>(m_RoomsLUT.Count);

            var previewRect = new Rect(10, 10, minSize.x - 20, minSize.y - 20);
            var offsetPos = new Vector2(previewRect.x + 40f, previewRect.yMax - 40f - ContentRect.height);

            foreach (var e in m_RoomsLUT.Keys)
            {
                m_ElementPositions.Add(new Vector2(
                        offsetPos.x + ContentRect.width * e.x, offsetPos.y - ContentRect.height * e.y));
            }
        }

        private void TraversePassage(Room room, List<int> searchTree, StringBuilder logBuilder)
        {
            searchTree.Add(room.x);

            if (room.roomEventType == RoomEventType.BossWave)
            {
                logBuilder.AppendLine(string.Join(" -> ", searchTree));
                searchTree.RemoveAt(searchTree.Count - 1);
                return;
            }

            for (var i = 0; i < room.nextRoomCount; i += 1)
            {
                TraversePassage(room.nextRooms[i], searchTree, logBuilder);
            }
            
            searchTree.RemoveAt(searchTree.Count - 1);
        }
    }
}