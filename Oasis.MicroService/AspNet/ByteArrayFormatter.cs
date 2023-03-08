namespace Oasis.MicroService;

using Microsoft.AspNetCore.Mvc.Formatters;

public sealed class ByteArrayInputFormatter : InputFormatter
{
    public ByteArrayInputFormatter()
    {
        SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream"));
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var stream = new MemoryStream();
        await context.HttpContext.Request.Body.CopyToAsync(stream);
        return await InputFormatterResult.SuccessAsync(stream.ToArray());
    }

    protected override bool CanReadType(Type? type)
    {
        return type == typeof(byte[]);
    }
}