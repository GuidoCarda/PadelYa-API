using System;
using System.Collections.Generic;

namespace padelya_api.DTOs.Report
{
    public class ReportEcommerceDto
    {
        public EcommerceStatisticsDto Statistics { get; set; } = new();
        public List<DailySalesDto> DailySales { get; set; } = new();
        public List<StatusDistributionDto> StatusDistribution { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
    }

    public class EcommerceStatisticsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageTicket { get; set; }
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class StatusDistributionDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
