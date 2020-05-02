using System.Collections;
using System.Collections.Generic;

namespace ChinesePostman.ValueObjects
{
	public class Path : IList<Connection>
	{
		private List<Connection> _connections;
		private double _cost;

		public Path() { }
		private Path(Path path)
		{
			Connection[] newConnections = new Connection[path._connections.Count];
			path.CopyTo(newConnections, 0);

			_connections = new List<Connection>(newConnections);
			_cost = path.Cost;
		}

		public double Cost { get => _connections != null ? _cost : double.MaxValue; }

		public void InitializeCost() => _connections = new List<Connection>();

		public Connection this[int index] { get => _connections[index]; set => _connections[index] = value; }

		public int Count => _connections.Count;

		public Path Clone() => new Path(this);

		public bool IsReadOnly => false;

		public void Add(Connection item)
		{
			_cost += item.Cost;
			_connections.Add(item);
		}

		public void Clear() => _connections.Clear();

		public bool Contains(Connection item) => _connections.Contains(item);

		public void CopyTo(Connection[] array, int arrayIndex) => _connections.CopyTo(array, arrayIndex);

		public IEnumerator<Connection> GetEnumerator() => _connections.GetEnumerator();

		public int IndexOf(Connection item) => _connections.IndexOf(item);

		public void Insert(int index, Connection item)
		{
			_cost += item.Cost;
			_connections.Insert(index, item);
		}

		public bool Remove(Connection item)
		{
			_cost -= item.Cost;
			return _connections.Remove(item);
		}

		public void RemoveAt(int index)
		{
			Connection item = _connections[index];
			Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator() => _connections.GetEnumerator();
	}
}