public static class FlagExtensions
{
    public static bool HasFlag(this int First, int Second) => (First & Second) == Second;
    public static int AddFlagReturn(this int First, int Second) => First | Second;
    public static void AddFlag(this ref int First, int Second) => First |= Second;
    public static int RemoveFlagReturn(this int First, int Second) => First & ~Second;
    public static void RemoveFlag(this ref int First, int Second) => First &= ~Second;
}