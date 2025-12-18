using System.Linq.Dynamic.Core;
using ThuocGiaThatAdmin.Contract.DTOs;

namespace ThuocGiaThat.Infrastucture.Utils
{
    public class DynamicFilterService
    {
        public IQueryable<T> ApplyFilters<T>(IQueryable<T> query, FilterRequest request)
        {
            if (request?.Filters == null || !request.Filters.Any())
                return query;

            var whereClause = BuildWhereClause(request.Filters, request.Logic);
            var parameters = BuildParameters(request.Filters);

            if (!string.IsNullOrEmpty(whereClause))
            {
                query = query.Where(whereClause, parameters);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortDirection = request.SortOrder.ToLower() == "desc" ? "descending" : "ascending";
                query = query.OrderBy($"{request.SortBy} {sortDirection}");
            }

            return query;
        }

        private string BuildWhereClause(List<FilterCondition> filters, string logic)
        {
            var conditions = new List<string>();

            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];
                var condition = BuildCondition(filter, i);
                if (!string.IsNullOrEmpty(condition))
                    conditions.Add(condition);
            }

            if (!conditions.Any())
                return string.Empty;

            var logicOperator = logic.ToLower() == "or" ? " || " : " && ";
            return string.Join(logicOperator, conditions.Select(c => $"({c})"));
        }

        private string BuildCondition(FilterCondition filter, int index)
        {
            return filter.Operator.ToLower() switch
            {
                "eq" => $"{filter.Field} == @{index}",
                "ne" => $"{filter.Field} != @{index}",
                "gt" => $"{filter.Field} > @{index}",
                "lt" => $"{filter.Field} < @{index}",
                "gte" => $"{filter.Field} >= @{index}",
                "lte" => $"{filter.Field} <= @{index}",
                "contains" => $"{filter.Field}.Contains(@{index})",
                "startswith" => $"{filter.Field}.StartsWith(@{index})",
                "endswith" => $"{filter.Field}.EndsWith(@{index})",
                "in" => $"@{index}.Contains({filter.Field})",
                "isnull" => $"{filter.Field} == null",
                "isnotnull" => $"{filter.Field} != null",
                _ => string.Empty
            };
        }

        private object[] BuildParameters(List<FilterCondition> filters)
        {
            return filters.Select(f => ConvertJsonElementToPrimitive(f.Value)).ToArray();
        }
        private object ConvertJsonElementToPrimitive(object value, Type? targetType = null)
        {
            if (value is System.Text.Json.JsonElement je)
            {
                switch (je.ValueKind)
                {
                    case System.Text.Json.JsonValueKind.Number:
                        if (targetType == typeof(int))
                        {
                            return je.GetInt32();
                        }
                        if (targetType == typeof(long))
                        {
                            return je.GetInt64();
                        }
                        if (targetType == typeof(decimal))
                        {
                            return je.GetDecimal();
                        }
                        if (je.TryGetInt32(out var i)) return i;
                        if (je.TryGetInt64(out var l)) return l;
                        if (je.TryGetDecimal(out var d)) return d;
                        if (je.TryGetDouble(out var dbl)) return dbl;
                        break;
                    case System.Text.Json.JsonValueKind.String:
                        var s = je.GetString();
                        if (targetType != null && targetType != typeof(string))
                        {
                            // try parse to targetType (int, DateTime, enum, ...)
                            return Convert.ChangeType(s, targetType);
                        }
                        return s;
                    case System.Text.Json.JsonValueKind.True:
                    case System.Text.Json.JsonValueKind.False:
                        return je.GetBoolean();
                    case System.Text.Json.JsonValueKind.Null:
                        return null!;
                }
            }
            return value;
        }
    }
}
