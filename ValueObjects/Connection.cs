using ChinesePostman.Entities;

namespace ChinesePostman.ValueObjects
{
	public class Connection
	{
		public Connection(double cost, Vertex from, Vertex to)
		{
			Cost = cost;
			From = from;
			To = to;
		}

		public double Cost { get; private set; }
		public Vertex From { get; private set; }
		public Vertex To { get; private set; }

		public override string ToString() => $"{From} --{Cost}--> {To}";
	}
}