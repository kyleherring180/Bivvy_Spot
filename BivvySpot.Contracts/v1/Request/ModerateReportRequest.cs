using BivvySpot.Model.Enums;

namespace BivvySpot.Contracts.v1.Request;

public class ModerateReportRequest
{
    public ReportStatus Status { get; set; }
    public string? ModeratorNote { get; set; }
}