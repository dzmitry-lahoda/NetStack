using System;
using System.Threading;

namespace System
{
    // does not contains and will not contain Exception shortcut
    //What about FileNotFoundException and SerializationException?
    internal static class Except
    {
        public static ArgumentNullException ArgumentNull() => new ArgumentNullException();

        public static ArgumentNullException ArgumentNull(string paramName) => new ArgumentNullException(paramName);

        public static ArgumentNullException ArgumentNull(string message, Exception innerException) => new ArgumentNullException(message, innerException);

        public static ArgumentNullException ArgumentNull(string paramName, string message) => new ArgumentNullException(paramName, message);

        public static ArgumentOutOfRangeException ArgumentOutOfRange() => new ArgumentOutOfRangeException();

        public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName) => 
            new ArgumentOutOfRangeException(paramName);
        public static ArgumentOutOfRangeException ArgumentOutOfRange(string message, Exception innerException) => 
            new ArgumentOutOfRangeException(message, innerException);

        public static IndexOutOfRangeException IndexOutOfRange() => new IndexOutOfRangeException();

        public static IndexOutOfRangeException IndexOutOfRange(string message) => 
            new IndexOutOfRangeException(message);
        public static IndexOutOfRangeException IndexOutOfRange(string message, Exception innerException) => 
            new IndexOutOfRangeException(message, innerException);            

        public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string message) => 
            new ArgumentOutOfRangeException(paramName, message);            

        public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object actualValue, string message) => 
            new ArgumentOutOfRangeException(paramName, actualValue, message);

        public static ArgumentException Argument() => new ArgumentException();
        public static ArgumentException Argument(string message) => new ArgumentException(message);
        public static ArgumentException Argument(string message, Exception innerException) => new ArgumentException(message, innerException);
        public static ArgumentException Argument(string message, string paramName) => new ArgumentException(message, paramName);
        public static ArgumentException Argument(string message, string paramName, Exception innerException) => new ArgumentException(message, paramName, innerException);

        public static TimeoutException Timeout() => new TimeoutException();
        public static TimeoutException Timeout(string message) => new TimeoutException(message);
        public static TimeoutException Timeout(string message, Exception innerException) => new TimeoutException(message, innerException);

        public static ObjectDisposedException ObjectDisposed(string objectName) => new ObjectDisposedException(objectName);
        public static ObjectDisposedException ObjectDisposed(string message, Exception innerException) => new ObjectDisposedException(message, innerException);
        public static ObjectDisposedException ObjectDisposed(string objectName, string message) => new ObjectDisposedException(objectName, message);

        public static NotSupportedException NotSupported() => new NotSupportedException();
        public static NotSupportedException NotSupported(string message) => new NotSupportedException(message);
        public static NotSupportedException NotSupported(string message, Exception innerException) => new NotSupportedException(message, innerException);

        public static InvalidOperationException InvalidOperation() => new InvalidOperationException();
        public static InvalidOperationException InvalidOperation(string message) => new InvalidOperationException(message);
        public static InvalidOperationException InvalidOperation(string message, Exception innerException) => 
            new InvalidOperationException(message, innerException);
        
        public static InvalidProgramException InvalidProgram() => new InvalidProgramException();
        public static InvalidProgramException InvalidProgram(string message) => new InvalidProgramException(message);

        public static InvalidProgramException InvalidProgram(string message, Exception inner) => 
            new InvalidProgramException(message, inner);

        public static NotImplementedException NotImplemented() => new NotImplementedException();
        public static NotImplementedException NotImplemented(string message) => new NotImplementedException(message);

        public static NotImplementedException NotImplemented(string message, Exception inner) => 
            new NotImplementedException(message, inner);

        public static OperationCanceledException OperationCanceled() => new OperationCanceledException();

        public static OperationCanceledException OperationCanceled(string message) => new OperationCanceledException(message);

        public static OperationCanceledException OperationCanceled(string message, Exception innerException) => 
            new OperationCanceledException(message, innerException);

        public static OperationCanceledException OperationCanceled(string message, Exception innerException, CancellationToken token) => 
            new OperationCanceledException(message, innerException, token);

        public static OperationCanceledException OperationCanceled(string message, CancellationToken token) => 
            new OperationCanceledException(message, token);

        public static OperationCanceledException OperationCanceled(CancellationToken token) => 
            new OperationCanceledException(token);
        
        // Drop this with C# wide spread adoptions
        public static T NotNull<T>(this T self, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "") =>
            self == null ? throw ArgumentNull(memberName) : self;
    }
}