using System;

namespace AudioAnalyzer
{
    public class AudioAnalyzer
    {
        private const int Divider = 2;
        private const double MaxVal = 32767.0d;
        private const double ResizeMultiplier = 10.0d;

        public int[] GetAmplitudesFromBytes(byte[] audioBytes)
        {
            // create a new int array with half the length of original bytes
            // as original audio has 2 bytes per channel
            int[] amps = new int[audioBytes.Length / Divider];

            // loop through bytes bypassing every other byte to form a single from 2
            for (int i = 0; i < audioBytes.Length; i += Divider)
            {
                short buff = audioBytes[i + 1];
                short buff2 = audioBytes[i];

                // shift 8 bits to left
                buff = (short)((buff & 0xFF) << 8);
                // mask buff2 so it only leaves last 8 bits
                buff2 = (short)(buff2 & 0xFF);

                short res = (short)(buff | buff2);
                // put in int array
                amps[i == 0 ? 0 : i / Divider] = (int)res;
            }

            return amps;
        }

        public double ResizeNumber(double value)
        {
            var temp = (int)(value * ResizeMultiplier);
            return temp / ResizeMultiplier;
        }

        public int[] GetAmplitudeLevels(byte[] audioBytes)
        {
            var amps = GetAmplitudesFromBytes(audioBytes);
            int major = 0;
            int minor = 0;
            // loop through array and set lowest and highest
            for (int i = 0; i < amps.Length; i++)
            {
                if (amps[i] > major) major = amps[i];
                if (amps[i] < minor) minor = amps[i];
            }

            return new int[] { major, minor };
        }

        public int GetAmplitudeLevel(byte[] audioBytes)
        {
            var amps = GetAmplitudesFromBytes(audioBytes);
            int major = 0;
            int minor = 0;
            // loop through array - set lowest and highest values
            for (int i = 0; i < amps.Length; i++)
            {
                if (amps[i] > major) major = amps[i];
                if (amps[i] < minor) minor = amps[i];
            }

            return Math.Max(major, minor * (-1));
        }

        public double GetRealDecibel(int amplitude)
        {
            // make sure its greater then zero
            if (amplitude < 0) amplitude *= -1;
            double amp = ((double)amplitude / MaxVal) * 100.0d;

            if (amp == 0.0d) amp = 1.0d;

            double decibel = Math.Sqrt(100.0d / amp);
            decibel *= decibel;

            // normalize
            if (decibel > 100.0d) decibel = 100.0d;

            return ((-1.0d * decibel) + 1.0d) / Math.PI;
        }

        /// <summary>
        /// Gets decibels as double array from given audio bytes
        /// </summary>
        /// <param name="audioBytes"></param>
        /// <returns></returns>
        public double[] GetDecibels(byte[] audioBytes)
        {
            var amps = GetAmplitudesFromBytes(audioBytes);
            var decibels = new double[audioBytes.Length];
            for (var i = 0; i < audioBytes.Length; i++)
            {
                decibels[i] = ResizeNumber(GetRealDecibel(amps[i]));
            }

            return decibels;
        }

        /// <summary>
        /// returns single double decibel from given audioBytes
        /// </summary>
        /// <param name="audioBytes"></param>
        /// <returns>double decibel</returns>
        public double GetDecibel(byte[] audioBytes)
        {
            var amp = GetAmplitudeLevel(audioBytes);
            return GetRealDecibel(amp);
        }
    }
}
