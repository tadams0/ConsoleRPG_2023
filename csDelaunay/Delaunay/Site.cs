using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace csDelaunay {

	public class Site<T> : ICoord {

		private static Queue<Site<T>> pool = new Queue<Site<T>>();

		public static Site<T> Create(KeyValuePair<Vector2f, T> p, int index, float weigth) {
			if (pool.Count > 0) {
				return pool.Dequeue().Init(p, index, weigth);
			} else {
				return new Site<T>(p, index, weigth);
			}
		}

		public static void SortSites(List<Site<T>> sites) {
			sites.Sort(delegate(Site<T> s0, Site<T> s1) {
				int returnValue = Voronoi<T>.CompareByYThenX(s0,s1);
				
				int tempIndex;
				
				if (returnValue == -1) {
					if (s0.siteIndex > s1.SiteIndex) {
						tempIndex = s0.SiteIndex;
						s0.SiteIndex = s1.SiteIndex;
						s1.SiteIndex = tempIndex;
					}
				} else if (returnValue == 1) {
					if (s1.SiteIndex > s0.SiteIndex) {
						tempIndex = s1.SiteIndex;
						s1.SiteIndex = s0.SiteIndex;
						s0.SiteIndex = tempIndex;
					}
				}
				
				return returnValue;
			});
		}

		public int Compare(Site<T> s1, Site<T> s2) {
			return s1.CompareTo(s2);
		}

		public int CompareTo(Site<T> s1) {
			int returnValue = Voronoi<T>.CompareByYThenX(this,s1);

			int tempIndex;

			if (returnValue == -1) {
				if (this.siteIndex > s1.SiteIndex) {
					tempIndex = this.SiteIndex;
					this.SiteIndex = s1.SiteIndex;
					s1.SiteIndex = tempIndex;
				}
			} else if (returnValue == 1) {
				if (s1.SiteIndex > this.SiteIndex) {
					tempIndex = s1.SiteIndex;
					s1.SiteIndex = this.SiteIndex;
					this.SiteIndex = tempIndex;
				}
			}

			return returnValue;
		}
		
		private const float EPSILON = 0.005f;
		private static bool CloseEnough(Vector2f p0, Vector2f p1) {
			return (p0-p1).magnitude < EPSILON;
		}
		
		private int siteIndex;
		public int SiteIndex {get{return siteIndex;} set{siteIndex=value;}}
		
		private Vector2f coord;
		public Vector2f Coord {get{return coord;}set{coord=value;}}

		public float x {get{return coord.x;}}
		public float y {get{return coord.y;}}

		private float weight;
		public float Weight {get{return weight;}}

		private T data;

		public T Data { get { return data;}}

		// The edges that define this Site's Voronoi region:
		private List<Edge<T>> edges;
		public List<Edge<T>> Edges {get{return edges;}}
		// which end of each edge hooks up with the previous edge in edges:
		private List<LR> edgeOrientations;
		// ordered list of points that define the region clipped to bounds:
		private List<Vector2f> region;

		public Site(KeyValuePair<Vector2f, T> p, int index, float weigth) {
			Init(p, index, weigth);
		}

		private Site<T> Init(KeyValuePair<Vector2f, T> p, int index, float weigth) {
			coord = p.Key;
			data = p.Value;
			siteIndex = index;
			this.weight = weigth;
			edges = new List<Edge<T>>();
			region = null;

			return this;
		}

		public override string ToString() {
			return "Site " + siteIndex + ": " + coord;
		}

		private void Move(Vector2f p) {
			Clear();
			coord = p;
		}

		public void Dispose() {
			Clear();
			pool.Enqueue(this);
		}

		private void Clear() {
			if (edges != null) {
				edges.Clear();
				edges = null;
			}
			if (edgeOrientations != null) {
				edgeOrientations.Clear();
				edgeOrientations = null;
			}
			if (region != null) {
				region.Clear();
				region = null;
			}
		}

		public void AddEdge(Edge<T> edge) {
			edges.Add(edge);
		}

		public Edge<T> NearestEdge() {
			edges.Sort(Edge<T>.CompareSitesDistances);
			return edges[0];
		}

        /// <summary>
        /// Gets the edge with the least distance to the given point.
        /// </summary>
        /// <param name="P">The given point.</param>
		/// <param name="distance">The distance to the edge.</param>
        public Edge<T> GetClosestEdge(Vector2f P, out float distance)
        {
            Edge<T> closestEdge = edges[0];
            distance = float.MaxValue;
            float currentDistance;
            for (int i = 0; i < edges.Count; i++)
            {
				if (!edges[i].Visible())
					continue;

                currentDistance = GetDistanceToEdge2(P, edges[i]);

                if (currentDistance > 0 && distance > currentDistance)
                {
                    distance = currentDistance;
                    closestEdge = edges[i];
                }
            }

            return closestEdge;
        }

        /// <summary>
        /// Gets the edge with the least distance to the given point.
        /// </summary>
        /// <param name="P">The given point.</param>
        /* public Edge<T> GetClosestEdge(Vector2f P)
        {
			Edge<T> closestEdge = edges[0];
			float distance = float.MaxValue;
			float currentDistance;
			for (int i = 0; i < edges.Count; i++)
            {
                if (!edges[i].Visible())
                    continue;

                currentDistance = GetDistanceToEdge(P, edges[i]);

                if (distance > currentDistance)
				{
					distance = currentDistance;
					closestEdge = edges[i];
                }
            }
            return closestEdge;
        }*/


        /// <summary>
        /// Gets the distance to the given edge from an arbitruary point.
        /// </summary>
        /// <param name="P">The given point.</param>
        /// <param name="edge">The edge to determine distance to.</param>
        /// <returns></returns>
        public float GetDistanceToEdge2(Vector2f p, Edge<T> edge)
        {
			Vector2f v = edge.ClippedEnds.Left;
			Vector2f w = edge.ClippedEnds.Right;

			var l2 = Vector2f.DistanceSquare(v, w);
			if (l2 == 0) return Vector2f.DistanceSquare(p, v); ;
            var t = ((p.x - v.x) * (w.x - v.x) + (p.y - v.y) * (w.y - v.y)) / l2;
            t = Math.Max(0, Math.Min(1, t));
			return Vector2f.DistanceSquare(p, new Vector2f(v.x + t * (w.x - v.x),
															v.y + t * (w.y - v.y)));
        }

        public bool isLeft(Vector2f a, Edge<T> edge)
        {
            return ((edge.ClippedEnds.Left.x - a.x) * (edge.ClippedEnds.Right.y - a.y) - (edge.ClippedEnds.Left.y - a.y) * (edge.ClippedEnds.Right.x - a.x)) > 0;
        }

        public List<Site<T>> NeighborSites() {
			if (edges == null || edges.Count == 0) {
				return new List<Site<T>>();
			}
			if (edgeOrientations == null) {
				ReorderEdges();
			}
			List<Site<T>> list = new List<Site<T>>();
			foreach (Edge<T> edge in edges) {
				list.Add(NeighborSite(edge));
			}
			return list;
		}

		private Site<T> NeighborSite(Edge<T> edge) {
			if (this == edge.LeftSite) {
				return edge.RightSite;
			}
			if (this == edge.RightSite) {
				return edge.LeftSite;
			}
			return null;
		}

		public List<Vector2f> Region(Rectf clippingBounds) {
			if (edges == null || edges.Count == 0) {
				return new List<Vector2f>();
			}
			if (edgeOrientations == null) {
				ReorderEdges();
			}
			if (region == null) {
				region = ClipToBounds(clippingBounds);
				if ((new Polygon(region)).PolyWinding() == Winding.CLOCKWISE) {
					region.Reverse();
				}
			}
			return region;
		}

		private void ReorderEdges() {
			EdgeReorderer<T> reorderer = new EdgeReorderer<T>(edges, typeof(Vertex));
			edges = reorderer.Edges;
			edgeOrientations = reorderer.EdgeOrientations;
			reorderer.Dispose();
		}

		private List<Vector2f> ClipToBounds(Rectf bounds) {
			List<Vector2f> points = new List<Vector2f>();
			int n = edges.Count;
			int i = 0;
			Edge<T> edge;

			while (i < n && !edges[i].Visible()) {
				i++;
			}

			if (i == n) {
				// No edges visible
				return new List<Vector2f>();
			}
			edge = edges[i];
			LR orientation = edgeOrientations[i];
			points.Add(edge.ClippedEnds[orientation]);
			points.Add(edge.ClippedEnds[LR.Other(orientation)]);

			for (int j = i + 1; j < n; j++) {
				edge = edges[j];
				if (!edge.Visible()) {
					continue;
				}
				Connect(ref points, j, bounds);
			}
			// Close up the polygon by adding another corner point of the bounds if needed:
			Connect(ref points, i, bounds, true);

			return points;
		}

		private void Connect(ref List<Vector2f> points, int j, Rectf bounds, bool closingUp = false) {
			Vector2f rightPoint = points[points.Count-1];
			Edge<T> newEdge = edges[j];
			LR newOrientation = edgeOrientations[j];

			// The point that must be conected to rightPoint:
			Vector2f newPoint = newEdge.ClippedEnds[newOrientation];

			if (!CloseEnough(rightPoint, newPoint)) {
				// The points do not coincide, so they must have been clipped at the bounds;
				// see if they are on the same border of the bounds:
				if (rightPoint.x != newPoint.x && rightPoint.y != newPoint.y) {
					// They are on different borders of the bounds;
					// insert one or two corners of bounds as needed to hook them up:
					// (NOTE this will not be correct if the region should take up more than
					// half of the bounds rect, for then we will have gone the wrong way
					// around the bounds and included the smaller part rather than the larger)
					int rightCheck = BoundsCheck.Check(rightPoint, bounds);
					int newCheck = BoundsCheck.Check(newPoint, bounds);
					float px, py;
					if ((rightCheck & BoundsCheck.RIGHT) != 0) {
						px = bounds.right;

						if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							py = bounds.bottom;
							points.Add(new Vector2f(px,py));

						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							py = bounds.top;
							points.Add(new Vector2f(px,py));

						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							if (rightPoint.y - bounds.y + newPoint.y - bounds.y < bounds.height) {
								py = bounds.top;
							} else {
								py = bounds.bottom;
							}
							points.Add(new Vector2f(px,py));
							points.Add(new Vector2f(bounds.left, py));
						}
					} else if ((rightCheck & BoundsCheck.LEFT) != 0) {
						px = bounds.left;

						if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							py = bounds.bottom;
							points.Add(new Vector2f(px,py));

						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							py = bounds.top;
							points.Add(new Vector2f(px,py));

						} else if ((newCheck & BoundsCheck.RIGHT) != 0) {
							if (rightPoint.y - bounds.y + newPoint.y - bounds.y < bounds.height) {
								py = bounds.top;
							} else {
								py = bounds.bottom;
							}
							points.Add(new Vector2f(px,py));
							points.Add(new Vector2f(bounds.right, py));
						}
					} else if ((rightCheck & BoundsCheck.TOP) != 0) {
						py = bounds.top;

						if ((newCheck & BoundsCheck.RIGHT) != 0) {
							px = bounds.right;
							points.Add(new Vector2f(px,py));

						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							px = bounds.left;
							points.Add(new Vector2f(px,py));

						} else if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							if (rightPoint.x - bounds.x + newPoint.x - bounds.x < bounds.width) {
								px = bounds.left;
							} else {
								px = bounds.right;
							}
							points.Add(new Vector2f(px,py));
							points.Add(new Vector2f(px, bounds.bottom));
						}
					} else if ((rightCheck & BoundsCheck.BOTTOM) != 0) {
						py = bounds.bottom;
						
						if ((newCheck & BoundsCheck.RIGHT) != 0) {
							px = bounds.right;
							points.Add(new Vector2f(px,py));
							
						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							px = bounds.left;
							points.Add(new Vector2f(px,py));
							
						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							if (rightPoint.x - bounds.x + newPoint.x - bounds.x < bounds.width) {
								px = bounds.left;
							} else {
								px = bounds.right;
							}
							points.Add(new Vector2f(px,py));
							points.Add(new Vector2f(px, bounds.top));
						}
					}
				}
				if (closingUp) {
					// newEdge's ends have already been added
					return;
				}
				points.Add(newPoint);
			}
			Vector2f newRightPoint = newEdge.ClippedEnds[LR.Other(newOrientation)];
			if (!CloseEnough(points[0], newRightPoint)) {
				points.Add(newRightPoint);
			}
		}

		public float Dist(ICoord p) {
			return (this.Coord - p.Coord).magnitude;
		}
	}

	public class BoundsCheck {
		public const int TOP = 1;
		public const int BOTTOM = 2;
		public const int LEFT = 4;
		public const int RIGHT = 8;

		/*
		 * 
		 * @param point
		 * @param bounds
		 * @return an int with the appropriate bits set if the Point lies on the corresponding bounds lines
		 */
		public static int Check(Vector2f point, Rectf bounds) {
			int value = 0;
			if (point.x == bounds.left) {
				value |= LEFT;
			}
			if (point.x == bounds.right) {
				value |= RIGHT;
			}
			if (point.y == bounds.top) {
				value |= TOP;
			}
			if (point.y == bounds.bottom) {
				value |= BOTTOM;
			}

			return value;
		}
	}
}
