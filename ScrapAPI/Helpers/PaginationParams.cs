namespace ScrapAPI.Helpers;

public class PaginationParams
    {
        private const int MaxPerPage = 50;
        public int CurrentPage { get; set; } = 1;
        private int _pageSize = 1000;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPerPage) ? MaxPerPage : value;
        }
    }