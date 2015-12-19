
namespace TapeSimulatorConsole
{
    public enum FilePosition
    {
        Head,
        Remain,
        Tail,
        Total
    }

    public enum WebSessionStatus
    {
        Inactive,
        Logging,
        Active
    }

    public enum WebSocketServerFeatureType
    {
        // Support multiple child ESS
        MultiESS,
        TapeIntegration
    }
}
