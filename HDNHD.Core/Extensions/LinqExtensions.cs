using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HDNHD.Core.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Used to chain an object with customization,
        /// such as modifying properties of an object returned from a LINQ query.
        /// Chaining of methods is supported since the method returns the object operated on.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="input"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        public static TSource Select<TSource>(this TSource input, Action<TSource> updater)
        {
            updater(input);
            return input;
        }

        /// <summary>
        /// Used to chain an object with customization,
        /// such as modifying properties of an object returned from a LINQ query
        /// Supports returning a different object, thus changing what object the next method
        /// in the chain operates on.
        /// Can also be used to project an object onto a lambda,
        /// removing the need for a local object reference
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="input"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        public static TResult Select<TSource, TResult>(this TSource input, Func<TSource, TResult> updater)
        {
            return updater(input);
        }
    }
}