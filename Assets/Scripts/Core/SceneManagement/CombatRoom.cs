namespace Core.SceneManagement
{
    public class CombatRoom : SceneBase
    {
        public override string SceneId { get => "CombatRoomRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Room; }
    }
}