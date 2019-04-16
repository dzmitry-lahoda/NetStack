namespace NetStack.Serialization
{
    internal static class Optimization
    {
        // backport of values from 3.0 to target performance into future, compile fold old target still get improvements on new runtime
        public const short AggressiveInliningAndOptimization = 0x0100 | 0x0200;
    }
}