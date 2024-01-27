using UnityEngine;

namespace Hahaha.System {
    public struct ScaledTimer {
        private double _startTime;

        public void Reset() {
            _startTime = Time.timeSinceLevelLoadAsDouble;
        }

        public void Reset(double offset) {
            _startTime = Time.timeSinceLevelLoadAsDouble - offset;
        }

        public void ResetFromCurrentTime(double currentTime) {
            Reset(currentTime - ElapsedAsDouble);
        }

        public void ResetFromCurrentInterval(double interval) {
            Reset(ElapsedAsDouble % interval);
        }

        public double ElapsedAsDouble => Time.timeSinceLevelLoadAsDouble - _startTime;

        public float Elapsed => (float)(Time.timeSinceLevelLoadAsDouble - _startTime);
    }
}
