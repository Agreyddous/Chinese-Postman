using System;

namespace ChinesePostman.Entities
{
	public abstract class Entity
	{
		public Entity() => Id = Guid.NewGuid();

		public Guid Id { get; private set; }
	}
}