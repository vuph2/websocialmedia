================================================================================
          TÀI LIỆU CHI TIẾT CÁC CHỨC NĂNG HỆ THỐNG (SYSTEM FEATURES GUIDE)
                    WEBSITE MẠNG XÃ HỘI NEXUS - CẨM NANG CHI TIẾT
         (SYSTEM FEATURES & SOURCE CODE REQUIREMENTS MAPPING DIRECTORY)
================================================================================
Tài liệu này cung cấp danh sách đầy đủ toàn bộ các chức năng (chức năng) trong hệ thống 
mạng xã hội Nexus, giải thích chi tiết:
1. Cách hoạt động (How it works)
2. Nơi hoạt động (Where it works) - các file Controller, View, Model, Table CSDL, CSS, JS liên quan.
3. Tại sao hoạt động (Why it works) - cơ chế hoạt động kỹ thuật, truy vấn EF Core, sự kiện SignalR,
   JS AJAX, cấu hình Fluent API, v.v.
Tài liệu này là sự kết hợp đồng bộ giữa Danh sách Chức năng Hệ thống và Bản đồ 
Ánh xạ Mã nguồn. Giúp các thành viên trong nhóm phát triển nắm bắt nhanh:
- Mã yêu cầu (FR, NFR, BR, UC, AC)
- Cách hoạt động (How it works)
- Nơi hoạt động (Where it works) - file, lớp, hàm, dòng code.
- Tại sao hoạt động (Why it works) - cơ chế hoạt động, truy vấn EF Core, SignalR, v.v.
--------------------------------------------------------------------------------
MỤC LỤC CHƯƠNG (MODULES INDEX)
--------------------------------------------------------------------------------
[MODULE 01] Xác thực & Quản lý tài khoản (FR01, FR02, UC01, BR01, BR02, AC01, AC02, NFR02)
[MODULE 02] Bảng tin hệ thống & Tải trang vô hạn (FR05, NFR01, AC08)
[MODULE 03] Đăng bài viết & Đính kèm đa định dạng (FR04, UC02, AC03, NFR07)
[MODULE 04] Tương tác: Thích & Bình luận bài viết (FR06, FR07, BR05, AC04)
[MODULE 05] Hệ thống Bình chọn / Khảo sát (Polls & AJAX Voting)
[MODULE 06] Bản tin ngắn 24 giờ (Stories Carousel & Viewer)
[MODULE 07] Kết bạn & Theo dõi người dùng (FR09, AC05)
[MODULE 08] Trang cá nhân & Thiết lập (FR03, BR04)
[MODULE 09] Nhắn tin trực tiếp thời gian thực (FR08, UC03, AC05)
[MODULE 10] Thông báo thời gian thực (FR10)
[MODULE 11] Báo cáo vi phạm & Hàng đợi kiểm duyệt (FR12, FR13, UC04, BR03, BR07)
[MODULE 12] Bảng quản trị Admin & Quản lý thành viên (FR14, FR15, FR16, BR06)
[MODULE 13] Sao lưu dữ liệu hệ thống (FR17, NFR06, AC07)
[MODULE 14] Khởi tạo cơ sở dữ liệu & Ánh xạ (Database & EF Core Mapping)
================================================================================
CHỨC NĂNG 1: XÁC THỰC, ĐĂNG KÝ & ĐĂNG NHẬP (AUTHENTICATION & ACCOUNT MANAGEMENT)
[MODULE 01] XÁC THỰC & QUẢN LÝ TÀI KHẢN (AUTHENTICATION & ACCOUNT)
================================================================================
- MÃ YÊU CẦU: FR01 (Đăng ký), FR02 (Đăng nhập), UC01 (Use Case Đăng ký), BR01 (Email độc nhất), BR02 (Mật khẩu ≥ 8 ký tự), AC01 (Nghiệm thu Đăng ký), AC02 (Nghiệm thu Đăng nhập), NFR02 (Mã hóa mật khẩu)
- CÁCH HOẠT ĐỘNG (How it works):
  * Người dùng có thể đăng ký tài khoản mới bằng cách cung cấp Email, Username, Password, First Name, Last Name.
  * Hệ thống hỗ trợ đăng nhập đa luồng (Multi-Credential Login): Người dùng nhập Username hoặc Email đều đăng nhập được.
  * Hỗ trợ đăng nhập bên thứ ba (Google OAuth): Cho phép liên kết và đăng nhập bằng tài khoản Google.
  * Cho phép người dùng đã đăng nhập thay đổi mật khẩu của mình hoặc đăng xuất khỏi hệ thống.
  * Người dùng tạo tài khoản mới bằng cách cung cấp thông tin cá nhân và mật khẩu tối thiểu 8 ký tự. Hệ thống mã hóa mật khẩu trước khi lưu.
  * Người dùng có thể đăng nhập bằng Email hoặc Username. Cho phép liên kết đăng nhập qua Google OAuth.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/AccountController.cs (Các Action: Login, LoginPost, RegisterPost, GoogleLogin, GoogleCallback, Logout, ChangePassword)
  * Views:
    - web/Views/Account/Login.cshtml (Giao diện đăng nhập)
    - web/Views/Account/Register.cshtml (Giao diện đăng ký)
    - web/Views/Account/ChangePassword.cshtml (Giao diện đổi mật khẩu)
  * CSDL (Database Tables):
    - AspNetUser (Bảng chứa thông tin tài khoản)
    - AspNetUserLogin (Lưu liên kết tài khoản Google OAuth với AspNetUser)
    - AspNetUserClaim (Lưu trữ các Claim của phiên đăng nhập)
  * Controller xử lý: web/Controllers/AccountController.cs
    - Đăng ký: RegisterPost (Dòng 83-135)
    - Đăng nhập: LoginPost (Dòng 39-81)
    - Google OAuth: GoogleLogin (Dòng 137-142) & GoogleCallback (Dòng 152-231)
  * Cấu hình nghiệp vụ mật khẩu: web/Program.cs (Dòng 46)
  * Giao diện (Views): web/Views/Account/Login.cshtml, web/Views/Account/Register.cshtml
  * Bảng Cơ sở dữ liệu: AspNetUser (Lưu thông tin cốt lõi), AspNetUserLogin (Liên kết OAuth Google)
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * ASP.NET Core Identity Framework (`UserManager<ApplicationUser>` và `SignInManager<ApplicationUser>`) tự động quản lý cookie phiên, mã hóa mật khẩu bằng thuật toán băm bảo mật (PBKDF2).
  * Trong `LoginPost`, hệ thống tìm kiếm tài khoản theo Email trước bằng `_userManager.FindByEmailAsync(model.EmailOrUsername)`. Nếu không thấy, hệ thống tiếp tục tìm kiếm theo Username qua `_userManager.FindByNameAsync(model.EmailOrUsername)`. Sau khi tìm thấy user tương ứng, hệ thống tiến hành kiểm tra mật khẩu bằng `_signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, ...)`.
  * Google OAuth hoạt động bằng cách chuyển hướng đến máy chủ xác thực của Google thông qua cấu hình `ChallengeResult("Google", redirectUrl)`. Khi Google xác thực thành công và callback về `GoogleCallback`, hệ thống kiểm tra email phản hồi, nếu chưa có tài khoản thì tạo mới và tự động liên kết tài khoản thông qua `_userManager.AddLoginAsync()`.
  * ASP.NET Core Identity tự động quản lý phiên làm việc bằng Cookie và mã hóa mật khẩu bằng thuật toán băm PBKDF2 bảo mật cao.
  * Action LoginPost sử dụng kiểm tra Email trước, nếu null thì tìm kiếm theo Username, tạo ra trải nghiệm đăng nhập đa luồng cực kỳ thân thiện.
================================================================================
CHỨC NĂNG 2: BẢNG TIN HỆ THỐNG & TỰ ĐỘNG PHÂN TRANG (SYSTEM FEED & INFINITE SCROLL)
[MODULE 02] BẢNG TIN HỆ THỐNG & TẢI TRANG ĐỘNG (SYSTEM FEED & INFINITE SCROLL)
================================================================================
- MÃ YÊU CẦU: FR05 (Xem bảng tin), NFR01 (Hiệu năng tải trang < 3s), AC08 (Hoạt động ổn định)
- CÁCH HOẠT ĐỘNG (How it works):
  * Hiển thị danh sách các bài viết của người dùng trên trang chủ.
  * Tự động lọc các bài viết dựa trên cài đặt quyền riêng tư (Public - tất cả mọi người, Followers - chỉ những người theo dõi mới xem được, Private - chỉ tác giả xem được).
  * Tự động lọc sạch (không hiển thị) các bài viết của người dùng đã chặn bạn hoặc người dùng mà bạn đã chặn.
  * Hỗ trợ tải thêm bài viết (Infinite Scroll/Load More) bằng cách cuộn chuột xuống dưới cùng hoặc nhấn nút tải thêm mà không cần tải lại toàn bộ trang.
  * Hiển thị bài viết của mọi người (công khai) hoặc của những người đang theo dõi theo trình tự thời gian mới nhất.
  * Tải thêm bài viết tự động khi người dùng cuộn xuống dưới cùng của trang bằng AJAX.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/HomeController.cs (Action: Index, GetMorePosts)
  * Views:
    - web/Views/Home/Index.cshtml (Giao diện trang chủ chứa khung cuộn)
    - web/Views/Home/_PostList.cshtml (Partial view kết xuất danh sách bài viết)
  * CSDL (Database Tables): Post, BlockedUser, Follow, PostMedia, PostLike, Comment
  * Tệp JavaScript: web/Views/Shared/_Layout.cshtml (Script lắng nghe sự kiện cuộn trang và kích hoạt fetch tải thêm bài viết)
  * Controller xử lý: web/Controllers/HomeController.cs
    - Bảng tin chính: Index (Dòng 34-177)
    - Tải thêm bài viết: GetMorePosts (Dòng 676-715)
  * Giao diện (Views): web/Views/Home/Index.cshtml, web/Views/Home/_PostList.cshtml (Partial View kết xuất HTML bài viết)
  * JavaScript cuộn trang: web/Views/Shared/_Layout.cshtml
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Sử dụng truy vấn EF Core kết hợp `.Include()` để tối ưu hóa truy vấn liên kết bảng (Eager Loading), nạp đồng thời Post, User tác giả, danh sách Like, Comment và các tệp đính kèm (`PostMedia`) trong một truy vấn duy nhất.
  * Truy vấn LINQ áp dụng bộ lọc chặn kép:
    `postsQuery = postsQuery.Where(p => !_db.BlockedUsers.Any(b => (b.BlockerId == currentUserId && b.BlockedUserId == p.UserId) || (b.BlockerId == p.UserId && b.BlockedUserId == currentUserId)))`
    giúp loại bỏ bài viết của những người dùng liên quan đến quan hệ chặn.
  * Khi cuộn xuống cuối trang, JavaScript kích hoạt hàm gọi AJAX `fetch('/Home/GetMorePosts?page=' + page)` gửi lên server. Server trả về nội dung HTML được kết xuất từ partial view `_PostList.cshtml`. JavaScript chèn chuỗi HTML này vào cuối danh sách bài viết hiện tại thông qua thuộc tính `.insertAdjacentHTML('beforeend', html)`.
  * EF Core sử dụng Eager Loading (.Include()) để nạp toàn bộ ảnh, video, danh sách thích và bình luận trong 1 câu truy vấn duy nhất, đạt hiệu năng tải dưới 3 giây (NFR01).
  * Khi cuộn trang, JS thực hiện gọi AJAX fetch() ngầm đến /Home/GetMorePosts, server trả về HTML kết xuất từ Partial View và JS tự chèn vào DOM bằng `.insertAdjacentHTML('beforeend', html)`.
================================================================================
CHỨC NĂNG 3: ĐĂNG BÀI VIẾT KÈM MULTI-FORMAT FILE UPLOADS (ANY FILE TYPE)
[MODULE 03] ĐĂNG BÀI VIẾT KÈM ĐA PHƯƠNG TIỆN (POST CREATION & MULTI-FORMAT UPLOADS)
================================================================================
- MÃ YÊU CẦU: FR04 (Đăng bài), UC02 (Use Case Đăng bài), AC03 (Nghiệm thu Đăng bài), NFR07 (Mobile Responsive layout)
- CÁCH HOẠT ĐỘNG (How it works):
  * Người dùng có thể đăng bài viết văn bản kèm cảm xúc (feeling), chọn quyền riêng tư, và đính kèm nhiều tệp tin bất kỳ (ảnh, video, PDF, ZIP, RAR, Word, Excel, PowerPoint, TXT...).
  * Cho phép tải lên tối đa 10 tệp tin cùng lúc, dung lượng mỗi tệp lên tới 50MB.
  * Giao diện bài viết hiển thị thông minh:
    - Nếu tệp là hình ảnh hoặc video: hiển thị dạng lưới đa phương tiện trực quan (hỗ trợ phát video trực tiếp bằng trình phát HTML5 có nút điều khiển).
    - Nếu tệp là tài liệu/nén (PDF, ZIP, Word...): hiển thị dưới dạng các thẻ tải xuống (attachment document cards) có biểu tượng đại diện của loại tệp tin, tên tệp gốc và nút tải về trực tiếp.
  * Cho phép người dùng đăng tải nội dung văn bản kèm tệp tin đính kèm bất kỳ (Ảnh, Video, PDF, ZIP, RAR, Word, Excel...).
  * Dung lượng mỗi tệp lên tới 50MB (tối đa 10 tệp cùng lúc).
  * Ở bảng tin, hình ảnh/video tự hiển thị dạng lưới; tài liệu/tệp nén tự hiển thị thành các thẻ download có biểu tượng mở rộng tương ứng và cho phép tải xuống trực tiếp.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/HomeController.cs (Các Action: CreatePost, EditPost, DeletePost)
  * Views:
    - web/Views/Home/CreatePost.cshtml (Giao diện tạo bài viết)
    - web/Views/Home/EditPost.cshtml (Giao diện sửa bài viết)
    - web/Views/Home/_PostList.cshtml (Logic phân biệt và hiển thị loại file)
  * CSDL (Database Tables): Post, PostMedia
  * Controller xử lý: web/Controllers/HomeController.cs
    - Đăng bài: CreatePost (Dòng 182-246)
    - Chỉnh sửa: EditPost (Dòng 405-423)
    - Xóa bài: DeletePost (Dòng 251-273)
  * Giao diện (Views): web/Views/Home/CreatePost.cshtml, web/Views/Home/_PostList.cshtml (Logic phân tích loại tệp đính kèm)
  * Bảng Cơ sở dữ liệu: Post (Thông tin bài viết), PostMedia (Đường dẫn tệp đính kèm)
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Thẻ input tải tệp ở `CreatePost.cshtml` đã được gỡ bỏ thuộc tính `accept` giới hạn, cho phép trình duyệt chọn mọi loại tệp: `<input type="file" id="filePicker" name="MediaFiles" multiple class="d-none">`.
  * Tại Controller, hệ thống nhận danh sách tệp qua `List<IFormFile> MediaFiles`. Các tệp được tạo tên duy nhất trên đĩa cứng bằng cách nối chuỗi `Guid.NewGuid() + "_" + file.FileName` và lưu vào thư mục `wwwroot/uploads/` nhằm tránh xung đột tên tệp.
  * Đường dẫn lưu trữ được ghi nhận vào bảng `PostMedia`.
  * Ở view hiển thị `_PostList.cshtml`, đoạn mã Razor kiểm tra phần mở rộng của tệp:
    ```csharp
    var ext = Path.GetExtension(media.FilePath).ToLower();
    bool isImage = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext);
    bool isVideo = new[] { ".mp4", ".mov", ".webm", ".ogg" }.Contains(ext);
    ```
    Nếu `isImage` hoặc `isVideo` là true, Razor sinh thẻ `<img>` hoặc `<video controls>`. Nếu false, Razor sinh một thẻ `div` với biểu tượng tài liệu, hiển thị tên file bằng cách cắt chuỗi bỏ tiền tố Guid (`media.FilePath.Substring(media.FilePath.IndexOf('_') + 1)`) để trả lại tên file gốc thân thiện với người dùng, kết nối với nút tải về `<a href="@media.FilePath" download>`.
  * Gỡ bỏ thuộc tính `accept` ở thẻ `<input type="file" ...>` cho phép chọn mọi loại file.
  * Phía backend tạo tên file duy nhất theo định dạng {Guid}_{Tên_Gốc} tránh trùng lặp và lưu vào thư mục `wwwroot/uploads/`.
  * Razor view đọc phần mở rộng của file để quyết định xuất thẻ <img>/<video> hay thẻ nén kèm đường dẫn tải về.
================================================================================
CHỨC NĂNG 4: THÍCH & BÌNH LUẬN BÀI VIẾT (POST LIKES & COMMENTS)
[MODULE 04] TƯƠNG TÁC BÀI VIẾT (POST LIKES & COMMENTS)
================================================================================
- MÃ YÊU CẦU: FR06 (Thích bài viết), FR07 (Bình luận bài viết), BR05 (Đăng nhập mới được bình luận), AC04 (Nghiệm thu Bình luận)
- CÁCH HOẠT ĐỘNG (How it works):
  * Cho phép người dùng thả tim/thích (like/love/haha/wow/sad/angry) bài viết. Nhấn lần đầu là thích, nhấn lần hai là hủy thích.
  * Người dùng có thể bình luận dưới mỗi bài viết và xem danh sách các bình luận hiện có.
  * Hỗ trợ tải toàn bộ bình luận của bài viết qua AJAX nếu danh sách bình luận quá dài.
  * Người dùng có thể tự xóa bình luận do chính mình viết.
  * Thả cảm xúc thích/bày tỏ cảm xúc (like, love, haha, wow...) trên bài viết. Nhấn lại để hủy.
  * Viết bình luận và hiển thị bình luận tức thời. Cho phép xóa bình luận của chính mình.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/HomeController.cs (Các Action: ToggleLike, AddComment, DeleteComment, AllComments)
  * Views:
    - web/Views/Home/_PostList.cshtml (Chứa giao diện tương tác thích, bình luận, và form nhập bình luận)
    - web/Views/Home/AllComments.cshtml (Trang hiển thị tất cả bình luận)
  * CSDL (Database Tables): PostLike, Comment, Notification
  * Controller xử lý: web/Controllers/HomeController.cs
    - Thích bài viết: ToggleLike (Dòng 275-347)
    - Viết bình luận: AddComment (Dòng 349-394)
    - Xóa bình luận: DeleteComment (Dòng 425-442)
  * Ràng buộc đăng nhập: Thuộc tính `[Authorize]` trên đầu hàm AddComment (Dòng 348)
  * Bảng Cơ sở dữ liệu: PostLike, Comment
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * `ToggleLike`: Tìm bản ghi trong bảng `PostLike` khớp với `PostId` và `UserId`. Nếu tồn tại, thực hiện `_db.PostLikes.Remove()` (hủy thích). Nếu chưa tồn tại, thêm bản ghi mới `PostLike` với loại cảm xúc đã chọn. Trả về kết quả JSON chứa số lượng thích mới và trạng thái của người dùng để cập nhật giao diện ngay lập tức mà không cần tải lại trang.
  * `AddComment`: Lưu một đối tượng `Comment` mới vào CSDL. Nếu người bình luận khác với người sở hữu bài viết, hệ thống tự động tạo một thông báo `Notification` lưu vào CSDL và gửi thông điệp thời gian thực qua SignalR đến nhóm kết nối của chủ sở hữu bài viết.
  * ToggleLike lưu/xóa lượt thích dựa vào ID người dùng và ID bài viết trong bảng PostLike, trả về JSON cập nhật số lượng động ngay tại giao diện.
  * AddComment lưu bình luận vào CSDL, đồng thời kích hoạt gửi thông báo thực tế cho chủ sở hữu bài viết qua SignalR.
================================================================================
CHỨC NĂNG 5: KHẢO SÁT / BÌNH CHỌN TRONG BÀI VIẾT (POLL CREATION & AJAX VOTING)
[MODULE 05] HỆ THỐNG KHẢO SÁT / BÌNH CHỌN (POLL VOTING ENGINE)
================================================================================
- CÁCH HOẠT ĐỘNG (How it works):
  * Khi đăng bài viết, người dùng có thể tạo một cuộc khảo sát ý kiến (Poll) kèm danh sách các phương án bình chọn.
  * Người xem có thể nhấp chọn một phương án để bình chọn.
  * Ngay sau khi nhấp chọn, hệ thống cập nhật tỷ lệ phần trăm bình chọn và tổng số phiếu bầu cho tất cả các phương án ngay lập tức mà không cần tải lại trang. Thanh phần trăm hiển thị hiệu ứng kéo rộng mượt mà.
  * Người dùng có thể tạo một bài viết chứa cuộc bình chọn (Poll) gồm câu hỏi và nhiều phương án.
  * Người xem nhấp chọn phương án để bình chọn.
  * Tỷ lệ phần trăm và tổng số phiếu của từng phương án được vẽ lại tức thời bằng hiệu ứng di chuyển thanh phần trăm động mà không tải lại trang.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/HomeController.cs (Action: VotePoll)
  * Views:
    - web/Views/Home/_PostList.cshtml (Cấu trúc giao diện hiển thị danh sách câu hỏi bình chọn)
  * Tệp JavaScript: web/Views/Shared/_Layout.cshtml (Hàm `votePoll` từ dòng 571-631)
  * CSDL (Database Tables): PollOption, PollVote
  * Controller xử lý: web/Controllers/HomeController.cs
    - Bình chọn: VotePoll (Dòng 465-503)
  * JavaScript thực thi AJAX: web/Views/Shared/_Layout.cshtml (Hàm votePoll, Dòng 571-631)
  * Giao diện (Views): web/Views/Home/_PostList.cshtml (Gán thuộc tính data-option-id vào các phương án)
  * Bảng Cơ sở dữ liệu: PollOption (Các phương án), PollVote (Chi tiết lượt bầu chọn)
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Trong giao diện, mỗi phương án bình chọn được gán thuộc tính `data-option-id="@option.Id"` và `onclick="votePoll(this, @option.Id)"`.
  * Hàm `votePoll` sử dụng `fetch` để gửi yêu cầu POST bất đồng bộ tới đầu cuối `/Home/VotePoll?optionId=X` kèm token CSRF lấy từ `__RequestVerificationToken` để phòng chống tấn công giả mạo yêu cầu chéo trang.
  * Tại Controller, hệ thống thực hiện kiểm tra xem người dùng hiện tại đã bình chọn cho cuộc khảo sát này (dựa trên `PostId` liên quan) chưa. Nếu chưa, lưu bản ghi mới vào `PollVote`.
  * Sau khi lưu thành công, hệ thống tính toán tổng số lượt bình chọn của bài viết đó và phần trăm của từng phương án, sau đó trả về cấu trúc JSON chứa mảng các option cập nhật: `{ success: true, options: [ { optionId: 1, votes: 5, percentage: 50 }, ... ] }`.
  * JavaScript ở client duyệt qua danh sách các option nhận được từ phản hồi, tìm kiếm phần tử HTML bằng thuộc tính `data-option-id`, cập nhật tỷ lệ rộng bằng CSS `style.width = opt.percentage + '%'`, điền thêm dấu tích (✓) và số lượng phiếu tương ứng, đồng thời loại bỏ thuộc tính `onclick` và thêm class `voted` vào khung poll để vô hiệu hóa lượt nhấp chuột tiếp theo của người dùng.
  * JS gửi request POST bất đồng bộ (fetch()) chứa optionId kèm CSRF Anti-Forgery Token.
  * Server kiểm tra quyền bình chọn (chưa từng vote câu hỏi này), lưu lượt bầu chọn, tính toán lại phần trăm của tất cả phương án trong cuộc khảo sát đó rồi phản hồi dạng JSON.
  * JS nhận kết quả, cập nhật độ rộng thuộc tính CSS (bar.style.width), thay đổi text phần trăm, gán dấu (✓) và vô hiệu hóa sự kiện click để tránh bình chọn trùng lặp.
================================================================================
CHỨC NĂNG 6: BẢN TIN NGẮN 24H (STORIES CAROUSEL & VIEWER)
[MODULE 06] BẢN TIN NGẮN 24 GIỜ (STORIES CAROUSEL & VIEWER)
================================================================================
- CÁCH HOẠT ĐỘNG (How it works):
  * Người dùng có thể đăng các hình ảnh hoặc video ngắn lên Story của mình. Các Story này sẽ tự động ẩn đi sau 24 giờ kể từ thời điểm đăng.
  * Hiển thị danh sách Story của mọi người ở đầu trang chủ dưới dạng thanh trượt ngang (Carousel). Kích thước các thẻ Story luôn cố định tỉ lệ chân dung (dọc) chuẩn, không bị kéo dãn bề ngang kể cả khi Story là video quay ngang.
  * Người dùng có thể nhấp vào Story để xem, thả biểu cảm thích/tim trên Story.
  * Hệ thống tự động ghi lại danh sách người xem Story (Story views) và hiển thị thống kê này cho người đăng Story.
  * Đăng Stories (tin ngắn) dạng hình ảnh/video ngắn biến mất hoàn toàn sau 24h.
  * Hiển thị danh sách Stories bằng thanh trượt ngang có kích thước dọc chân dung chuẩn 110px x 180px, không bị kéo dãn hình kể cả khi video là khung hình ngang.
  * Người dùng xem Stories có thể thả tim/thích và chủ Stories có thể xem danh sách thống kê những ai đã xem tin của mình.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/HomeController.cs (Các Action: CreateStory, DeleteStory, DeleteAllStories, ToggleStoryLike, RecordStoryView)
  * Views:
    - web/Views/Home/Index.cshtml (Giao diện thanh trượt Stories ở dòng 92-133 và mã modal trình phát Story)
  * CSDL (Database Tables): Story, StoryLike, StoryView
  * Controller xử lý: web/Controllers/HomeController.cs
    - Đăng Story: CreateStory (Dòng 925-976)
    - Xem Story: RecordStoryView (Dòng 1257-1277)
    - Thả tim Story: ToggleStoryLike (Dòng 1197-1255)
  * CSS định dạng: web/wwwroot/css/site.css (Style cho .story-card và các thẻ media con)
  * Bảng Cơ sở dữ liệu: Story, StoryLike, StoryView
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Khi truy cập trang chủ, Controller chỉ lấy ra các Story được đăng trong vòng 24 giờ qua bằng điều kiện `s.CreatedAt >= DateTime.Now.AddHours(-24)` và tác giả không nằm trong danh sách chặn.
  * Để giải quyết lỗi thẻ video quay ngang làm giãn rộng thẻ Story trong flexbox, CSS được thiết lập cố định cho `.story-card` với các giá trị `width: 110px; min-width: 110px; height: 180px; display: block; overflow: hidden; position: relative;`. Thẻ con `<img>` và `<video>` bên trong được gán CSS `width: 100%; height: 100%; object-fit: cover;` giúp tự động cắt xén nội dung vừa vặn vào khung ảnh chân dung chuẩn mà không làm biến dạng hình ảnh/video.
  * Khi click vào một Story, JavaScript mở modal trình phát, phát video tự động, đồng thời gửi một yêu cầu AJAX ngầm đến `/Home/RecordStoryView?storyId=X`. Controller nhận yêu cầu, kiểm tra nếu người xem không phải là tác giả và chưa có bản ghi lượt xem nào cho Story này, hệ thống sẽ chèn một dòng dữ liệu vào bảng `StoryView`.
  * LINQ chỉ truy vấn các Stories trong vòng 24 giờ qua: `s.CreatedAt >= DateTime.Now.AddHours(-24)`.
  * CSS sử dụng width: 110px; min-width: 110px; height: 180px; kèm object-fit: cover cho các thẻ ảnh/video con giúp tự động xén bớt phần dư thừa, vừa khít khung dọc mà không bị méo.
  * Sự kiện mở xem Story sẽ gửi AJAX /Home/RecordStoryView?storyId=X để ghi nhận lượt xem.
================================================================================
CHỨC NĂNG 7: KẾT BẠN & THEO DÕI NGƯỜI DÙNG (FRIEND REQUESTS & FOLLOWS)
[MODULE 07] KẾT BẠN & THEO DÕI NGƯỜI DÙNG (FRIENDS & FOLLOWS)
================================================================================
- MÃ YÊU CẦU: FR09 (Kết bạn), AC05 (Nghiệm thu nhắn tin & bạn bè)
- CÁCH HOẠT ĐỘNG (How it works):
  * Theo dõi (Follow): Cho phép người dùng theo dõi người khác để cập nhật bài viết của họ trên bảng tin của mình.
  * Kết bạn (Friendship): Người dùng gửi yêu cầu kết bạn. Bên nhận có quyền Chấp nhận (Accept) hoặc Từ chối (Decline). Sau khi chấp nhận, cả hai trở thành bạn bè và tự động thiết lập trạng thái theo dõi lẫn nhau (mutual follow).
  * Hủy kết bạn (Remove Friend) hoặc hủy yêu cầu kết bạn đang chờ.
  * Theo dõi người dùng khác để cập nhật bài viết của họ trên bảng tin của mình.
  * Gửi lời mời kết bạn. Người nhận có thể Đồng ý hoặc Từ chối. Khi đồng ý kết bạn, hệ thống tự động thiết lập trạng thái theo dõi chéo lẫn nhau (mutual follow).
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/HomeController.cs (Các Action: ToggleFollow, AddFriend, AcceptFriend, DeclineFriend, RemoveFriend, Friends, GetFollowers, GetFollowing)
  * Views:
    - web/Views/Home/Friends.cshtml (Giao diện danh sách bạn bè & các lời mời kết bạn)
    - web/Views/Home/Profile.cshtml (Các nút tương tác Kết bạn / Theo dõi trên trang cá nhân)
  * CSDL (Database Tables): Follow, Friendship, Notification
  * Controller xử lý: web/Controllers/HomeController.cs
    - Theo dõi: ToggleFollow (Dòng 507-545)
    - Gửi kết bạn: AddFriend (Dòng 1021-1079)
    - Đồng ý kết bạn: AcceptFriend (Dòng 1081-1131)
    - Từ chối kết bạn: DeclineFriend (Dòng 1133-1159)
    - Hủy kết bạn: RemoveFriend (Dòng 1161-1195)
  * Giao diện (Views): web/Views/Home/Friends.cshtml, web/Views/Home/Profile.cshtml (Các nút hành động)
  * Bảng Cơ sở dữ liệu: Friendship, Follow
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * `ToggleFollow`: Kiểm tra bảng `Follow` xem đã tồn tại mối quan hệ giữa `FollowerId` (người nhấn) và `FollowingId` (người được theo dõi) chưa. Nếu có thì xóa (Unfollow), nếu chưa thì thêm mới (Follow).
  * `AddFriend`: Tạo bản ghi trong bảng `Friendship` với `Status = FriendshipStatus.Pending` (0) và ghi nhận `RequesterId`, `ReceiverId`. Đồng thời, hệ thống tạo một thông báo gửi tới người nhận.
  * `AcceptFriend`: Cập nhật `Status = FriendshipStatus.Accepted` (1) trong bảng `Friendship`. Đồng thời, hệ thống tạo hai bản ghi trong bảng `Follow` để tự động thiết lập theo dõi 2 chiều giữa hai người dùng, đảm bảo bài viết của cả hai xuất hiện trên bảng tin của nhau.
  * AcceptFriend cập nhật trạng thái Status = FriendshipStatus.Accepted trong bảng Friendship, đồng thời tự động chèn 2 bản ghi vào bảng Follow đại diện cho mối quan hệ 2 chiều, giúp bảng tin hiển thị bài viết của cả hai.
================================================================================
CHỨC NĂNG 8: TRANG CÁ NHÂN & THIẾT LẬP THÔNG TIN (PROFILE & USER SETTINGS)
[MODULE 08] TRANG CÁ NHÂN & THIẾT LẬP (PROFILE & USER SETTINGS)
================================================================================
- MÃ YÊU CẦU: FR03 (Quản lý hồ sơ), BR04 (Người dùng có thể chặn tài khoản khác)
- CÁCH HOẠT ĐỘNG (How it works):
  * Hiển thị thông tin cá nhân của người dùng bao gồm Họ tên, Tiểu sử (Bio), Ảnh đại diện, Ảnh bìa, danh sách bài viết đã đăng, và danh sách bạn bè.
  * Hỗ trợ cập nhật thông tin cá nhân (Họ tên, Bio) và tải lên thay đổi Ảnh đại diện / Ảnh bìa.
  * Ngăn chặn việc tràn chữ: Các tiểu sử (Bio) dài hoặc tên dài sẽ tự động ngắt xuống dòng, không làm vỡ giao diện Profile.
  * Thực thi quy tắc chặn (Block view): Nếu người dùng B bị người dùng A chặn, khi B cố gắng truy cập trang cá nhân của A, hệ thống sẽ trả về lỗi 404 (Không tìm thấy trang). Tuy nhiên, Admin và Moderator được miễn trừ quy tắc này để có thể kiểm tra thông tin tài khoản phục vụ mục đích kiểm duyệt.
  * Xem trang cá nhân của mình hoặc người khác.
  * Giao diện tự động ngắt dòng văn bản dài (tiểu sử Bio/tên dài) tránh vỡ cấu trúc CSS.
  * Chặn hiển thị: Nếu User A chặn User B, khi B cố truy cập trang cá nhân của A sẽ nhận lỗi 404 NotFound. Ngoại lệ: Admin và Moderator vẫn xem được trang để phục vụ công tác kiểm duyệt.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/HomeController.cs (Các Action: Profile, Settings)
  * Views:
    - web/Views/Home/Profile.cshtml (Giao diện trang cá nhân)
    - web/Views/Home/Settings.cshtml (Giao diện cài đặt thông tin cá nhân)
  * CSS: web/wwwroot/css/site.css hoặc styles nhúng ở Profile.cshtml để xử lý ngắt chữ (`word-break: break-word;`).
  * CSDL (Database Tables): AspNetUser, BlockedUser
  * Controller xử lý: web/Controllers/HomeController.cs (Action Profile, Dòng 548-627)
  * Giao diện (Views): web/Views/Home/Profile.cshtml
  * CSS định dạng: web/wwwroot/css/site.css hoặc styles nhúng ở Profile (`word-break: break-word; flex-wrap: wrap;`)
  * Bảng Cơ sở dữ liệu: AspNetUser, BlockedUser
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Để ngăn tràn chữ, CSS trên các thẻ chứa văn bản dài được thiết lập thuộc tính `flex-wrap: wrap; word-break: break-word; white-space: normal;` giúp văn bản tự động bẻ dòng khi chạm biên giới hạn của thẻ cha.
  * Trong Action `Profile`, hệ thống truy vấn CSDL để kiểm tra mối quan hệ chặn:
    ```csharp
    bool isBlockedByTarget = _db.BlockedUsers.Any(b => b.BlockerId == id && b.BlockedUserId == currentUserId);
    if (isBlockedByTarget && !User.IsInRole("Admin") && !User.IsInRole("Moderator"))
    {
        return NotFound();
    }
    ```
    Nếu tồn tại bản ghi chặn và người dùng hiện tại không có vai trò `Admin` hoặc `Moderator` (`!User.IsInRole("Admin")`), ASP.NET Core trả về kết quả `NotFound()` (Mã trạng thái HTTP 404). Ngược lại, nếu là Admin/Moderator, hệ thống bỏ qua và tiếp tục kết xuất trang cá nhân để phục vụ kiểm duyệt.
  * CSS sử dụng word-break: break-word; white-space: normal; giúp chữ tự xuống dòng.
  * Action Profile kiểm tra CSDL:
    `bool isBlockedByTarget = _db.BlockedUsers.Any(b => b.BlockerId == id && b.BlockedUserId == currentUserId);`
    Nếu trả về true và người truy cập không có vai trò Admin/Moderator, Action sẽ lập tức trả về NotFound() (HTTP 404).
================================================================================
CHỨC NĂNG 9: NHẮN TIN TRỰC TIẾP THỜI GIAN THỰC (REAL-TIME DIRECT MESSAGES)
[MODULE 09] NHẮN TIN TRỰC TIẾP THỜI GIAN THỰC (REAL-TIME CHAT & DMs)
================================================================================
- MÃ YÊU CẦU: FR08 (Nhắn tin), UC03 (Use Case Nhắn tin), AC05 (Nghiệm thu Nhắn tin)
- CÁCH HOẠT ĐỘNG (How it works):
  * Người dùng có thể tìm kiếm bạn bè và mở hộp thoại nhắn tin trực tiếp.
  * Tin nhắn gửi đi được truyền tải ngay lập tức đến màn hình đối phương mà không cần tải lại trang. Hỗ trợ đính kèm tệp tin đa phương tiện trong chat.
  * Hiển thị chỉ báo đối phương đang gõ tin nhắn (Typing Indicator).
  * Hiển thị trạng thái tin nhắn đã đọc/chưa đọc. Cập nhật số lượng tin nhắn chưa đọc lên biểu tượng Chat trên thanh điều hướng.
  * Người dùng mở hộp thoại chat trực tiếp với bạn bè. Tin nhắn gửi đi xuất hiện tức thời trên màn hình bên nhận. Hỗ trợ đính kèm ảnh, video, tài liệu trong khung chat.
  * Hiển thị trạng thái tin nhắn (đã đọc/chưa đọc) và hiển thị chỉ báo đối phương đang gõ tin nhắn (Typing Indicator). Cập nhật số tin nhắn chưa đọc lên thanh điều hướng.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/ChatController.cs (Các Action: Index, GetMessages, GetUnreadCount, SearchUsers, UploadChatMedia)
  * SignalR Hub: web/Hubs/NexusHub.cs (Quản lý các sự kiện kết nối và truyền dữ liệu WebSocket)
  * Views: web/Views/Chat/Index.cshtml (Giao diện chat chính)
  * Tệp JavaScript: web/wwwroot/js/chat.js (hoặc mã script kết nối SignalR tích hợp)
  * CSDL (Database Tables): Conversation, ChatMessage, BlockedUser
  * Controller xử lý: web/Controllers/ChatController.cs
    - Trang chat: Index (Dòng 24-167)
    - Tải tin nhắn: GetMessages (Dòng 169-209)
  * SignalR Hub: web/Hubs/NexusHub.cs
    - Gửi tin nhắn: SendDirectMessage (Dòng 67-149)
    - Báo đã đọc: MarkConversationRead (Dòng 154-176)
    - Chỉ báo gõ: SendTypingIndicator (Dòng 181-194)
  * Giao diện & Script: web/Views/Chat/Index.cshtml, web/wwwroot/js/chat.js (hoặc mã script kết nối SignalR tích hợp)
  * Bảng Cơ sở dữ liệu: Conversation, ChatMessage
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Sử dụng thư viện SignalR thiết lập kết nối WebSocket hai chiều thời gian thực.
  * Khi người dùng truy cập trang Chat, client khởi chạy kết nối SignalR tới `/nexusHub`.
  * Trên server, phương thức `OnConnectedAsync` lấy ID người dùng và đưa kết nối của họ vào một Group mang tên chính `UserId` của họ thông qua `Groups.AddToGroupAsync(Context.ConnectionId, userId)`.
  * Khi gửi tin nhắn, client gọi phương thức `SendDirectMessage` của Hub. Hub tiến hành kiểm tra chặn: nếu một trong hai bên đã chặn nhau, tin nhắn sẽ bị chặn và không được lưu hay gửi đi.
  * Hub chuẩn hóa ID cuộc trò chuyện bằng cách sắp xếp thứ tự bảng chữ cái của 2 ID người dùng để tạo cuộc hội thoại duy nhất. Sau đó lưu tin nhắn vào bảng `ChatMessage`.
  * Cuối cùng, Hub gửi gói dữ liệu tin nhắn tới Group của người nhận thông qua `Clients.Group(recipientId).SendAsync("ReceiveDirectMessage", payload)` và gửi lại cho các tab đang mở của chính người gửi.
  * SignalR thiết lập kết nối WebSocket liên tục. Trên server, phương thức OnConnectedAsync đưa kết nối client vào một nhóm (Group) có tên trùng với UserId của họ.
  * Khi gửi tin nhắn, Hub lưu vào bảng ChatMessage, sau đó gọi phương thức client ReceiveDirectMessage gửi tới Group của người nhận và người gửi để đồng bộ.
  * ID hội thoại được tạo theo thứ tự từ điển của 2 ID người dùng để đảm bảo duy nhất.
================================================================================
CHỨC NĂNG 10: THÔNG BÁO THỜI GIAN THỰC (REAL-TIME NOTIFICATIONS)
[MODULE 10] THÔNG BÁO THỜI GIAN THỰC (REAL-TIME NOTIFICATIONS)
================================================================================
- MÃ YÊU CẦU: FR10 (Thông báo)
- CÁCH HOẠT ĐỘNG (How it works):
  * Người dùng nhận được thông báo ngay lập tức khi có người thích bài viết, bình luận, gửi lời mời kết bạn, chấp nhận kết bạn, theo dõi, hoặc khi bài viết của mình bị Admin/Moderator xóa do vi phạm.
  * Hiển thị số lượng thông báo chưa đọc trên quả chuông thông báo ở thanh điều hướng. Số lượng này tự động tăng lên khi có thông báo mới mà không cần tải lại trang.
  * Nhận thông báo lập tức khi có người thích, bình luận bài viết, gửi lời mời kết bạn, đồng ý kết bạn, theo dõi, hoặc khi bài viết bị ban quản trị xóa.
  * Badge (số đếm quả chuông) tự tăng lên động mà không cần reload trang.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/HomeController.cs (Các Action: Notifications, MarkNotificationsRead)
  * Controller xử lý: web/Controllers/HomeController.cs (Action Notifications (Dòng 880-910), MarkNotificationsRead (Dòng 912-923))
  * SignalR Hub: web/Hubs/NexusHub.cs
  * Views:
    - web/Views/Home/Notifications.cshtml (Trang danh sách thông báo)
    - web/Views/Shared/_Layout.cshtml (Nơi chứa mã JavaScript lắng nghe sự kiện thông báo)
  * CSDL (Database Tables): Notification
  * Giao diện: web/Views/Home/Notifications.cshtml, JavaScript lắng nghe tại web/Views/Shared/_Layout.cshtml
  * Bảng Cơ sở dữ liệu: Notification
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Khi các hành động (Like, Comment, Follow...) xảy ra ở các Controller tương ứng, sau khi lưu bản ghi vào bảng `Notification` trong CSDL, Controller sử dụng `IHubContext<NexusHub>` để lấy thông tin kết nối và gọi:
    `_hubContext.Clients.Group(targetUserId).SendAsync("ReceiveNotification", new { count = unreadCount, message = content })`.
  * Tại Client (`_Layout.cshtml`), mã JavaScript đăng ký lắng nghe sự kiện:
    ```javascript
    connection.on("ReceiveNotification", function(data) {
        // Cập nhật số lượng hiển thị trên Badge quả chuông
        // Hiển thị thông báo Toast góc màn hình
    });
    ```
    Điều này giúp cập nhật giao diện người dùng tức thời thông qua kết nối WebSocket đang hoạt động.
  * Khi có hành động phát sinh, các Controller lưu dữ liệu vào bảng Notification, sau đó gọi:
    `_hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", data)`
  * JS ở layout đăng ký sự kiện connection.on("ReceiveNotification", ...) để tăng số lượng badge và hiện hộp thông báo nổi (Toast).
================================================================================
CHỨC NĂNG 11: BÁO CÁO VI PHẠM & HÀNG ĐỢI KIỂM DUYỆT (REPORTING & MODERATION QUEUE)
[MODULE 11] BÁO CÁO VI PHẠM & HÀNG ĐỢI KIỂM DUYỆT (REPORT & MODERATION)
================================================================================
- MÃ YÊU CẦU: FR12 (Báo cáo vi phạm), FR13 (Kiểm duyệt nội dung), UC04 (Use Case Kiểm duyệt), BR03 & BR07 (Nội dung vi phạm bị xóa & cảnh cáo)
- CÁCH HOẠT ĐỘNG (How it works):
  * Người dùng thông thường có thể báo cáo (flag) các bài viết hoặc người dùng vi phạm các chính sách cộng đồng kèm theo lý do cụ thể.
  * Người kiểm duyệt (Moderator) và Quản trị viên (Admin) có giao diện Hàng đợi kiểm duyệt chuyên biệt để theo dõi danh sách các nội dung bị báo cáo.
  * Người kiểm duyệt có thể Bác bỏ báo cáo (Dismiss) hoặc Xóa bài viết vi phạm (Delete). Khi xóa bài viết, hệ thống tự động gửi thông báo cảnh cáo kèm lý do đến tác giả bài viết đó mà không gây lỗi ứng dụng.
  * Người dùng báo cáo bài viết vi phạm chính sách kèm lý do.
  * Người kiểm duyệt (Moderator) và Admin theo dõi hàng đợi báo cáo (/Moderation/Queue).
  * Có quyền Bác bỏ báo cáo (Dismiss) hoặc Xóa bài viết (Delete). Khi xóa bài viết, hệ thống tự động gửi thông báo cảnh cáo an toàn tới tác giả bài viết đó.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/ModerationController.cs (Các Action: Queue, Process) và web/Controllers/AdminController.cs (Action: ResolveReport)
  * Views:
    - web/Views/Moderation/Queue.cshtml (Giao diện hàng đợi kiểm duyệt của Moderator)
    - web/Views/Admin/Reports.cshtml (Giao diện quản lý báo cáo của Admin)
  * CSDL (Database Tables): Report, Notification, Post
  * Controller xử lý: web/Controllers/ModerationController.cs
    - Hàng đợi: Queue (Dòng 24-61)
    - Xử lý: Process (Dòng 65-103)
  * Admin Controller xử lý: web/Controllers/AdminController.cs (Action ResolveReport, Dòng 226-263)
  * Giao diện (Views): web/Views/Moderation/Queue.cshtml, web/Views/Admin/Reports.cshtml
  * Bảng Cơ sở dữ liệu: Report, Notification, Post
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Khi người dùng gửi báo cáo, một bản ghi mới với trạng thái `Status = "Pending"` được tạo trong bảng `Report`.
  * Các Action kiểm duyệt được bảo vệ bởi thuộc tính quyền hạn `[Authorize(Roles = "Admin,Moderator")]`.
  * Để ngăn lỗi NullReferenceException khi xóa bài viết, hệ thống thực thi theo thứ tự logic chính xác:
    1. Đầu tiên, lấy ID tác giả của bài viết bị báo cáo từ đối tượng Post trước khi nó bị tách khỏi ngữ cảnh theo dõi: `string postOwnerId = post.UserId;`.
    2. Tạo mới và lưu bản ghi cảnh cáo `Notification` cho tác giả đó: `_db.Notifications.Add(...)`.
    3. Thực hiện xóa bài viết khỏi CSDL: `_db.Posts.Remove(post)`.
    4. Gọi `_db.SaveChangesAsync()`.
    Do EF Core Change Tracker sẽ tự động dọn sạch các mối liên kết khóa ngoại (Foreign Key) khi một đối tượng bị đánh dấu xóa, việc truy cập `post.UserId` sau khi đã xóa đối tượng sẽ gây ra lỗi tham chiếu null (NullReferenceException). Cách thiết kế trên đảm bảo dữ liệu được truy xuất an toàn trước khi thực hiện thao tác xóa.
  * Các Controller được bảo vệ bằng phân quyền [Authorize(Roles = "Admin,Moderator")].
  * Để tránh lỗi NullReferenceException khi xóa bài viết, logic code tuân thủ thứ tự chính xác:
    1. Lưu lại ID tác giả bài viết từ Post trước khi ngắt kết nối: `string postOwnerId = post.UserId;`
    2. Tạo mới và lưu bản ghi cảnh cáo Notification cho tác giả đó.
    3. Thực hiện xóa Post khỏi db: `_db.Posts.Remove(post);`
    4. Lưu thay đổi: `await _db.SaveChangesAsync();`
    Cơ chế này đảm bảo EF Change Tracker không giải phóng mối liên kết ngoại trước khi thực hiện lưu thông báo phạt.
================================================================================
CHỨC NĂNG 12: BẢNG ĐIỀU KHIỂN ADMIN & QUẢN LÝ THÀNH VIÊN (ADMIN DASHBOARD)
[MODULE 12] BẢNG QUẢN TRỊ ADMIN & QUẢN LÝ THÀNH VIÊN (ADMIN DASHBOARD)
================================================================================
- MÃ YÊU CẦU: FR14 (Quản lý người dùng), FR15 (Thống kê hệ thống), FR16 (Phân quyền), BR06 (Admin có toàn quyền)
- CÁCH HOẠT ĐỘNG (How it works):
  * Cung cấp số liệu thống kê tổng quan của toàn hệ thống (Tổng số thành viên, tổng số bài viết, tổng số bình luận, số lượt đăng ký tài khoản mới trong ngày hôm nay).
  * Quản trị viên có thể tìm kiếm thành viên, thay đổi vai trò (Role) của người dùng giữa Admin, Moderator và User.
  * Quản trị viên có thể Khóa tài khoản (Block) người dùng và bắt buộc nhập lý do khóa cụ thể, hoặc Mở khóa tài khoản (Unblock). Người dùng bị khóa sẽ không thể đăng nhập vào hệ thống.
  * Admin truy cập trang Dashboard xem các thống kê toàn diện (tổng số user, bài viết, bình luận, chat, lượng đăng ký mới trong ngày).
  * Tìm kiếm người dùng, đổi vai trò thành viên (User -> Moderator -> Admin).
  * Khóa tài khoản (Block) người dùng bắt buộc nhập lý do hoặc Mở khóa tài khoản (Unblock). Tài khoản bị khóa sẽ lập tức bị chặn không cho đăng nhập.
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/AdminController.cs (Các Action: Dashboard, Users, BlockUser, UnblockUser, ChangeRole)
  * Views:
    - web/Views/Admin/Dashboard.cshtml (Giao diện thống kê)
    - web/Views/Admin/Users.cshtml (Giao diện quản lý người dùng, phân quyền và khóa tài khoản)
  * CSDL (Database Tables): AspNetUser, AspNetRole, AspNetUserRole, Post, Comment, Report
  * Controller xử lý: web/Controllers/AdminController.cs
    - Dashboard thống kê: Dashboard (Dòng 29-86)
    - Quản lý user: Users (Dòng 88-143)
    - Khóa/Mở khóa: BlockUser (Dòng 145-159) & UnblockUser (Dòng 161-175)
    - Đổi vai trò: ChangeRole (Dòng 177-197)
  * Chặn đăng nhập: web/Controllers/AccountController.cs (Hàm LoginPost, Dòng 54-58)
  * Giao diện (Views): web/Views/Admin/Dashboard.cshtml, web/Views/Admin/Users.cshtml
  * Bảng Cơ sở dữ liệu: AspNetUser, AspNetRole, AspNetUserRole
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Bộ lọc `[Authorize(Roles = "Admin")]` đảm bảo chỉ có tài khoản thuộc vai trò Admin mới có thể truy cập các tài nguyên quản trị này.
  * Các số liệu thống kê được tính toán bằng các hàm gom cụm hiệu năng cao của LINQ (`_db.Users.Count()`, `_db.Posts.Count()`, v.v.) và lọc đăng ký trong ngày bằng cách so sánh thuộc tính ngày tháng (`u.CreatedAt >= DateTime.Today`).
  * Khi khóa người dùng, Admin cập nhật các trường dữ liệu trực tiếp trên thực thể `ApplicationUser`: `IsBlocked = true`, `BlockedAt = DateTime.UtcNow`, `BlockedReason = reason`. Khi người dùng cố gắng đăng nhập, Action `LoginPost` trong `AccountController` kiểm tra:
    `if (user.IsBlocked) { ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Lý do: " + user.BlockedReason); return View(model); }`
    Điều này ngăn chặn hoàn toàn việc đăng nhập của các tài khoản vi phạm.
  * Bộ lọc `[Authorize(Roles = "Admin")]` ngăn chặn truy cập trái phép.
  * Khi khóa, hệ thống cập nhật `IsBlocked = true` và `BlockedReason = reason`. Tại AccountController.LoginPost, khi xác thực thành công, hệ thống kiểm tra nếu `user.IsBlocked == true` thì từ chối cấp session cookie và hiển thị lý do khóa lên màn hình đăng nhập.
================================================================================
CHỨC NĂNG 13: KHỞI TẠO CƠ SỞ DỮ LIỆU & ÁNH XẠ EF CORE (DATABASE INITIALIZATION)
[MODULE 13] SAO LƯU DỮ LIỆU HỆ THỐNG (DATA BACKUP ENGINE)
================================================================================
- MÃ YÊU CẦU: FR17 (Sao lưu dữ liệu), NFR06 (Backup dữ liệu mỗi ngày), AC07 (Backup dữ liệu thành công)
- CÁCH HOẠT ĐỘNG (How it works):
  * Cơ sở dữ liệu của ứng dụng được đặt tên là `WebSocialMediaDB` sử dụng LocalDB của SQL Server.
  * Tệp tin cơ sở dữ liệu `WebSocialMediaDB.mdf` và tệp nhật ký `WebSocialMediaDB_log.ldf` được lưu trữ trực tiếp trong thư mục dự án tại `web/Databases/` để dễ dàng sao lưu, chia sẻ giữa các thành viên phát triển.
  * Toàn bộ các bảng trong CSDL (bao gồm cả các bảng mặc định của ASP.NET Identity) được thiết kế và ánh xạ sang dạng tên tiếng Anh số ít (Singular Table Names).
  * Cung cấp giao diện quản trị sao lưu dữ liệu (/Admin/Backup) cho phép Admin tạo bản sao lưu (.bak) trực tuyến bất kỳ lúc nào.
  * Hiển thị danh sách các bản sao lưu đã tạo kèm theo dung lượng file (tính bằng KB/MB) và ngày giờ tạo tệp.
  * Cho phép tải trực tiếp file sao lưu về máy cá nhân (Download) hoặc xóa file sao lưu khỏi máy chủ.
- NƠI HOẠT ĐỘNG (Where it works):
  * Tệp cấu hình: web/appsettings.json (Chuỗi kết nối Connection String)
  * Khởi chạy hệ sinh thái: web/Program.cs (Thiết lập DataDirectory)
  * Ánh xạ CSDL: web/Data/ApplicationDbContext.cs (Phương thức `OnModelCreating`)
  * Thư mục lưu trữ: web/Databases/
  * Controller xử lý: web/Controllers/AdminController.cs
    - Xem danh sách và trang backup: Backup (Dòng 286-312)
    - Tạo backup: CreateBackup (Dòng 314-345)
    - Tải xuống: DownloadBackup (Dòng 347-367)
    - Xóa: DeleteBackup (Dòng 369-395)
  * Giao diện (Views): web/Views/Admin/Backup.cshtml
  * Liên kết sidebar: web/Views/Shared/_AdminLayout.cshtml (Dòng 623)
  * Thư mục lưu trữ tệp vật lý: web/Databases/Backups/
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Tại `Program.cs`, hệ thống giải quyết vấn đề sai lệch đường dẫn tương đối khi sử dụng `|DataDirectory|` trong chuỗi kết nối bằng cách định nghĩa cứng thư mục chạy thực tế:
    ```csharp
    var dbFolder = Path.Combine(builder.Environment.ContentRootPath, "Databases");
    AppDomain.CurrentDomain.SetData("DataDirectory", dbFolder);
    ```
    Điều này giúp cho cả quá trình chạy ứng dụng và quá trình chạy công cụ dòng lệnh của EF Core (như tạo migration) đều tìm thấy chính xác tệp tin MDF nằm trong thư mục `Databases` của dự án mà không bị lỗi.
  * Để ánh xạ tên bảng số ít mà không làm vỡ mã nguồn C# hiện tại, trong lớp `ApplicationDbContext.cs`, Fluent API thực hiện cấu hình ánh xạ:
    ```csharp
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Post>().ToTable("Post");
        builder.Entity<Comment>().ToTable("Comment");
        // Ánh xạ tương tự cho tất cả thực thể khác...
    }
    ```
    EF Core sẽ tự động chuyển đổi các câu lệnh SQL tự sinh sử dụng tên bảng số ít (ví dụ: `SELECT * FROM Post`), trong khi lập trình viên vẫn có thể gọi các thuộc tính số nhiều trong mã nguồn (ví dụ: `_db.Posts.ToList()`), đảm bảo tính nhất quán tuyệt đối.
  * Thực hiện sao lưu bất đồng bộ bằng lệnh SQL gốc (T-SQL BACKUP DATABASE) qua EF Core:
    `await _db.Database.ExecuteSqlRawAsync("BACKUP DATABASE [WebSocialMediaDB] TO DISK = {0} WITH FORMAT, INIT", backupPath);`
    Lệnh này chạy online, tạo ra file `.bak` chuẩn mà không khóa cơ sở dữ liệu hay làm gián đoạn ứng dụng (không downtime).
  * Tên tệp tải xuống được kiểm soát để ngăn chặn tấn công Directory Traversal (chỉ cho phép tệp đuôi `.bak` hợp lệ).
================================================================================
CHỨC NĂNG 14: SAO LƯU DỮ LIỆU HỆ THỐNG (DATA BACKUP - FR17 & NFR06 & AC07)
[MODULE 14] KHỞI TẠO CƠ SỞ DỮ LIỆU & ÁNH XẠ (DATABASE & EF CORE MAPPING)
================================================================================
- CÁCH HOẠT ĐỘNG (How it works):
  * Quản trị viên (Admin) có giao diện chuyên biệt (/Admin/Backup) để quản lý các bản sao lưu cơ sở dữ liệu (.bak).
  * Cho phép bấm "Tạo bản sao lưu mới" để tạo một tệp sao lưu tức thời.
  * Hiển thị danh sách các tệp sao lưu đã tạo (Tên tệp, dung lượng file tính bằng KB/MB, và thời gian tạo tệp).
  * Hỗ trợ tải trực tiếp tệp sao lưu về máy cá nhân (Download) hoặc xóa bản sao lưu (Delete) khỏi hệ thống.
  * Cơ sở dữ liệu ứng dụng được lưu trực tiếp trong thư mục dự án tại web/Databases/WebSocialMediaDB.mdf và WebSocialMediaDB_log.ldf.
  * Ánh xạ toàn bộ 23 bảng CSDL mặc định của Identity và tự tạo sang dạng tên tiếng Anh số ít (Singular Table Names).
- NƠI HOẠT ĐỘNG (Where it works):
  * Controller: web/Controllers/AdminController.cs (Các Action: Backup, CreateBackup, DownloadBackup, DeleteBackup)
  * Views:
    - web/Views/Admin/Backup.cshtml (Giao diện hiển thị, tạo và quản trị file backup)
    - web/Views/Shared/_AdminLayout.cshtml (Thêm menu link "Sao lưu dữ liệu")
  * Thư mục lưu trữ tệp vật lý: web/Databases/Backups/
  * Cấu hình chuỗi kết nối: web/appsettings.json
  * Cấu hình thư mục chứa MDF động: web/Program.cs (Dòng 10-12)
  * Lớp ánh xạ Fluent API: web/Data/ApplicationDbContext.cs (Phương thức OnModelCreating)
- TẠI SAO HOẠT ĐỘNG (Why it works):
  * Việc sao lưu được thực hiện hoàn toàn trực tuyến và bất đồng bộ bằng cách chạy câu lệnh SQL gốc (T-SQL BACKUP DATABASE) qua Entity Framework Core DbContext:
    `_db.Database.ExecuteSqlRawAsync("BACKUP DATABASE [WebSocialMediaDB] TO DISK = {0} WITH FORMAT, INIT", backupPath);`
  * Câu lệnh này chỉ thị cho SQL Server tạo bản sao lưu vật lý sang file `.bak` mà không khóa hay làm gián đoạn bất kỳ kết nối CSDL nào khác (không gây downtime cho ứng dụng).
  * Controller kiểm tra độ an toàn của tên tệp tải về để ngăn ngừa tấn công Directory Traversal (chỉ cho phép tệp đuôi `.bak` hợp lệ).
  * Thư mục lưu trữ `Backups/` được phân tích động từ `DataDirectory` do SQL Server LocalDB chạy dưới tài khoản Windows người dùng hiện tại nên có toàn quyền ghi file.
  * Trong Program.cs, mã C# giải quyết chuỗi `|DataDirectory|` thành đường dẫn vật lý tuyệt đối trước khi nạp DbContext:
    `AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(builder.Environment.ContentRootPath, "Databases"));`
    Cơ chế này ngăn ngừa lỗi lệch đường dẫn giữa thiết kế migrations (Design-time) và chạy ứng dụng thực tế (Runtime).
  * Trong ApplicationDbContext.cs, gọi ánh xạ Fluent API `.ToTable("Post")` ép EF Core tự sinh SQL với tên số ít, trong khi giữ nguyên thuộc tính C# số nhiều `DbSet<Post> Posts` để tránh lỗi biên dịch mã nguồn hiện tại.
================================================================================
