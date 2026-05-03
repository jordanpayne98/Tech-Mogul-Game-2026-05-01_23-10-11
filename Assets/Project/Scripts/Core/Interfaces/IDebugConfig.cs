using Project.Core.Debugging;

namespace Project.Core.Interfaces
{
    /// <summary>
    /// Interface allowing Core layer to query debug log settings without depending on Infrastructure.
    /// Concrete implementation lives in Project.Infrastructure.Debugging.DebugConfig.
    /// </summary>
    public interface IDebugConfig
    {
        bool ShouldLog(DebugCategory category);
    }
}
