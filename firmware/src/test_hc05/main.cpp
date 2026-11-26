// =============================================================================
// test_hc05/main.cpp — Test kết nối Bluetooth HC-05
// Kiểm tra: baudrate, truyền/nhận chuỗi \n hai chiều, phản hồi ping/echo
//
// Cách dùng (Serial Monitor ở 38400 baud):
//   Gõ bất kỳ → Arduino echo lại + thêm " [ECHO]"
//   Gõ "PING" → phản hồi "PONG\n"
//   Gõ "STATUS" → in baudrate và trạng thái
//   Hàng giây: Arduino chủ động gửi một dòng telemetry giả "D:0.00,0.00,255\n"
//   để kiểm thử chiều nhận của controller_app
//
// QUAN TRỌNG: Rút cáp USB/Serial của Arduino khi HC-05 đang ở chế độ hoạt động
// (D0/D1 dùng chung với USB-UART CH340). Nạp code TRƯỚC khi cắm HC-05.
// =============================================================================
#include <Arduino.h>

#define HC05_BAUD_RATE 38400
#define FAKE_TELE_INTERVAL_MS 1000UL

static char   g_rxBuf[64];
static uint8_t g_rxIdx   = 0;
static uint32_t g_lastTelemMs = 0;
static uint32_t g_rxCount = 0;
static uint32_t g_txCount = 0;

static void processLine(char* line);

void setup() {
    Serial.begin(HC05_BAUD_RATE);
    delay(100);  // OK: delay ngắn trong setup(), không trong loop()
    Serial.println(F("HC-05 TEST READY"));
    Serial.print(F("BAUD:"));
    Serial.println(HC05_BAUD_RATE);
    Serial.println(F("CMD: PING | STATUS | <any text to echo>"));
    g_lastTelemMs = millis();
}

void loop() {
    // --- Nhận dữ liệu từ app controller hoặc Serial Monitor ---
    while (Serial.available() > 0) {
        char c = (char)Serial.read();
        if (c == '\r') continue;
        if (c == '\n') {
            g_rxBuf[g_rxIdx] = '\0';
            if (g_rxIdx > 0) {
                g_rxCount++;
                processLine(g_rxBuf);
            }
            g_rxIdx = 0;
        } else if (g_rxIdx < 62) {
            g_rxBuf[g_rxIdx++] = c;
        } else {
            g_rxIdx = 0;  // Buffer đầy, reset
        }
    }

    // --- Gửi telemetry giả mỗi 1 giây để kiểm tra chiều Arduino → App ---
    uint32_t now = millis();
    if ((now - g_lastTelemMs) >= FAKE_TELE_INTERVAL_MS) {
        g_lastTelemMs = now;
        // Giả lập bản tin telemetry chuẩn — dùng để xác nhận app parse được
        Serial.print(F("D:0.00,0.00,255\n"));
        g_txCount++;
    }
}

static void processLine(char* line) {
    if (strcmp(line, "PING") == 0) {
        Serial.print(F("PONG\n"));
        g_txCount++;
    } else if (strcmp(line, "STATUS") == 0) {
        Serial.print(F("STATUS:BAUD="));
        Serial.print(HC05_BAUD_RATE);
        Serial.print(F(",RX="));
        Serial.print(g_rxCount);
        Serial.print(F(",TX="));
        Serial.print(g_txCount);
        Serial.print('\n');
        g_txCount++;
    } else {
        // Echo bất kỳ chuỗi nào về lại (kèm thẻ [ECHO])
        Serial.print(F("ECHO:"));
        Serial.print(line);
        Serial.print('\n');
        g_txCount++;
    }
}
