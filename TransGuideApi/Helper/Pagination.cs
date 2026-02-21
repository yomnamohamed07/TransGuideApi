namespace TransGuideApi.Helper
{
	public class Pagination<T>
	{
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public int Count { get; set; }
		public IReadOnlyList<T> Data { get; set; }
		public Pagination(int pageindex, int pagesize, IReadOnlyList<T> data, int count)
		{
			PageIndex = pageindex;
			PageSize = pagesize;
			Data = data;
			Count = count;

		}
	}
}
