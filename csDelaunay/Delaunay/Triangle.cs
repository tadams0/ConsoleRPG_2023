using System.Collections;
using System.Collections.Generic;

namespace csDelaunay {

	public class Triangle<T>
    {

		private List<Site<T>> sites;
		public List<Site<T>> Sites {get{return sites;}}

		public Triangle(Site<T> a, Site<T> b, Site<T> c) {
			sites = new List<Site<T>>();
			sites.Add(a);
			sites.Add(b);
			sites.Add(c);
		}

		public void Dispose() {
			sites.Clear();
		}
	}
}