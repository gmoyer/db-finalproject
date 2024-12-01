using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using System.Data;

namespace ServerSubscriptionManager.Filters
{
    public class TransactionWrapper(SubscriptionContext context) : IAsyncActionFilter
    {
        private readonly SubscriptionContext _context = context;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            var executedContext = await next(); // executing the API controller action

            // The status code indicates an error in the range [400, 600).
            var statusCodeError = executedContext.HttpContext.Response.StatusCode >= 400 && executedContext.HttpContext.Response.StatusCode < 600;

            if (executedContext.Exception == null && !statusCodeError)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }
        }
    }
}
