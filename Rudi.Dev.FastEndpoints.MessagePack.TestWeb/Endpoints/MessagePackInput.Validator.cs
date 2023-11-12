using FastEndpoints;
using FluentValidation;

namespace Rudi.Dev.FastEndpoints.MessagePack.TestWeb.Endpoints;

public class MessagePackInputValidator : Validator<MessagePackInputRequest>
{
    public MessagePackInputValidator()
    {
        RuleFor(x => x.Test).NotEmpty();
    }
}