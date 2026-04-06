using System.Collections.Generic;

namespace KhoaNVCB_Client.Models // (Giữ nguyên namespace của cục cứt nếu nó khác nhé)
{
    public class SubmitQuizRequest
    {
        // 1. Mã của Phiên thi hiện tại (Để Admin biết bài này nộp vào hòm nào)
        public int SessionId { get; set; }

        // 2. Chuyên mục (Phục vụ cho việc thống kê điểm sau này)
        public int CategoryId { get; set; }

        // 3. Đập bỏ AccountId, thay bằng 3 thông tin thực tế của sinh viên
        public string FullName { get; set; } = "";
        public string StudentIdOrEmail { get; set; } = "";
        public string ClassName { get; set; } = "";

        // 4. Danh sách đáp án: [Mã Câu Hỏi] -> "A/B/C/D"
        public Dictionary<int, string> Answers { get; set; } = new();
    }
}