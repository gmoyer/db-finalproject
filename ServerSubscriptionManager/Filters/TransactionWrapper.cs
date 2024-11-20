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
            if (executedContext.Exception == null)
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
