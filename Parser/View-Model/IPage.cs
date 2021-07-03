using System.Collections.Generic;

namespace Parser
{
    interface IPage
    {
        string Url { get; }
        List<string> Words { get; }

        List<Statistics> Statistics { get; }
    }
}
