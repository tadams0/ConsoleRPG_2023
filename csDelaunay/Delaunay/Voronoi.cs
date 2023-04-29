using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace csDelaunay {

	public class Voronoi<T> {

		private SiteList<T> sites;
		private List<Triangle<T>> triangles;

		private List<Edge<T>> edges;
		public List<Edge<T>> Edges {get{return edges;}}

		// TODO generalize this so it doesn't have to be a rectangle;
		// then we can make the fractal voronois-within-voronois
		private Rectf plotBounds;
		public Rectf PlotBounds {get{return plotBounds;}}
		
		private Dictionary<Vector2f,Site<T>> sitesIndexedByLocation;
		public Dictionary<Vector2f,Site<T>> SitesIndexedByLocation {get{return sitesIndexedByLocation;}}

		private Random weigthDistributor;

		public void Dispose() {
			sites.Dispose();
			sites = null;

			foreach (Triangle<T> t in triangles) {
				t.Dispose();
			}
			triangles.Clear();

			foreach (Edge<T> e in edges) {
				e.Dispose();
			}
			edges.Clear();

			plotBounds = Rectf.zero;
			sitesIndexedByLocation.Clear();
			sitesIndexedByLocation = null;
		}

		public Voronoi(List<KeyValuePair<Vector2f, T>> points, Rectf plotBounds) {
			weigthDistributor = new Random();
			Init(points,plotBounds);
		}

		public Voronoi(List<KeyValuePair<Vector2f, T>> points, Rectf plotBounds, int lloydIterations) {
			weigthDistributor = new Random();
			Init(points,plotBounds);
			LloydRelaxation(lloydIterations);
		}

		private void Init(List<KeyValuePair<Vector2f, T>> points, Rectf plotBounds) {
			sites = new SiteList<T>();
			sitesIndexedByLocation = new Dictionary<Vector2f, Site<T>>();
			AddSites(points);
			this.plotBounds = plotBounds;
			triangles = new List<Triangle<T>>();
			edges = new List<Edge<T>>();
			
			FortunesAlgorithm();
		}

		private void AddSites(List<KeyValuePair<Vector2f, T>> points) {
			for (int i = 0; i < points.Count; i++) {
				AddSite(points[i], i);
			}
		}

		private void AddSite(KeyValuePair<Vector2f, T> p, int index) {
			float weigth = (float)weigthDistributor.NextDouble() * 100;
			Site<T> site = Site<T>.Create(p, index, weigth);
			sites.Add(site);
			sitesIndexedByLocation[p.Key] = site;
		}

		public List<Vector2f> Region (Vector2f p) {
			Site<T> site;
			if (sitesIndexedByLocation.TryGetValue(p, out site)) {
				return site.Region(plotBounds);
			} else {
				return new List<Vector2f>();
			}
		}

		public List<Vector2f> NeighborSitesForSite(Vector2f coord) {
			List<Vector2f> points = new List<Vector2f>();
			Site<T> site;
			if (sitesIndexedByLocation.TryGetValue(coord, out site)) {
				List<Site<T>> sites = site.NeighborSites();
				foreach (Site<T> neighbor in sites) {
					points.Add(neighbor.Coord);
				}
			}

			return points;
		}

		public List<Circle> Circles() {
			return sites.Circles();
		}

		public List<LineSegment<T>> VoronoiBoundarayForSite(Vector2f coord) {
			return LineSegment<T>.VisibleLineSegments(Edge<T>.SelectEdgesForSitePoint(coord, edges));
		}
		/*
		public List<LineSegment> DelaunayLinesForSite(Vector2f coord) {
			return DelaunayLinesForEdges(Edge.SelectEdgesForSitePoint(coord, edges));
		}*/

		public List<LineSegment<T>> VoronoiDiagram() {
			return LineSegment<T>.VisibleLineSegments(edges);
		}
		/*
		public List<LineSegment> Hull() {
			return DelaunayLinesForEdges(HullEdges());
		}*/

		/// <summary>
		/// Searches through all sites within this instance to find the site with the least distance to the given point.
		/// </summary>
		/// <param name="p">The given point.</param>
		/// <returns>The site that was closest to the given point.</returns>
		public Site<T> FindClosestSite(Vector2f p)
		{
			Site<T> site = null;
			float closestDistance = float.MaxValue;
			float currentDistance;
            foreach (var kvp in sitesIndexedByLocation)
            {
				currentDistance = p.DistanceSquare(kvp.Key);
				if (currentDistance < closestDistance)
				{
					closestDistance = currentDistance;
					site = kvp.Value;
				}
            }
			return site;
        }


        /// <summary>
        /// Searches through the whole Voronoi diagram for the closest site.
        /// </summary>
        /// <param name="point">A point anywhere within the voronoi diagram.</param>
        /// <returns>Nearest site by distance squared.</returns>
        /*
		public Site FindClosestSite(Vector2f point)
		{
			Site closestSite = null;
			float nearestDist = float.MaxValue;
			float currentDist;
			foreach (Site site in sites.GetAllSites())
			{
				currentDist = point.DistanceSquare(site.Coord);

                if (currentDist <  nearestDist)
				{
					nearestDist = currentDist;
                    closestSite = site;
                }

            }

			return closestSite;
		}*/

        public List<Edge<T>> HullEdges() {
			return edges.FindAll(edge=>edge.IsPartOfConvexHull());
		}

		public List<Vector2f> HullPointsInOrder() {
			List<Edge<T>> hullEdges = HullEdges();

			List<Vector2f> points = new List<Vector2f>();
			if (hullEdges.Count == 0) {
				return points;
			}

			EdgeReorderer<T> reorderer = new EdgeReorderer<T>(hullEdges, typeof(Site<T>));
			hullEdges = reorderer.Edges;
			List<LR> orientations = reorderer.EdgeOrientations;
			reorderer.Dispose();

			LR orientation;
			for (int i = 0; i < hullEdges.Count; i++) {
				Edge<T> edge = hullEdges[i];
				orientation = orientations[i];
				points.Add(edge.Site(orientation).Coord);
			}
			return points;
		}

		public List<List<Vector2f>> Regions() {
			return sites.Regions(plotBounds);
		}

		public List<Vector2f> SiteCoords() {
			return sites.SiteCoords();
		}

		private void FortunesAlgorithm() {
			Site<T> newSite, bottomSite, topSite, tempSite;
			Vertex v, vertex;
			Vector2f newIntStar = Vector2f.zero;
			LR leftRight;
			Halfedge<T> lbnd, rbnd, llbnd, rrbnd, bisector;
			Edge<T> edge;

			Rectf dataBounds = sites.GetSitesBounds();

			int sqrtSitesNb = (int)Math.Sqrt(sites.Count() + 4);
			HalfedgePriorityQueue<T> heap = new HalfedgePriorityQueue<T>(dataBounds.y, dataBounds.height, sqrtSitesNb);
			EdgeList<T> edgeList = new EdgeList<T>(dataBounds.x, dataBounds.width, sqrtSitesNb);
			List<Halfedge<T>> halfEdges = new List<Halfedge<T>>();
			List<Vertex> vertices = new List<Vertex>();

			Site<T> bottomMostSite = sites.Next();
			newSite = sites.Next();

			while (true) {
				if (!heap.Empty()) {
					newIntStar = heap.Min();
				}

				if (newSite != null &&
				    (heap.Empty() || CompareByYThenX(newSite, newIntStar) < 0)) {
					// New site is smallest
					//Debug.Log("smallest: new site " + newSite);

					// Step 8:
					lbnd = edgeList.EdgeListLeftNeighbor(newSite.Coord);	// The halfedge just to the left of newSite
					//UnityEngine.Debug.Log("lbnd: " + lbnd);
					rbnd = lbnd.edgeListRightNeighbor;		// The halfedge just to the right
					//UnityEngine.Debug.Log("rbnd: " + rbnd);
					bottomSite = RightRegion(lbnd, bottomMostSite);			// This is the same as leftRegion(rbnd)
					// This Site determines the region containing the new site
					//UnityEngine.Debug.Log("new Site is in region of existing site: " + bottomSite);

					// Step 9
					edge = Edge<T>.CreateBisectingEdge(bottomSite, newSite);
					//UnityEngine.Debug.Log("new edge: " + edge);
					edges.Add(edge);

					bisector = Halfedge<T>.Create(edge, LR.LEFT);
					halfEdges.Add(bisector);
					// Inserting two halfedges into edgelist constitutes Step 10:
					// Insert bisector to the right of lbnd:
					edgeList.Insert(lbnd, bisector);

					// First half of Step 11:
					if ((vertex = Vertex.Intersect(lbnd, bisector)) != null) {
						vertices.Add(vertex);
						heap.Remove(lbnd);
						lbnd.vertex = vertex;
						lbnd.ystar = vertex.y + newSite.Dist(vertex);
						heap.Insert(lbnd);
					}

					lbnd = bisector;
					bisector = Halfedge<T>.Create(edge, LR.RIGHT);
					halfEdges.Add(bisector);
					// Second halfedge for Step 10::
					// Insert bisector to the right of lbnd:
					edgeList.Insert(lbnd, bisector);

					// Second half of Step 11:
					if ((vertex = Vertex.Intersect(bisector, rbnd)) != null) {
						vertices.Add(vertex);
						bisector.vertex = vertex;
						bisector.ystar = vertex.y + newSite.Dist(vertex);
						heap.Insert(bisector);
					}

					newSite = sites.Next();
				} else if (!heap.Empty()) {
					// Intersection is smallest
					lbnd = heap.ExtractMin();
					llbnd = lbnd.edgeListLeftNeighbor;
					rbnd = lbnd.edgeListRightNeighbor;
					rrbnd = rbnd.edgeListRightNeighbor;
					bottomSite = LeftRegion(lbnd, bottomMostSite);
					topSite = RightRegion(rbnd, bottomMostSite);
					// These three sites define a Delaunay triangle
					// (not actually using these for anything...)
					// triangles.Add(new Triangle(bottomSite, topSite, RightRegion(lbnd, bottomMostSite)));

					v = lbnd.vertex;
					v.SetIndex();
					lbnd.edge.SetVertex(lbnd.leftRight, v);
					rbnd.edge.SetVertex(rbnd.leftRight, v);
					edgeList.Remove(lbnd);
					heap.Remove(rbnd);
					edgeList.Remove(rbnd);
					leftRight = LR.LEFT;
					if (bottomSite.y > topSite.y) {
						tempSite = bottomSite;
						bottomSite = topSite;
						topSite = tempSite;
						leftRight = LR.RIGHT;
					}
					edge = Edge<T>.CreateBisectingEdge(bottomSite, topSite);
					edges.Add(edge);
					bisector = Halfedge<T>.Create(edge, leftRight);
					halfEdges.Add(bisector);
					edgeList.Insert(llbnd, bisector);
					edge.SetVertex(LR.Other(leftRight), v);
					if ((vertex = Vertex.Intersect(llbnd, bisector)) != null) {
						vertices.Add(vertex);
						heap.Remove(llbnd);
						llbnd.vertex = vertex;
						llbnd.ystar = vertex.y + bottomSite.Dist(vertex);
						heap.Insert(llbnd);
					}
					if ((vertex = Vertex.Intersect(bisector, rrbnd)) != null) {
						vertices.Add(vertex);
						bisector.vertex = vertex;
						bisector.ystar = vertex.y + bottomSite.Dist(vertex);
						heap.Insert(bisector);
					}
				} else {
					break;
				}
			}

			// Heap should be empty now
			heap.Dispose();
			edgeList.Dispose();

			foreach (Halfedge<T> halfedge in halfEdges) {
				halfedge.ReallyDispose();
			}
			halfEdges.Clear();

			// we need the vertices to clip the edges
			foreach (Edge<T> e in edges) {
				e.ClipVertices(plotBounds);
			}
			// But we don't actually ever use them again!
			foreach (Vertex ve in vertices) {
				ve.Dispose();
			}
			vertices.Clear();
		}

		public void LloydRelaxation(int nbIterations) {
			// Reapeat the whole process for the number of iterations asked
			for (int i = 0; i < nbIterations; i++) {
				List<KeyValuePair<Vector2f,T>> newPoints = new List<KeyValuePair<Vector2f, T>>();
				// Go thourgh all sites
				sites.ResetListIndex();
				Site<T> site = sites.Next();

				while (site != null) {
					// Loop all corners of the site to calculate the centroid
					List<Vector2f> region = site.Region(plotBounds);
					if (region.Count < 1) {
						site = sites.Next();
						continue;
					}
					
					Vector2f centroid = Vector2f.zero;
					float signedArea = 0;
					float x0 = 0;
					float y0 = 0;
					float x1 = 0;
					float y1 = 0;
					float a = 0;
					// For all vertices except last
					for (int j = 0; j < region.Count-1; j++) {
						x0 = region[j].x;
						y0 = region[j].y;
						x1 = region[j+1].x;
						y1 = region[j+1].y;
						a = x0*y1 - x1*y0;
						signedArea += a;
						centroid.x += (x0 + x1)*a;
						centroid.y += (y0 + y1)*a;
					}
					// Do last vertex
					x0 = region[region.Count-1].x;
					y0 = region[region.Count-1].y;
					x1 = region[0].x;
					y1 = region[0].y;
					a = x0*y1 - x1*y0;
					signedArea += a;
					centroid.x += (x0 + x1)*a;
					centroid.y += (y0 + y1)*a;

					signedArea *= 0.5f;
					centroid.x /= (6*signedArea);
					centroid.y /= (6*signedArea);
					// Move site to the centroid of its Voronoi cell
					newPoints.Add(new KeyValuePair<Vector2f, T>(centroid, site.Data));
					site = sites.Next();
				}

				// Between each replacement of the cendroid of the cell,
				// we need to recompute Voronoi diagram:
				Rectf origPlotBounds = this.plotBounds;
				Dispose();
				Init(newPoints,origPlotBounds);
			}
		}

		private Site<T> LeftRegion(Halfedge<T> he, Site<T> bottomMostSite) {
			Edge<T> edge = he.edge;
			if (edge == null) {
				return bottomMostSite;
			}
			return edge.Site(he.leftRight);
		}
		
		private Site<T> RightRegion(Halfedge<T> he, Site<T> bottomMostSite) {
			Edge<T> edge = he.edge;
			if (edge == null) {
				return bottomMostSite;
			}
			return edge.Site(LR.Other(he.leftRight));
		}

		public static int CompareByYThenX(Site<T> s1, Site<T> s2) {
			if (s1.y < s2.y) return -1;
			if (s1.y > s2.y) return 1;
			if (s1.x < s2.x) return -1;
			if (s1.x > s2.x) return 1;
			return 0;
		}
		
		public static int CompareByYThenX(Site<T> s1, Vector2f s2) {
			if (s1.y < s2.y) return -1;
			if (s1.y > s2.y) return 1;
			if (s1.x < s2.x) return -1;
			if (s1.x > s2.x) return 1;
			return 0;
		}

	}
}
