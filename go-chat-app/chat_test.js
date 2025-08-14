import ws from 'k6/ws';
import { check, sleep } from 'k6';

export const options = {
    // Thiết lập các tùy chọn kiểm thử
    vus: 10, // virtual users (số người dùng ảo)
    duration: '30s', // thời gian chạy kiểm thử
    thresholds: {
        // Kiểm tra các chỉ số quan trọng
        // ws_connecting là metric dạng trend, không dùng 'count'
        // 'ws_connecting': ['avg<100'], // Ví dụ: độ trễ trung bình phải nhỏ hơn 100ms
        'ws_msgs_sent': ['rate<10'], // Tỷ lệ tin nhắn gửi trên giây < 10
        'ws_msgs_received': ['rate<10'], // Tỷ lệ tin nhắn nhận trên giây < 10
    },
};

export default function () {
    const url = 'ws://localhost:9000/ws'; // Thay đổi nếu bạn dùng cổng khác
    const params = { tags: { my_tag: 'chat' } };

    const res = ws.connect(url, params, function (socket) {
        socket.on('open', () => {
            // Gửi tin nhắn sau khi kết nối thành công
            socket.send(`hello from k6 at ${Date.now()}`);
        });

        socket.on('message', (data) => {
            // Khi nhận được tin nhắn, in ra console
            // console.log(`Message received: ${data}`);
        });

        socket.on('close', () => {
            // console.log('disconnected');
        });

        // Giữ kết nối mở trong suốt thời gian chạy
        socket.on('error', (e) => {
            if (e.error() != 'websocket: close sent') {
                console.log('An unexpected error occurred: ', e.error());
            }
        });
    });

    check(res, { 'status is 101': (r) => r && r.status === 101 });

    sleep(0);
}