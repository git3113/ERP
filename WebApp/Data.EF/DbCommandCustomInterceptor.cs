using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util;

namespace Data.EF
{
    /// <summary>
    /// Sql执行拦截器
    /// </summary>
    public class DbCommandCustomInterceptor : DbCommandInterceptor
    {
        public async override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var obj = await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
            return obj;
        }

        public async override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds >= GlobalContext.SystemConfig.DBSlowSqlLogTime * 1000)
            {
                LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            int val = await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
            return val;
        }

        public async override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            var obj = await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
            return obj;
        }

        public async override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds >= GlobalContext.SystemConfig.DBSlowSqlLogTime * 1000)
            {
                LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            var obj = await base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
            return obj;
        }

        public async override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            var obj = await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
            return obj;
        }

        public async override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds >= GlobalContext.SystemConfig.DBSlowSqlLogTime * 1000)
            {
                LogHelper.Warn("耗时的Sql：" + command.GetCommandText());
            }
            var reader = await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
            return reader;
        }
    }
}
