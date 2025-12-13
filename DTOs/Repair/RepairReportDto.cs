namespace padelya_api.DTOs.Repair
{
  public class RepairReportDto
  {
    public RepairStatisticsDto Statistics { get; set; } = new();
    public List<DailyRepairDto> DailyRepairs { get; set; } = new();
    public List<RepairStatusDistributionDto> StatusDistribution { get; set; } = new();
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
  }

  public class RepairStatisticsDto
  {
    /// <summary>
    /// Total de reparaciones en el período
    /// </summary>
    public int TotalRepairs { get; set; }

    /// <summary>
    /// Reparaciones completadas (Delivered)
    /// </summary>
    public int CompletedRepairs { get; set; }

    /// <summary>
    /// Reparaciones pendientes (Received, InRepair, ReadyForPickup)
    /// </summary>
    public int PendingRepairs { get; set; }

    /// <summary>
    /// Reparaciones canceladas
    /// </summary>
    public int CancelledRepairs { get; set; }

    /// <summary>
    /// Total facturado por reparaciones completadas en el período
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Tiempo promedio de reparación en días (desde CreatedAt hasta FinishedAt)
    /// </summary>
    public decimal AverageRepairTimeDays { get; set; }

    /// <summary>
    /// Tiempo total de reparaciones en días
    /// </summary>
    public decimal TotalRepairTimeDays { get; set; }

    /// <summary>
    /// Ingreso promedio por reparación
    /// </summary>
    public decimal AverageRevenuePerRepair { get; set; }

    /// <summary>
    /// Tasa de completado (completadas / total * 100)
    /// </summary>
    public decimal CompletionRate { get; set; }

    /// <summary>
    /// Tasa de cancelación
    /// </summary>
    public decimal CancellationRate { get; set; }
  }

  public class DailyRepairDto
  {
    public string Date { get; set; } = string.Empty;
    public int RepairCount { get; set; }
    public decimal Revenue { get; set; }
  }

  public class RepairStatusDistributionDto
  {
    public string Status { get; set; } = string.Empty;
    public string DisplayStatus { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
  }

  public class MonthlyRevenueDto
  {
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int RepairCount { get; set; }
  }
}

