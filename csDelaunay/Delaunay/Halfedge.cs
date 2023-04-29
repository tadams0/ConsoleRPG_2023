using System.Collections;
using System.Collections.Generic;

namespace csDelaunay {

	public class Halfedge<T>
    {

		#region Pool
		private static Queue<Halfedge<T>> pool = new Queue<Halfedge<T>>();

		public static Halfedge<T> Create(Edge<T> edge, LR lr) {
			if (pool.Count > 0) {
				return pool.Dequeue().Init(edge,lr);
			} else {
				return new Halfedge<T>(edge,lr);
			}
		}
		public static Halfedge<T> CreateDummy() {
			return Create(null, null);
		}
		#endregion

		#region Object
		public Halfedge<T> edgeListLeftNeighbor;
		public Halfedge<T> edgeListRightNeighbor;
		public Halfedge<T> nextInPriorityQueue;

		public Edge<T> edge;
		public LR leftRight;
		public Vertex vertex;

		// The vertex's y-coordinate in the transformed Voronoi space V
		public float ystar;

		public Halfedge(Edge<T> edge, LR lr) {
			Init(edge, lr);
		}

		private Halfedge<T> Init(Edge<T> edge, LR lr) {
			this.edge = edge;
			leftRight = lr;
			nextInPriorityQueue = null;
			vertex = null;

			return this;
		}

		public override string ToString() {
			return "Halfedge (LeftRight: " + leftRight + "; vertex: " + vertex + ")";
		}

		public void Dispose() {
			if (edgeListLeftNeighbor != null || edgeListRightNeighbor != null) {
				// still in EdgeList
				return;
			}
			if (nextInPriorityQueue != null) {
				// still in PriorityQueue
				return;
			}
			edge = null;
			leftRight = null;
			vertex = null;
			pool.Enqueue(this);
		}

		public void ReallyDispose() {
			edgeListLeftNeighbor = null;
			edgeListRightNeighbor = null;
			nextInPriorityQueue = null;
			edge = null;
			leftRight = null;
			vertex = null;
			pool.Enqueue(this);
		}

		public bool IsLeftOf(Vector2f p) {
			Site<T> topSite;
			bool rightOfSite, above, fast;
			float dxp, dyp, dxs, t1, t2, t3, y1;

			topSite = edge.RightSite;
			rightOfSite = p.x > topSite.x;
			if (rightOfSite && this.leftRight == LR.LEFT) {
				return true;
			}
			if (!rightOfSite && this.leftRight == LR.RIGHT) {
				return false;
			}

			if (edge.a == 1) {
				dyp = p.y - topSite.y;
				dxp = p.x - topSite.x;
				fast = false;
				if ((!rightOfSite && edge.b < 0) || (rightOfSite && edge.b >= 0)) {
					above = dyp >= edge.b * dxp;
					fast = above;
				} else {
					above = p.x + p.y * edge.b > edge.c;
					if (edge.b < 0) {
						above = !above;
					} 
					if (!above) {
						fast = true;
					}
				}
				if (!fast) {
					dxs = topSite.x - edge.LeftSite.x;
					above = edge.b * (dxp * dxp - dyp * dyp) < dxs * dyp * (1+2 * dxp/dxs + edge.b * edge.b);
					if (edge.b < 0) {
						above = !above;
					}
				}
			} else {
				y1 = edge.c - edge.a * p.x;
				t1 = p.y - y1;
				t2 = p.x - topSite.x;
				t3 = y1 - topSite.y;
				above = t1 * t1 > t2 * t2 + t3 * t3;
			}
			return this.leftRight == LR.LEFT ? above : !above;
		}
		#endregion
	}
}
