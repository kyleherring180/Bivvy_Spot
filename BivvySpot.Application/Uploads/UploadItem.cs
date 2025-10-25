namespace BivvySpot.Application.Uploads;

public sealed record UploadItem(
    string FileName,
    string ContentType,
    long Length,
    Func<Stream> OpenReadStream
);