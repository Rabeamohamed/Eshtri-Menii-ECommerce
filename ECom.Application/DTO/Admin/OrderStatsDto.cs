using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Application.DTO.Admin
{   
    // Order Stats
    public record OrderStatsDto
    {
        public int TotalOrders { get; init; }
        public int PendingOrders { get; init; }
        public int CompletedOrders { get; init; }
        public int CancelledOrders { get; init; }
        public int FailedOrders { get; init; }

    }
}
