using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Common.Models
{
    /// </summary>
    public class PaginatedResponse<T>
    {
        /// <summary>Mevcut sayfadaki veriler.</summary>
        public List<T> Items { get; set; } = new();

        /// <summary>Toplam kayıt sayısı (tüm sayfalar).</summary>
        public int TotalCount { get; set; }

        /// <summary>Sayfa numarası (1'den başlar).</summary>
        public int PageNumber { get; set; }

        /// <summary>Sayfa başına kayıt sayısı.</summary>
        public int PageSize { get; set; }

        /// <summary>Toplam sayfa sayısı.</summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>Önceki sayfa var mı?</summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>Sonraki sayfa var mı?</summary>
        public bool HasNextPage => PageNumber < TotalPages;

        public static PaginatedResponse<T> Create(
            List<T> items, int totalCount, int pageNumber, int pageSize)
            => new()
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
    }
}
