using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Dtos.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Application.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyQueryParameters<T>(this IQueryable<T> query, QueryParameters parameters)
        {
            // Filtering
            if (!string.IsNullOrEmpty(parameters.FilterBy) && !string.IsNullOrEmpty(parameters.FilterValue))
            {
                var propertyInfo = typeof(T).GetProperty(parameters.FilterBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    // Generate the lambda expression for filtering
                    query = query.Where(e =>
                        EF.Functions.Like(
                            EF.Property<string>(e, parameters.FilterBy),
                            $"%{parameters.FilterValue}%"));
                }
            }

            // Sorting
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                var propertyInfo = typeof(T).GetProperty(parameters.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    var parameter = Expression.Parameter(typeof(T), "e");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
                    var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
                    query = string.Equals(parameters.SortOrder, "desc", StringComparison.OrdinalIgnoreCase)
                        ? query.OrderByDescending(lambda)
                        : query.OrderBy(lambda);
                }
            }

            // Pagination
            query = query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize);

            return query;
        }

        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, QueryParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.FilterBy) && !string.IsNullOrEmpty(parameters.FilterValue))
            {
                // Special handling for navigation properties (e.g., Category.Name)
                if (parameters.FilterBy.Contains("."))
                {
                    var properties = parameters.FilterBy.Split('.');
                    var navigationProperty = properties[0]; // e.g., "Category"
                    var property = properties[1]; // e.g., "Name"

                    query = query.WhereDynamic(navigationProperty, property, parameters.FilterValue);
                }
                else
                {
                    // Handle direct properties
                    query = query.Where(e =>
                        EF.Functions.Like(EF.Property<string>(e, parameters.FilterBy), $"%{parameters.FilterValue}%"));
                }
            }

            // Additional search term filtering
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(e =>
                    EF.Functions.Like(EF.Property<string>(e, "Name"), $"%{parameters.SearchTerm}%"));
            }

            return query;
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, QueryParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                var isDescending = parameters.SortOrder?.ToLower() == "desc";

                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, parameters.SortBy))
                    : query.OrderBy(e => EF.Property<object>(e, parameters.SortBy));
            }

            return query;
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, QueryParameters parameters)
        {
            return query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize);
        }

        public static IQueryable<T> WhereDynamic<T>(
            this IQueryable<T> source,
            string navigationProperty,
            string property,
            string filterValue)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var navigationExpression = Expression.PropertyOrField(parameter, navigationProperty);
            var propertyExpression = Expression.PropertyOrField(navigationExpression, property);

            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) });
            var filterValueExpression = Expression.Constant($"%{filterValue}%");

            var likeExpression = Expression.Call(
                likeMethod!,
                Expression.Constant(EF.Functions),
                propertyExpression,
                filterValueExpression
            );

            var lambda = Expression.Lambda<Func<T, bool>>(likeExpression, parameter);

            return source.Where(lambda);
        }
    }
}
