using System;

namespace Sandbox
{
	/// <summary> Singleton that maintains a seed for all generated numbers. </summary>
	internal class SeedRandom
	{
		public Random rand { get; private set; }
		private int seed;

		public static SeedRandom Instance
		{
			get
			{
				if ( instance == null )
				{
					throw new Exception( "SeedRandom not yet initialized." );
				}
				return instance;
			}
		}
		private static SeedRandom instance;

		public static SeedRandom Initialize( int seed )
		{
			if ( instance != null )
			{
				if ( seed == instance.seed )
				{
					Reset();
				}
				else
				{
					Log.Info( $"SeedRandom overwrote seed {instance.seed} with {seed}." );
					instance.Force( seed );
				}
			}

			instance = new SeedRandom( seed );
			return instance;
		}

		public static void Reset()
		{
			Instance.rand = new Random( Instance.seed );
		}

		public static int Next()
		{
			return Instance.rand.Next();
		}

		public static int Next( int maxValue )
		{
			return Instance.rand.Next( maxValue );
		}

		public static int Next( int minValue, int maxValue )
		{
			return Instance.rand.Next( minValue, maxValue );
		}

		public static Vector3 VectorInCube( BBox cube )
		{
			return Instance.rand.VectorInCube( cube );
		}

		public static Color RandomColor()
		{
			return Instance.RandomColorInstance();
		}

		private SeedRandom( int seed )
		{
			rand = new Random( seed );
			this.seed = seed;
		}

		private void Force( int seed )
		{
			Instance.rand = new Random( seed );
			Instance.seed = seed;
		}

		public Color RandomColorInstance()
		{
			switch ( rand.Next( 6 ) )
			{
				case 0: return Color.White;
				case 1: return Color.Red;
				case 2: return Color.Green;
				case 3: return Color.Blue;
				case 4: return Color.Yellow;
				case 5: return Color.Cyan;
				case 6: return Color.Magenta;
				default: return Color.Black;
			}
		}
	}
}
