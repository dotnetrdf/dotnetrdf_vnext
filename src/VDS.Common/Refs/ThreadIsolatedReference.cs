/*
VDS.Common is licensed under the MIT License

Copyright (c) 2012-2015 Robert Vesse

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Threading;

namespace VDS.Common.Refs
{
    /// <summary>
    /// Provides a thread isolated reference to some reference type
    /// </summary>
    /// <typeparam name="T">Reference Type</typeparam>
    /// <remarks>
    /// <para>
    /// Essentially the ThreadIsolatedReference guarantees that each thread that accesses it sees a thread-specific view of the reference.  The initial value of the reference for each Thread is generated either by an initialiser function passed to the constructor when the ThreadSafeReference is created or otherwise is null.  This is essentially what the <see cref="ThreadLocal{T}"/> introduced in .Net 4.0 does but we continue to use this our own wrapper because we need backwards compatibility with .Net 3.5 and ThreadLocal does not play quite right with some of our usage patterns
    /// </para>
    /// </remarks>
    public class ThreadIsolatedReference<T> 
        : IDisposable
        where T : class
    {
        private Dictionary<int, T> _refs = new Dictionary<int, T>();
        private Func<T> _init;

        /// <summary>
        /// Creates a new ThreadSafeReference where the initial value of the reference on each thread is null
        /// </summary>
        public ThreadIsolatedReference()
        { }

        /// <summary>
        /// Creates a new ThreadSafeReference where the initial value of the reference on each thread is generated by the given initialiser function
        /// </summary>
        /// <param name="init">Initialiser Function</param>
        public ThreadIsolatedReference(Func<T> init)
        {
            this._init = init;
        }

        /// <summary>
        /// Gets the initialiser function
        /// </summary>
        public Func<T> Initialiser
        {
            get
            {
                return this._init;
            }
        }

        /// <summary>
        /// Gets/Sets the value for the current thread
        /// </summary>
        public T Value
        {
            get
            {
                try
                {
                    Monitor.Enter(this._refs);
                    int id = Thread.CurrentThread.ManagedThreadId;
                    if (!this._refs.ContainsKey(id))
                    {
                        this._refs.Add(id, null);
                        if (this._init != null)
                        {
                            this._refs[id] = this._init();
                        }
                    }
                    return this._refs[id];
                }
                finally
                {
                    Monitor.Exit(this._refs);
                }
            }
            set
            {
                try
                {
                    Monitor.Enter(this._refs);
                    int id = Thread.CurrentThread.ManagedThreadId;
                    if (this._refs.ContainsKey(id))
                    {
                        this._refs[id] = value;
                    }
                    else
                    {
                        this._refs.Add(id, value);
                    }
                }
                finally
                {
                    Monitor.Exit(this._refs);
                }
            }
        }

        /// <summary>
        /// Disposes of a Thread Safe reference
        /// </summary>
        /// <remarks>
        /// Does not take any dispose actions on the references it holds, it will drop those so the GC can collect them as desired.  This also avoids any unintended consequences of a dispose on one thread causing strange behaviour on another
        /// </remarks>
        public void Dispose()
        {
            try
            {
                Monitor.Enter(this._refs);
                this._refs.Clear();
            }
            finally
            {
                Monitor.Exit(this._refs);
            }
        }
    }

    /// <summary>
    /// Provides a thread isolated reference to some value type
    /// </summary>
    /// <typeparam name="T">Reference Type</typeparam>
    public class ThreadIsolatedValue<T>
        : IDisposable
        where T : struct
    {
        private Dictionary<int, T> _refs = new Dictionary<int, T>();
        private Func<T> _init;

        /// <summary>
        /// Creates a new ThreadSafeValue where the initial value of the struct on each thread is default
        /// </summary>
        public ThreadIsolatedValue() { }

        /// <summary>
        /// Creates a new ThreadSafeValue where the initial value of the struct on each thread is generated by the given initialiser function
        /// </summary>
        /// <param name="init">Initialiser Function</param>
        public ThreadIsolatedValue(Func<T> init)
        {
            this._init = init;
        }

        /// <summary>
        /// Gets the initialiser function
        /// </summary>
        public Func<T> Initialiser
        {
            get
            {
                return this._init;
            }
        }

        /// <summary>
        /// Gets/Sets the value for the current thread
        /// </summary>
        public T Value
        {
            get
            {
                try
                {
                    Monitor.Enter(this._refs);
                    int id = Thread.CurrentThread.ManagedThreadId;
                    if (!this._refs.ContainsKey(id))
                    {
                        T value = (this._init != null) ? this._init() : default(T);
                        this._refs.Add(id, value);
                    }
                    return this._refs[id];
                }
                finally
                {
                    Monitor.Exit(this._refs);
                }
            }
            set
            {
                try
                {
                    Monitor.Enter(this._refs);
                    int id = Thread.CurrentThread.ManagedThreadId;
                    if (this._refs.ContainsKey(id))
                    {
                        this._refs[id] = value;
                    }
                    else
                    {
                        this._refs.Add(id, value);
                    }
                }
                finally
                {
                    Monitor.Exit(this._refs);
                }
            }
        }

        /// <summary>
        /// Disposes of the value
        /// </summary>
        public void Dispose()
        {
            try
            {
                Monitor.Enter(this._refs);
                this._refs.Clear();
            }
            finally
            {
                Monitor.Exit(this._refs);
            }
        }
    }
}
