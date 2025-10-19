using AutoGestao.Models;

namespace AutoGestao.Extensions
{
    public static class StandardGridViewModelExtensions
    {
        /// <summary>
        /// Converte StandardGridViewModel<T> para StandardGridViewModel (não-genérico)
        /// </summary>
        public static StandardGridViewModel ToNonGeneric<T>(this StandardGridViewModel<T> source) where T : class
        {
            return new StandardGridViewModel
            {
                Items = [.. source.Items.Cast<object>()],
                CurrentPage = source.CurrentPage,
                PageSize = source.PageSize,
                TotalRecords = source.TotalRecords,
                OrderBy = source.OrderBy,
                OrderDirection = source.OrderDirection,
                Filters = source.Filters,
                Columns = source.Columns,
                RowActions = source.RowActions,
                Title = source.Title,
                SubTitle = source.SubTitle,
                Icon = source.Icon,
                CreateUrl = source.CreateUrl,
                EntityName = source.EntityName,
                AdditionalData = source.AdditionalData
            };
        }

        /// <summary>
        /// Converte StandardGridViewModel para StandardGridViewModel<T>
        /// </summary>
        public static StandardGridViewModel<T> ToGeneric<T>(this StandardGridViewModel source) where T : class
        {
            return new StandardGridViewModel<T>
            {
                Items = [.. source.Items.Cast<T>()],
                CurrentPage = source.CurrentPage,
                PageSize = source.PageSize,
                TotalRecords = source.TotalRecords,
                OrderBy = source.OrderBy,
                OrderDirection = source.OrderDirection,
                Filters = source.Filters,
                Columns = source.Columns,
                RowActions = source.RowActions,
                Title = source.Title,
                SubTitle = source.SubTitle,
                Icon = source.Icon,
                CreateUrl = source.CreateUrl,
                EntityName = source.EntityName,
                AdditionalData = source.AdditionalData
            };
        }
    }
}
