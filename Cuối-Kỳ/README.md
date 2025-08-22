## Caro (Gomoku) WinForms over TCP

A two-laptop Caro game using TCP sockets and a plaintext protocol suitable for Wireshark.

### Run

- Host: run `CaroWinApp`, choose Port (default 9090), click Host.
- Join: run `CaroWinApp`, enter Host IP and same Port, click Join.
- Host plays `X` first. Click cells to move. Use Reset to clear. Chat via box.

### Protocol (text, one line per message, UTF-8)\n
- `HELLO HOST X` or `HELLO CLIENT O`
- `MOVE r c` (0-based row, col)
- `RESET`
- `CHAT text...`

No TLS so Wireshark can read packets easily.

### Wireshark tips

- Filter: `tcp.port == 9090`
- Right-click any packet → Follow → TCP Stream to view conversation
- Expect lines like `HELLO`, `MOVE`, `CHAT`.

### Notes

- Allow app through Windows Firewall for chosen port.
- Ensure both machines are on same network or routed appropriately.

