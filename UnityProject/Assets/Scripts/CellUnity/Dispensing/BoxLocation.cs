namespace CellUnity.Model.Dispensing
{
	public class BoxLocation
	{
		public BoxLocation(long x, long y, long z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		public long X { get; private set; }
		public long Y { get; private set; }
		public long Z { get; private set; }
		
		public override int GetHashCode()
		{
			return (int)(X + Y * 7 + Z * 17);
		}
		
		public override bool Equals(object obj)
		{
			if (obj != null && GetType().Equals(obj.GetType()))
			{
				BoxLocation other = (BoxLocation)obj;
				return (X == other.X && Y == other.Y && Z == other.Z);
			}
			
			return false;
		}
		
		public override string ToString()
		{
			return string.Format("[X={0}, Y={1}, Z={2}]", X, Y, Z);
		}
		
		public BoxLocation Move(int dx, int dy, int dz)
		{
			return new BoxLocation(X + dx, Y + dy, Z + dz);
		}
	}
}