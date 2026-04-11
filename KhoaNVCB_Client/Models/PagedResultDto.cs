// Lưu ý: Đổi namespace thành KhoaNVCB_API.Dtos đối với Backend
namespace KhoaNVCB_Client.Models
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}