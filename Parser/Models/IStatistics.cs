using System;

namespace Parser
{
    interface IStatistics
    {
        int Id { get; set; }
        string Url { get; set; }
        string Word { get; set; }
        int Count { get; set; }
    }
}
