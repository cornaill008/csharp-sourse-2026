namespace System.Runtime.CompilerServices
{
    // Unity's runtime doesn't ship this marker type, but the C# 9 compiler needs it
    // present to emit `init` accessors (and therefore `record` types). This satisfies
    // that requirement for anything compiled inside the Core assembly (Result<,>).
    internal static class IsExternalInit
    {
    }
}
