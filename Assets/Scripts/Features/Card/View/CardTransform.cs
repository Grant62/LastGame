using UnityEngine;

namespace Features.Card.View
{
    public struct CardTransform
    {
        public Vector3 Pos { get; set; }
        public Quaternion Rot { get; set; }

        public CardTransform(Vector3 pos, Quaternion rot)
        {
            Pos = pos;
            Rot = rot;
        }
    }
}
