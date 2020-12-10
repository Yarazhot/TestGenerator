using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Main
{
    public class StackTracer : ITracer
    {
        private Dictionary<Thread, Stack<MethodTraceResult>> executableQueries;
        private Dictionary<Thread, List<MethodTraceResult>> stoppedQueries;

        public StackTracer()
        {
            executableQueries = new Dictionary<Thread, Stack<MethodTraceResult>>();
            stoppedQueries = new Dictionary<Thread, List<MethodTraceResult>>();
        }

        public void StartTrace()
        {
            lock (executableQueries)
            {
                Thread currentThread = Thread.CurrentThread;
                if (!executableQueries.ContainsKey(currentThread))
                {
                    executableQueries.Add(currentThread, new Stack<MethodTraceResult>());
                }

                StackTrace stackTrace = new StackTrace();
                MethodBase callerMethod = stackTrace.GetFrame(1).GetMethod();

                MethodTraceResult currentMethodTrace = new MethodTraceResult();
                currentMethodTrace.clazz = callerMethod.ReflectedType.Name;
                currentMethodTrace.name = callerMethod.Name;
                currentMethodTrace.Stopwatch = new Stopwatch();
                currentMethodTrace.Stopwatch.Start();
                executableQueries[currentThread].Push(currentMethodTrace);
            }
        }

        public void StopTrace()
        {
            lock (executableQueries)
            {
                Thread currentThread = Thread.CurrentThread;
                Stack<MethodTraceResult> currentStack = executableQueries[currentThread];
                MethodTraceResult currentMethodTrace = currentStack.Pop();
                currentMethodTrace.Stopwatch.Stop();
                if (currentStack.Count > 0)
                {
                    MethodTraceResult fatherTrace = currentStack.Peek();
                    if (fatherTrace.methods == null)
                    {
                        fatherTrace.methods = new List<MethodTraceResult>();
                    }

                    fatherTrace.methods.Add(currentMethodTrace);
                }
                else
                {
                    if (!stoppedQueries.ContainsKey(currentThread))
                    {
                        stoppedQueries.Add(currentThread, new List<MethodTraceResult>());
                    }

                    stoppedQueries[currentThread].Add(currentMethodTrace);
                }
            }
        }

        public TraceResult GetTraceResult()
        {
            return new TraceResult(stoppedQueries);
        }
    }
}