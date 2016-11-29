using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Chamelerun.Serialization
{
    public class LevelSegmentSerializer : EditorWindow
    {
        private string pathName = "Assets/Resources/LevelSegments/";

        [MenuItem("Window/LevelSegmentSerializer")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(LevelSegmentSerializer));
        }

        public void OnGUI()
        {
            pathName = EditorGUILayout.TextField("Path", pathName);

            if (GUILayout.Button("Serialize Selected"))
            {
                if (Selection.activeGameObject != null)
                {
                    SerializableLevelSegment levelSegment = Selection.activeGameObject.GetComponent<SerializableLevelSegment>();
                    if (levelSegment != null)
                    {
                        SerializeLevelSegment(levelSegment);
                    }
                    else
                    {
                        Debug.LogWarning("selected object is not a level segment");
                    }
                }
            }
            if (GUILayout.Button("Serialize All"))
            {
                SerializableLevelSegment[] allLevelSegments = GameObject.FindObjectsOfType<SerializableLevelSegment>();
                foreach (SerializableLevelSegment levelSegment in allLevelSegments)
                {
                    SerializeLevelSegment(levelSegment);
                }
            }
        }

        private void SerializeLevelSegment(SerializableLevelSegment levelSegment)
        {
            Type[] extraTypes = { typeof(Powerup), typeof(Hazard), typeof(Enemy) };
            XmlSerializer serializer = new XmlSerializer(typeof(LevelSegment), extraTypes);
            Stream fs = new FileStream(pathName + levelSegment.name + ".xml", FileMode.Create);
            XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
            serializer.Serialize(writer, levelSegment.GetSerializableObject());
            writer.Close();
            Debug.Log("Sucessfully serialized " + levelSegment.name);
        }
    }
}