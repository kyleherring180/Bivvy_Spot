using BivvySpot.Model.Entities;
using BivvySpot.Model.Enums;

namespace BivvySpot.ModelTests;

public class ReportModelTests
{
    [Fact]
    public void Constructor_Sets_Defaults()
    {
        var postId = Guid.NewGuid();
        var reporterId = Guid.NewGuid();
        var report = new Report(postId, reporterId, "Reason");
        Assert.Equal(postId, report.PostId);
        Assert.Equal(reporterId, report.ReporterId);
        Assert.Equal("Reason", report.Reason);
        Assert.Equal(ReportStatus.Open, report.Status);
        Assert.Null(report.ResolvedBy);
        Assert.Null(report.ResolvedDate);
        Assert.NotEqual(default, report.CreatedDate);
                Assert.NotEqual(default, report.UpdatedDate);
    }

    [Fact]
    public void Moderate_Sets_Resolved_Fields_When_Not_Open()
    {
        var report = new Report(Guid.NewGuid(), Guid.NewGuid(), "X");
        var moderatorId = Guid.NewGuid();
        report.Moderate(ReportStatus.Resolved, moderatorId, "ok");
        Assert.Equal(ReportStatus.Resolved, report.Status);
        Assert.Equal("ok", report.ModeratorNote);
        Assert.Equal(moderatorId, report.ResolvedBy);
        Assert.NotNull(report.ResolvedDate);
    }

    [Fact]
    public void Moderate_Clears_Resolved_Fields_When_Set_To_Open()
    {
        var report = new Report(Guid.NewGuid(), Guid.NewGuid(), "X");
        report.Moderate(ReportStatus.Resolved, Guid.NewGuid(), "x");
        report.Moderate(ReportStatus.Open, Guid.NewGuid(), null);

        Assert.Equal(ReportStatus.Open, report.Status);
        Assert.Null(report.ResolvedBy);
        Assert.Null(report.ResolvedDate);
    }
}
