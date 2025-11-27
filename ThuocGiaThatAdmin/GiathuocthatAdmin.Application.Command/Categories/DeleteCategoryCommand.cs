using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Common.Interfaces;

namespace ThuocGiaThatAdmin.Commands.Categories
{
    /// <summary>
    /// Command to delete a category
    /// </summary>
    public class DeleteCategoryCommand : ICommand<DeleteCategoryResponse>
    {
        public int Id { get; set; }
    }

    public class DeleteCategoryResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int AffectedRows { get; set; }
    }
}
