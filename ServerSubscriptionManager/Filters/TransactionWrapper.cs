using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using System.Data;

namespace ServerSubscriptionManager.Filters
{
    public class TransactionWrapper : IAsyncActionFilter
    {
        private readonly SubscriptionContext _context;

        public TransactionWrapper(SubscriptionContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            var executedContext = await next(); // executing the API controller action
            if (executedContext.Exception == null)
            {
                await transaction.CommitAsync(); // commit the transaction if no exceptions
            }
            else
            {
                await transaction.RollbackAsync(); // rollback the transaction if an exception occurred
            }
        }
    }
}
