using System;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public struct ExponentialBackoff
    {
        private readonly int _delayMilliseconds;
        private readonly int _maxDelayMilliseconds;
        private int _retries;
        private int _pow;

        public ExponentialBackoff(int delayMilliseconds, int maxDelayMilliseconds)
        {
            _delayMilliseconds = delayMilliseconds;
            _maxDelayMilliseconds = maxDelayMilliseconds;
            _retries = 0;
            _pow = 1;
        }

        public int Retries => _retries;

        public void Reset()
        {
            _retries = 0;
            _pow = 1;
        }

        public int GetNextDelay()
        {
            var pow = _pow;
            if (_retries + 1 < 31)
            {
                pow = pow << 1; // m_pow = Pow(2, m_retries - 1)
            }

            var delay = Math.Min(_delayMilliseconds * (pow - 1) / 2, _maxDelayMilliseconds);
            return delay;
        }

        public Task Delay()
        {
            var delay = GetNextDelay();
            ++_retries;
            if (_retries < 31) {
                _pow = _pow << 1;
            }
            
            return Task.Delay(delay);
        }
    }
}