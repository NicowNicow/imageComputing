using System;

namespace imageComputing 
{
    //Allow Tail Recursion in C#. Slower than regular recursion, but will prevent stack overflow

    public interface TailRecursionProcess {
        T DoRecursion<T>(Func<TailRecursionResult<T>> Tfunction);
        TailRecursionResult<T> End<T>(T Tresult);
        TailRecursionResult<T> Next<T>(Func<TailRecursionResult<T>> TnextStep);
    }
    
    public class TailRecursionResult<T> {
        public bool isFinalResult {get; private set;}
        public T Tresult {get; private set;}
        public Func<TailRecursionResult<T>> TnextStep {get; private set;}

        internal TailRecursionResult(bool isFinalResult, T Tresult, Func<TailRecursionResult<T>> TnextStep) {
            this.isFinalResult = isFinalResult;
            this.Tresult = Tresult;
            this.TnextStep = TnextStep;
        }
    
    }
}