using BivvySpot.Model.Enums;

namespace BivvySpot.Contracts.v1.Request;

public class AddInteractionRequest
{
    public InteractionType InteractionType { get; set; }
}
