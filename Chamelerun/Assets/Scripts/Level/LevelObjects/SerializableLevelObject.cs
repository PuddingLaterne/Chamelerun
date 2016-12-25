﻿using UnityEngine;
using System.Collections;

namespace Chamelerun.Serialization
{
    public class SerializableLevelObject : MonoBehaviour
    {
        public string ID;
        public bool IsOptional;

        public virtual LevelObject GetSerializableObject()
        {
            return new LevelObject(ID, IsOptional, transform.localPosition, transform.eulerAngles.z, transform.localScale);
        }
    }

    public class LevelObject
    {
        public Vector2 Position { get; protected set; }
        public float Rotation { get; protected set; }
        public Vector2 Scale { get; protected set; }
        public string ID { get; protected set; }
        public bool IsOptional { get; protected set; }

        public LevelObject() { }

        public LevelObject(string ID, bool isOptional, Vector2 position, float rotation, Vector2 scale)
        {
            this.ID = ID;
            IsOptional = isOptional;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public virtual void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.LevelObjectSpawner.SpawnLevelObject(ID, Position + positionOffset, Rotation, Scale);
        }
    }
}