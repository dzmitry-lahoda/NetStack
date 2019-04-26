namespace NetStack.Serialization
{
    internal static class Optimization
    {
        #if !NO_INLINING
        // backport of values from 3.0 to target performance into future, compile fold old target still get improvements on new runtime
        public const short AggressiveInliningAndOptimization = 0x0100 | 0x0200;
        
        #else
        public const short AggressiveInliningAndOptimization = 0;
        #endif
    }
}