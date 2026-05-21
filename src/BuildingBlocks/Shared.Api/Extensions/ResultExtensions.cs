using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Results;

namespace Shared.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToFailureActionResult(this ControllerBase controller, Result result)
    {
        return result.Error.Type switch
        {
            ErrorType.Validation => controller.BadRequest(new ApiResponse<string>(result.Error.Message)),
            ErrorType.Unauthorized => controller.Unauthorized(new ApiResponse<string>(result.Error.Message)),
            ErrorType.NotFound => controller.NotFound(new ApiResponse<string>(result.Error.Message)),
            ErrorType.Conflict => controller.Conflict(new ApiResponse<string>(result.Error.Message)),
            _ => controller.BadRequest(new ApiResponse<string>(result.Error.Message))
        };
    }
}
