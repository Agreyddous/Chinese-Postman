using System;
using System.Collections.Generic;
using System.Linq;
using ChinesePostman.ValueObjects;
using Newtonsoft.Json;

namespace ChinesePostman.Entities
{
	public class Graph : Entity
	{
		public Graph(List<Vertex> vertices) => Vertices = vertices;
		public Graph(double?[,] matrix) => _fromMatrix(matrix);

		public List<Vertex> Vertices { get; private set; }
		public List<Connection> Connections { get => _getConnections(); }
		public bool Eulerian { get => !Vertices.Where(vertex => !vertex.Balanced).Any(); }
		public double Cost { get => Connections.Sum(connection => connection.Cost); }
		public List<Connection> Circuit { get => _calculateCircuit(Vertices.FirstOrDefault(), new List<Connection>()); }

		public override string ToString() => JsonConvert.SerializeObject(_toMatrix());

		public bool IsStronglyConnected()
		{
			bool result = false;
			bool[] visitedVertices = new bool[Vertices.Count];

			_performDFS(0,
			   visitedVertices,
			   vertex => vertex.OutboundConnections,
			   connection => Vertices.IndexOf(connection.To));

			if (!visitedVertices.Where(visitedVertex => !visitedVertex).Any())
			{
				visitedVertices = new bool[Vertices.Count];

				_performDFS(0,
					visitedVertices,
					vertex => vertex.InboundConnections,
					connection => Vertices.IndexOf(connection.From));

				result = !visitedVertices.Where(visitedVertex => !visitedVertex).Any();
			}

			return result;
		}

		public void Balance()
		{
			List<Vertex> positivelyUnbalancedVertices = new List<Vertex>();
			Dictionary<Vertex, Dictionary<Vertex, Path>> negativelyUnbalancedVertices = new Dictionary<Vertex, Dictionary<Vertex, Path>>();

			foreach (Vertex vertex in Vertices)
			{
				if (vertex.BalanceDifference > 0)
					positivelyUnbalancedVertices.Add(vertex);

				else if (vertex.BalanceDifference < 0)
					negativelyUnbalancedVertices.Add(vertex, _getMinimumDistance(vertex));
			}

			List<UnbalancedVerticesPair> unbalancedVerticesPairs = new List<UnbalancedVerticesPair>();

			foreach (KeyValuePair<Vertex, Dictionary<Vertex, Path>> negativelyUnbalancedVertex in negativelyUnbalancedVertices)
				foreach (Vertex positivelyUnbalancedVertex in positivelyUnbalancedVertices)
					unbalancedVerticesPairs.Add(new UnbalancedVerticesPair(negativelyUnbalancedVertex, positivelyUnbalancedVertex));

			unbalancedVerticesPairs.Sort();
			List<Vertex> usedVertices = new List<Vertex>();
			List<UnbalancedVerticesPair> usedUnbalancedVerticesPairs = new List<UnbalancedVerticesPair>();

			foreach (UnbalancedVerticesPair unbalancedVerticesPair in unbalancedVerticesPairs)
			{
				if (!usedVertices.Contains(unbalancedVerticesPair.From)
					&& !usedVertices.Contains(unbalancedVerticesPair.To))
				{
					usedVertices.Add(unbalancedVerticesPair.To);
					usedVertices.Add(unbalancedVerticesPair.From);

					usedUnbalancedVerticesPairs.Add(unbalancedVerticesPair);
				}
			}

			foreach (UnbalancedVerticesPair unbalancedVerticePair in usedUnbalancedVerticesPairs)
				foreach (Connection pathConnection in unbalancedVerticePair.Path)
					pathConnection.From.Connect(pathConnection.Cost, pathConnection.To);
		}

		private List<Connection> _getConnections()
		{
			IEnumerable<Connection> connections = new List<Connection>();

			foreach (IEnumerable<Connection> vertexConnections in Vertices.Select(vertex => vertex.Connections))
				connections = connections.Union(vertexConnections);

			return connections.ToList();
		}

		private void _performDFS(int vertexIndex,
						   bool[] visitedVertices,
						   Func<Vertex, IEnumerable<Connection>> connectionsFunction,
						   Func<Connection, int> connectedVertexIndexFunction)
		{
			visitedVertices[vertexIndex] = true;

			foreach (Connection connection in connectionsFunction(Vertices[vertexIndex]))
			{
				int connectedVertexIndex = connectedVertexIndexFunction(connection);

				if (!visitedVertices[connectedVertexIndex])
					_performDFS(connectedVertexIndex,
						visitedVertices,
						connectionsFunction,
						connectedVertexIndexFunction);
			}
		}

		private double?[,] _toMatrix()
		{
			double?[,] matrix = new double?[Vertices.Count, Vertices.Count];

			for (int rowIndex = 0; rowIndex < Vertices.Count; rowIndex++)
				for (int collumnIndex = 0; collumnIndex < Vertices.Count; collumnIndex++)
					matrix[rowIndex, collumnIndex] = Vertices[rowIndex].OutboundConnections.Where(connection => connection.To == Vertices[collumnIndex]).FirstOrDefault()?.Cost;

			return matrix;
		}

		private void _fromMatrix(double?[,] matrix)
		{
			Vertices = new List<Vertex>();

			int verticesCount = matrix.GetLength(1);

			for (int vertexIndex = 0; vertexIndex < verticesCount; vertexIndex++)
				Vertices.Add(new Vertex(vertexIndex.ToString()));

			for (int rowIndex = 0; rowIndex < verticesCount; rowIndex++)
				for (int collumnIndex = 0; collumnIndex < verticesCount; collumnIndex++)
					if (matrix[rowIndex, collumnIndex] != null)
						Vertices[rowIndex].Connect(matrix[rowIndex, collumnIndex].Value,
								 Vertices[collumnIndex]);
		}

		private Dictionary<Vertex, Path> _getMinimumDistance(Vertex sourceVertex)
		{
			Dictionary<Vertex, Path> distances = new Dictionary<Vertex, Path>();
			List<Vertex> shortestPathFound = new List<Vertex>();

			foreach (Vertex vertex in Vertices)
				distances.Add(vertex, new Path());

			distances[sourceVertex].InitializeCost();

			for (int step = 0; step < Vertices.Count - 1; step++)
			{
				Vertex closestVertex = _closestVertex(distances, shortestPathFound);
				shortestPathFound.Add(closestVertex);

				foreach (Vertex vertex in Vertices.Where(vertex => !shortestPathFound.Contains(vertex)))
				{
					Connection connection = closestVertex.Connection(vertex);

					if (connection != null
						&& distances[closestVertex].Cost + connection.Cost < distances[vertex].Cost)
					{
						distances[vertex] = distances[closestVertex].Clone();
						distances[vertex].Add(connection);
					}
				}
			}

			return distances;
		}

		private Vertex _closestVertex(Dictionary<Vertex, Path> distances, List<Vertex> shortestPath)
		{
			double distance = double.MaxValue;
			Vertex closestVertex = null;

			foreach (Vertex vertex in Vertices)
				if (!shortestPath.Contains(vertex) && distances[vertex].Cost <= distance)
				{
					distance = distances[vertex].Cost;
					closestVertex = vertex;
				}

			return closestVertex;
		}

		private List<Connection> _calculateCircuit(Vertex vertex, List<Connection> circuit)
		{
			Connection connection = vertex.OutboundConnections.Where(connection => !circuit.Contains(connection)).FirstOrDefault();

			if (connection != null)
			{
				circuit.Add(connection);
				_calculateCircuit(connection.To, circuit);
			}

			return circuit;
		}
	}
}