using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;

namespace CoprimePiGenerator {

	class Program {

		static RandomNumberGenerator rng = RandomNumberGenerator.Create();
		static Stopwatch watch = new Stopwatch();
		static double average_time = 0;

		struct Approximation {

			public int iterations;
			public int coprimes;
			public double ratio;
			public double approximation;
			public double error;

		}

		static void Main(string[] args) {

			List<Approximation> attempts = new List<Approximation>();

			int start = 0;
			int stop  = 1000000;
			int step  = 1000;

			Console.CursorVisible = false;

			int cursor_left_write = Console.CursorLeft;
			int cursor_top_write = Console.CursorTop;

			for (int i = start; i < stop; i += step) {

				watch.Reset();
				watch.Start();
				attempts.Add(ApproximatePi(i, false));
				watch.Stop();

				average_time = (average_time + watch.ElapsedMilliseconds) / (i / step);

				if (double.IsNaN(average_time))
					average_time = 0;

				Console.Write(string.Format("{0:00.0}%\n{1:0.00}ms\n{2} random numbers", ((float)(100 * i)/(stop-start)), average_time, i));
				Console.SetCursorPosition(cursor_left_write, cursor_top_write);

			}

			using (StreamWriter sw = new StreamWriter("dataset-a.csv")) {

				for (int i = 0; i < attempts.Count; i++) {

					sw.WriteLine(attempts[i].iterations + "," + attempts[i].approximation + "," + attempts[i].error);

				}

			}

			Console.CursorVisible = true;

		}

		static Approximation ApproximatePi(int iterations, bool print) {

			int coprimes = 0;

			for (int i = 0; i < iterations; i++) {

				uint r1 = RandomIntRange(0, 1000);
				uint r2 = RandomIntRange(0, 1000);

				if (GCD(new uint[] { r1, r2 }) == 1) {

					coprimes++;

				}

			}

			double ratio = (double)coprimes / iterations;
			double approximation = Math.Sqrt((6.0f / ratio));
			double error = (Math.Abs(approximation - Math.PI) / Math.PI) * 100;

			Approximation approx = new Approximation();
			approx.iterations = iterations;
			approx.coprimes = coprimes;
			approx.ratio = ratio;
			approx.approximation = approximation;
			approx.error = error;

			if (print) {

				Console.WriteLine(string.Format("{0} -> {1} -> {2}", coprimes, ratio, approximation));
				Console.WriteLine(string.Format("{0}% error", error));

			}

			return approx;

		}

		static UInt32 RandomIntRange(uint start, uint stop) {

			byte[] data = new byte[4];
			rng.GetBytes(data);

			uint n = BitConverter.ToUInt32(data, 0);

			n %= stop;

			return n;

		}

		static uint GCD(uint[] numbers) {

			return numbers.Aggregate(GCD);

		}

		static uint GCD(uint x, uint y) {

			return y == 0 ? x : GCD(y, x % y);

		}

	}

}
