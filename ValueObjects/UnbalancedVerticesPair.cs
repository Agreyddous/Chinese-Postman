using System;
using System.Collections.Generic;
using ChinesePostman.Entities;

namespace ChinesePostman.ValueObjects
{
	public class UnbalancedVerticesPair : IComparable<UnbalancedVerticesPair>
	{
		public UnbalancedVerticesPair(KeyValuePair<Vertex, Dictionary<Vertex, Path>> from, Vertex to)
		{
			From = from.Key;
			To = to;
			Path = from.Value[to];
		}

		public Vertex From { get; private set; }
		public Vertex To { get; private set; }
		public Path Path { get; private set; }

		public int CompareTo(UnbalancedVerticesPair other) => Convert.ToInt32(Path.Cost - other.Path.Cost);
	}
}