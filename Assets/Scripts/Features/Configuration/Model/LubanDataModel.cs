using System.IO;
using Luban;
using QFramework;
using UnityEngine;
using UnityEngine.Networking;

namespace Features.Configuration.Model
{
    public class LubanDataModel : ILubanDataModel
    {
        private static readonly string DataDir = Application.streamingAssetsPath + "/LubanData";

        public cfg.Tables Tables { get; }

        public LubanDataModel()
        {
            Tables = new cfg.Tables(LoadByteBuf);
        }

        private ByteBuf LoadByteBuf(string fileName)
        {
            string path = Path.Combine(DataDir, fileName + ".bytes");

#if UNITY_ANDROID && !UNITY_EDITOR
            using UnityWebRequest request = UnityWebRequest.Get(path);
            request.SendWebRequest();
            while (!request.isDone) { }
            if (request.result != UnityWebRequest.Result.Success)
            {
                return new ByteBuf(new byte[0]);
            }
            return new ByteBuf(request.downloadHandler.data);
#else
            if (!File.Exists(path))
            {
                return new ByteBuf(new byte[0]);
            }
            return new ByteBuf(File.ReadAllBytes(path));
#endif
        }
    }
}
