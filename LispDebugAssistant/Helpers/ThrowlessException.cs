using System;

namespace Nucs.Alda.Helpers {
    class ThrowlessException : Exception {
        public ThrowlessException(string message, string stackTrace) : base(message) {
            this.StackTrace = stackTrace;
        }

        public ThrowlessException(string message, string stackTrace, Exception inner) : base(message, inner) {
            this.StackTrace = stackTrace;
        }


        public override string StackTrace { get; }
    }
}