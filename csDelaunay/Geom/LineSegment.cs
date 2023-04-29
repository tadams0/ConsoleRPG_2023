using System.Collections;
using System.Collections.Generic;

namespace csDelaunay {

	public class LineSegment<T>
    {

		public static List<LineSegment<T>> VisibleLineSegments(List<Edge<T>> edges) {
			List<LineSegment<T>> segments = new List<LineSegment<T>>();

			foreach (Edge<T> edge in edges) {
				if (edge.Visible()) {
					Vector2f p1 = edge.ClippedEnds[LR.LEFT];
					Vector2f p2 = edge.ClippedEnds[LR.RIGHT];
					segments.Add(new LineSegment<T>(p1,p2));
				}
			}

			return segments;
		}

		public static float CompareLengths_MAX(LineSegment<T> segment0, LineSegment<T> segment1) {
			float length0 = (segment0.p0 - segment0.p1).magnitude;
			float length1 = (segment1.p0 - segment1.p1).magnitude;
			if (length0 < length1) {
				return 1;
			}
			if (length0 > length1) {
				return -1;
			}
			return 0;
		}

		public static float CompareLengths(LineSegment<T> edge0, LineSegment<T> edge1) {
			return - CompareLengths_MAX(edge0, edge1);
		}

		public Vector2f p0;
		public Vector2f p1;

		public LineSegment (Vector2f p0, Vector2f p1) {
			this.p0 = p0;
			this.p1 = p1;
		}
	}
}