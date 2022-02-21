using System;
using UnityEngine;

namespace UnityExtras.Optional
{
    public sealed class WaitUntilWithTimeout : CustomYieldInstruction
    {
        private readonly Func<bool> _predicate;
        private readonly float _timeout;

        public override bool keepWaiting => !_predicate() && Time.time < _timeout;

        public WaitUntilWithTimeout(Func<bool> predicate, float timeout)
        {
            _predicate = predicate;
            _timeout = Time.time + timeout;
        }
    }
}