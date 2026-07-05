using Features.Enemy.Define;

namespace Features.Enemy.Data
{
    public class EnemyRuntimeData
    {
        public int EnemyId;

        public int HP;

        public int MaxHP;

        public int Armor;

        public int Damage;

        public int MoveSpeed;

        public int SlotIndex;

        public bool IsFacingRight;

        public EnemyIntentType CurrentIntent;

        public EnemyRuntimeData MutableClone()
        {
            return (EnemyRuntimeData)MemberwiseClone();
        }
    }
}