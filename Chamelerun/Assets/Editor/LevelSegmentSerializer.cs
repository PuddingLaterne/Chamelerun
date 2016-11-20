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

            if (GUILayout.Button("Serialize"))
            {
                if (Selection.activeGameObject != null)
                {
                    SerializableLevelSegment levelSegment = Selection.activeGameObject.GetComponent<SerializableLevelSegment>();
                    if (levelSegment != null)
                    {
                        string fileName = pathName + levelSegment.name + ".xml";
                        SerializeLevelSegment(fileName, levelSegment);
                    }
                    else
                    {
                        Debug.LogWarning("selected object is not a level segment");
                    }
                }
            }
        }

        public void SerializeLevelSegment(string fileName, SerializableLevelSegment levelSegment)
        {
            Type[] extraTypes = { typeof(Powerup), typeof(Hazard), typeof(Enemy) };
            XmlSerializer serializer = new XmlSerializer(typeof(LevelSegment), extraTypes);
            Stream fs = new FileStream(fileName, FileMode.Create);
            XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
            serializer.Serialize(writer, levelSegment.GetSerializableObject());
            writer.Close();
        }
    }
}