using System.Collections.Generic;
using System.Linq;
using ChinesePostman.ValueObjects;

namespace ChinesePostman.Entities
{
	public class Vertex : Entity
	{
		public Vertex(string description)
		{
			Description = description;

			OutboundConnections = new List<Connection>();
			InboundConnections = new List<Connection>();
		}

		public string Description { get; private set; }
		public List<Connection> OutboundConnections { get; private set; }
		public List<Connection> InboundConnections { get; private set; }
		public IEnumerable<Connection> Connections { get => OutboundConnections.Union(InboundConnections); }
		public int Degree { get => OutboundConnections.Count + InboundConnections.Count; }
		public int BalanceDifference { get => (OutboundConnections.Count - InboundConnections.Count); }
		public bool Balanced { get => BalanceDifference == 0; }
		public bool Odd { get => Degree % 2 == 1; }
		public bool Even { get => Degree % 2 == 0; }

		public override string ToString() => Description;

		public Connection Connection(Vertex vertex) => Connections.Where(connection => connection.To == vertex).FirstOrDefault();

		public void Connect(double cost, Vertex vertex)
		{
			Connection connection = new Connection(cost, this, vertex);

			OutboundConnections.Add(connection);
			vertex.InboundConnections.Add(connection);
		}

		public bool IsConnectedTo(Vertex vertex) => OutboundConnections.Where(connection => connection.To == vertex).Any();
	}
}