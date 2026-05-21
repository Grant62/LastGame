using System.Collections.Generic;

namespace QFramework
{
    public class ElementCodeInfo
    {
        public BindInfo BindInfo;
        public string BehaviourName;
        public Dictionary<string, string> DicNameToFullName = new();
        public readonly List<BindInfo> BindInfos = new();
        public readonly List<ElementCodeInfo> ElementCodeDatas = new();
    }
}