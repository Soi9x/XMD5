# XMD5


Cách sử dụng

Chạy lệnh:
XMD5 -create <directoryPath>
Chương trình sẽ tạo tệp VRFMD5.json chứa các MD5 hashes của tất cả tệp trong thư mục, bỏ qua XMD5.exe và VRFMD5.json.

Xác minh MD5 hashes:

Chạy lệnh:

XMD5 -verify <directoryPath>

Chương trình sẽ so sánh các MD5 hashes hiện tại với giá trị trong VRFMD5.json, bỏ qua XMD5.exe và VRFMD5.json.

Kết quả

Nếu tất cả các tệp khớp, chương trình sẽ hiển thị "**Match**".

Nếu có bất kỳ tệp nào không khớp, chương trình sẽ hiển thị "**Not Match**" và liệt kê các tệp không khớp.
