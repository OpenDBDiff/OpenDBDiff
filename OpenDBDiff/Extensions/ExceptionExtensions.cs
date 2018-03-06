using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDBDiff.Front.Extensions
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<Exception> FlattenHierarchy(this Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var innerException = ex;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }

        public static string GetAllMessages(this Exception exception)
        {
            var messages = exception
                .FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message);

            return String.Join(Environment.NewLine, messages);
        }
    }
}
