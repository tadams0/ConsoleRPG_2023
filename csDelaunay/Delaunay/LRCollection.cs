using System;

namespace csDelaunay
{
	public class LRCollection<T>
	{
		public T Left
		{
			get { return left; }
		}

		public T Right
		{
			get { return right; }
		}

		private T left;
		private T right;
		public T this[LR index]
		{
			get
			{
				return index == LR.LEFT ? left : right;
			}
			set
			{
				if (index == LR.LEFT)
				{
					left = value;
				}
				else
				{
					right = value;
				}
			}
		}

		public void Clear()
		{
			left = default(T);
			right = default(T);
		}
	}
}