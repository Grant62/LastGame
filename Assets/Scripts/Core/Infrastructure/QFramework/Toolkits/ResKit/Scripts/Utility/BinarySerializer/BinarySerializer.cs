/****************************************************************************
 * Copyright (c) 2018 ~ 2022 liangxie
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace QFramework
{
    public class BinarySerializer : IBinarySerializer
    {
        public bool SerializeBinary(string path, object obj)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("SerializeBinary Without Valid Path.");
                return false;
            }

            if (obj == null)
            {
                Debug.LogWarning("SerializeBinary obj is Null.");
                return false;
            }

            using (FileStream fs = new(path, FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new();
                bf.Serialize(fs, obj);
                return true;
            }
        }

        public object DeserializeBinary(Stream stream)
        {
            if (stream == null)
            {
                Debug.LogWarning("DeserializeBinary Failed!");
                return null;
            }


            using (stream)
            {
                BinaryFormatter bf = new();
                object data = bf.Deserialize(stream);

                // TODO:这里没风险嘛?
                return data;
            }
        }

        public object DeserializeBinary(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("DeserializeBinary Without Valid Path.");
                return null;
            }

            FileInfo fileInfo = new(path);

            if (!fileInfo.Exists)
            {
                Debug.LogWarning("DeserializeBinary File Not Exit.");
                return null;
            }

            using (FileStream fs = fileInfo.OpenRead())
            {
                BinaryFormatter bf = new();

                object data = bf.Deserialize(fs);

                return data;
            }
        }


        public object DeserializeBinary(byte[] info)
        {
            object obj;

            using (MemoryStream ms = new(info))
            {
                IFormatter iFormatter = new BinaryFormatter();
                obj = iFormatter.Deserialize(ms);
            }

            return obj;
        }
    }
}