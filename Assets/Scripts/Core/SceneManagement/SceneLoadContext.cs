namespace Core.SceneManagement
{
    public class SceneLoadContext
    {
        public static readonly SceneLoadContext Empty = new();

        public string CharacterId { get; set; }

        public string LevelId { get; set; }

        public object UserData { get; set; }
    }
}