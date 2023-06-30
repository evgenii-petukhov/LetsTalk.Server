using LetsTalk.Server.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Services;

public abstract class GenericDataLayerService
{
    public static async Task<QuerySingleResponse<T>> GetSingleValueAsync<T>(
        IQueryable<T> query,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await query.FirstAsync(cancellationToken);

            return new QuerySingleResponse<T>(response);
        }
        catch (InvalidOperationException)
        {
            return new QuerySingleResponse<T>();
        }
    }
}
