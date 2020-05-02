using System;
using System.Collections.Generic;
using ChinesePostman.Entities;
using ChinesePostman.ValueObjects;

namespace ChinesePostman
{
	class Program
	{
		static void Main(string[] args)
		{
			Vertex aVertex = new Vertex("A");
			Vertex bVertex = new Vertex("B");
			Vertex cVertex = new Vertex("C");
			Vertex dVertex = new Vertex("D");
			Vertex eVertex = new Vertex("E");
			Vertex fVertex = new Vertex("F");

			aVertex.Connect(10, bVertex);
			aVertex.Connect(20, cVertex);

			bVertex.Connect(10, eVertex);
			bVertex.Connect(50, dVertex);

			cVertex.Connect(33, eVertex);
			cVertex.Connect(20, dVertex);

			dVertex.Connect(5, eVertex);
			dVertex.Connect(12, fVertex);

			eVertex.Connect(12, aVertex);
			eVertex.Connect(1, fVertex);

			fVertex.Connect(22, cVertex);

			List<Vertex> vertices = new List<Vertex> { aVertex, bVertex, cVertex, dVertex, eVertex, fVertex };

			Graph graph = new Graph(vertices);

			Console.Clear();
			Console.WriteLine("Inicial Graph:");
			Console.WriteLine(graph.ToString());

			if (graph.IsStronglyConnected())
			{
				while (!graph.Eulerian)
					graph.Balance();

				Console.WriteLine($"\n\nGraph Total Cost: {graph.Cost}\n");

				Console.WriteLine("Circuit:\n");
				foreach (Connection connection in graph.Circuit)
					Console.Write($"{connection} | ");

				Console.WriteLine("\n\nFinal Graph:");
				Console.WriteLine(graph.ToString());
			}

			else
				Console.WriteLine("Graph is not Strongly Connect. Can't find a solution");
		}
	}
}
