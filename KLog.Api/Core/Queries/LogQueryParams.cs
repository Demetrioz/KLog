using KLog.DataModel.Entities;
using System;

namespace KLog.Api.Core.Queries
{
    public class LogQueryParams
    {
        private const int _maxPageSize = 100;
        private int _pageSize = 50;
        private int _page = 1;

        public string Source { get; set; }
        public bool MostRecent { get; set; } = true;
        public LogLevel? LogLevel { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? StopTime { get; set; }
        public int Page 
        { 
            get { return _page; }
            set
            {
                if(value < 1)
                    _page = 1;
                else 
                    _page = value;
            }
        }
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value > _maxPageSize)
                    _pageSize = _maxPageSize;
                else
                    _pageSize = value;
            }
        }
    }
}
