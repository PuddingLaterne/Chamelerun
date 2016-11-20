using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Chamelerun.Serialization
{
    public class LevelSegmentLoader
    {
        public List<LevelSegment> LoadLevelSegments(string resourceDirectory)
        {
            List<LevelSegment> levelSegments = new List<LevelSegment>();

            Type[] extraTypes = { typeof(Powerup), typeof(Hazard), typeof(Enemy) };
            XmlSerializer serializer = new XmlSerializer(typeof(LevelSegment), extraTypes);

            MemoryStream memoryStream = null;
            StreamReader streamReader = null;
            StringReader stringReader = null;
            XmlReader xmlReader = null;

            UnityEngine.Object[] assets = Resources.LoadAll(resourceDirectory);
            foreach (UnityEngine.Object asset in assets)
            {
                LevelSegment levelSegment = null;
                try
                {
                    TextAsset textAsset = (TextAsset)asset;

                    memoryStream = new MemoryStream(textAsset.bytes);
                    streamReader = new StreamReader(memoryStream, true);
                    string text = streamReader.ReadToEnd();

                    stringReader = new System.IO.StringReader(text);
                    xmlReader = System.Xml.XmlReader.Create(stringReader);

                    levelSegment = (LevelSegment)serializer.Deserialize(xmlReader);
                }
                catch (InvalidCastException exception)
                {
                    Debug.LogWarning("error while loading " + asset.name + ": not a serialized level segment\n" + exception.Message);
                }
                catch (XmlException exception)
                {
                    Debug.LogWarning("error while loading " + asset.name + ": could not be deserialized\n" + exception.Message);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("error while loading " + asset.name + ": " + exception.Message);
                }
                finally
                {
                    if (memoryStream != null)
                    {
                        memoryStream.Close();
                        memoryStream = null;
                    }
                    if (streamReader != null)
                    {
                        streamReader.Close();
                        streamReader = null;
                    }
                    if (stringReader != null)
                    {
                        stringReader.Close();
                        stringReader = null;
                    }
                    if (xmlReader != null)
                    {
                        xmlReader.Close();
                        xmlReader = null;
                    }
                }
                levelSegments.Add(levelSegment);
            }
            return levelSegments;
        }
    }
}