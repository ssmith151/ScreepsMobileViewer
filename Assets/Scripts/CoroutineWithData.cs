using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScreepsViewer
{
    public class CoroutineWithData
    {
        public Coroutine Routine { get; private set; }
        public object result;
        private IEnumerator target;
        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            //Debug.Log("starting CoRoutineWithData...");
            this.target = target;
            this.Routine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (target.MoveNext())
            {
                result = target.Current;
                yield return result;
            }
        }
    }
}
