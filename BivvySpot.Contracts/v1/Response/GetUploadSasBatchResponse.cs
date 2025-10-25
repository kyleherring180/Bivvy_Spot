namespace BivvySpot.Contracts.v1.Response;

public record GetUploadSasBatchResponse(IReadOnlyList<GetUploadSasResponse> Items);