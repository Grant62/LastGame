using QFramework;

namespace Features.Configuration.Model
{
    public interface ILubanDataModel : IUtility
    {
        cfg.Tables Tables { get; }
    }
}
