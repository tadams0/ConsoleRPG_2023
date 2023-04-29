using System.Collections;
using System.Collections.Generic;

namespace csDelaunay {

	public class EdgeList<T>
    {

		private float deltaX;
		private float xmin;

		private int hashSize;
		private Halfedge<T>[] hash;
		private Halfedge<T> leftEnd;
		public Halfedge<T> LeftEnd {get{return leftEnd;}}
		private Halfedge<T> rightEnd;
		public Halfedge<T> RightEnd {get{return rightEnd;}}

		public void Dispose() {
			Halfedge<T> halfedge = leftEnd;
			Halfedge<T> prevHe;
			while (halfedge != rightEnd) {
				prevHe = halfedge;
				halfedge = halfedge.edgeListRightNeighbor;
				prevHe.Dispose();
			}
			leftEnd = null;
			rightEnd.Dispose();
			rightEnd = null;

			hash = null;
		}

		public EdgeList(float xmin, float deltaX, int sqrtSitesNb) {
			this.xmin = xmin;
			this.deltaX = deltaX;
			hashSize = 2 * sqrtSitesNb;

			hash = new Halfedge<T>[hashSize];

			// Two dummy Halfedges:
			leftEnd = Halfedge<T>.CreateDummy();
			rightEnd = Halfedge<T>.CreateDummy();
			leftEnd.edgeListLeftNeighbor = null;
			leftEnd.edgeListRightNeighbor = rightEnd;
			rightEnd.edgeListLeftNeighbor = leftEnd;
			rightEnd.edgeListRightNeighbor = null;
			hash[0] = leftEnd;
			hash[hashSize - 1] = rightEnd;
		}

		/*
		 * Insert newHalfedge to the right of lb
		 * @param lb
		 * @param newHalfedge
		 */
		public void Insert(Halfedge<T> lb, Halfedge<T> newHalfedge) {
			newHalfedge.edgeListLeftNeighbor = lb;
			newHalfedge.edgeListRightNeighbor = lb.edgeListRightNeighbor;
			lb.edgeListRightNeighbor.edgeListLeftNeighbor = newHalfedge;
			lb.edgeListRightNeighbor = newHalfedge;
		}

		/*
		 * This function only removes the Halfedge from the left-right list.
		 * We cannot dispose it yet because we are still using it.
		 * @param halfEdge
		 */
		public void Remove(Halfedge<T> halfedge) {
			halfedge.edgeListLeftNeighbor.edgeListRightNeighbor = halfedge.edgeListRightNeighbor;
			halfedge.edgeListRightNeighbor.edgeListLeftNeighbor = halfedge.edgeListLeftNeighbor;
			halfedge.edge = Edge<T>.DELETED;
			halfedge.edgeListLeftNeighbor = halfedge.edgeListRightNeighbor = null;
		}

		/*
		 * Find the rightmost Halfedge that is still elft of p
		 * @param p
		 * @return
		 */
		public Halfedge<T> EdgeListLeftNeighbor(Vector2f p) {
			int bucket;
			Halfedge<T> halfedge;

			// Use hash table to get close to desired halfedge
			bucket = (int)((p.x - xmin)/deltaX * hashSize);
			if (bucket < 0) {
				bucket = 0;
			}
			if (bucket >= hashSize) {
				bucket = hashSize - 1;
			}
			halfedge = GetHash(bucket);
			if (halfedge == null) {
				for (int i = 0; true; i++) {
					if ((halfedge = GetHash(bucket - i)) != null) break;
					if ((halfedge = GetHash(bucket + i)) != null) break;
				}
			}
			// Now search linear list of haledges for the correct one
			if (halfedge == leftEnd || (halfedge != rightEnd && halfedge.IsLeftOf(p))) {
				do {
					halfedge = halfedge.edgeListRightNeighbor;
				} while (halfedge != rightEnd && halfedge.IsLeftOf(p));
				halfedge = halfedge.edgeListLeftNeighbor;

			} else {
				do {
					halfedge = halfedge.edgeListLeftNeighbor;
				} while (halfedge != leftEnd && !halfedge.IsLeftOf(p));
			}

			// Update hash table and reference counts
			if (bucket > 0 && bucket < hashSize - 1) {
				hash[bucket] = halfedge;
			}
			return halfedge;
		}

		// Get entry from the has table, pruning any deleted nodes
		private Halfedge<T> GetHash(int b) {
			Halfedge<T> halfedge;

			if (b < 0 || b >= hashSize) {
				return null;
			}
			halfedge = hash[b];
			if (halfedge != null && halfedge.edge == Edge<T>.DELETED) {
				// Hash table points to deleted halfedge. Patch as necessary
				hash[b] = null;
				// Still can't dispose halfedge yet!
				return null;
			} else {
				return halfedge;
			}
		}
	}
}
