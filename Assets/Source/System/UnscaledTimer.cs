using UnityEngine;

namespace Hahaha.System {
    public struct UnscaledTimer {
        private double _startTime;

        public void Reset() {
            _startTime = Time.unscaledTimeAsDouble;
        }

        public void Reset(double offset) {
            _startTime = Time.unscaledTimeAsDouble - offset;
        }

        public double ElapsedAsDouble => Time.unscaledTimeAsDouble - _startTime;

        public float Elapsed => (float)(Time.unscaledTimeAsDouble - _startTime);
    }
}
