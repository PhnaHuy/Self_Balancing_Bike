// =============================================================================
// nidec_pfm.h — Timer2 PFM output for Nidec 24H055M020
// Arduino Uno: OC2B on D3, 50% duty, variable frequency.
// =============================================================================
#pragma once
#include <Arduino.h>
#include "config.h"

static bool g_nidecPfmRunning = false;
static uint8_t g_nidecPfmClockSelect = 0;

static inline void nidec_pfm_init() {
    pinMode(PIN_NIDEC_PFM, OUTPUT);
    digitalWrite(PIN_NIDEC_PFM, LOW);
    TCCR2A = _BV(WGM20) | _BV(WGM21);
    TCCR2B = _BV(WGM22);
    TCNT2 = 0;
    g_nidecPfmRunning = false;
    g_nidecPfmClockSelect = 0;
}

static inline void nidec_pfm_stop() {
    TCCR2B = _BV(WGM22);
    TCCR2A = _BV(WGM20) | _BV(WGM21);
    TCNT2 = 0;
    digitalWrite(PIN_NIDEC_PFM, LOW);
    g_nidecPfmRunning = false;
    g_nidecPfmClockSelect = 0;
}

static inline uint16_t nidec_pfm_setFrequency(uint16_t frequencyHz) {
    if (frequencyHz == 0) {
        nidec_pfm_stop();
        return 0;
    }

    static const uint16_t prescalers[] = {1, 8, 32, 64, 128, 256, 1024};
    uint8_t selectedIndex = sizeof(prescalers) / sizeof(prescalers[0]) - 1;
    uint16_t counts = 256;

    for (uint8_t i = 0; i < sizeof(prescalers) / sizeof(prescalers[0]); ++i) {
        uint32_t divisor = (uint32_t)prescalers[i] * frequencyHz;
        uint32_t candidate = (F_CPU + divisor / 2) / divisor;
        if (candidate >= 2 && candidate <= 256) {
            if (candidate & 1) {
                ++candidate;
            }
            selectedIndex = i;
            counts = (uint16_t)candidate;
            break;
        }
    }

    uint8_t top = (uint8_t)(counts - 1);
    uint8_t compare = (uint8_t)(counts / 2 - 1);
    uint8_t clockSelect = selectedIndex + 1;

    pinMode(PIN_NIDEC_PFM, OUTPUT);
    OCR2A = top;
    OCR2B = compare;
    if (!g_nidecPfmRunning) {
        TCNT2 = 0;
        TCCR2A = _BV(WGM20) | _BV(WGM21) | _BV(COM2B1);
        TCCR2B = _BV(WGM22) | clockSelect;
        g_nidecPfmRunning = true;
    } else if (clockSelect != g_nidecPfmClockSelect) {
        TCCR2B = _BV(WGM22) | clockSelect;
    }
    g_nidecPfmClockSelect = clockSelect;

    return (uint16_t)(F_CPU / ((uint32_t)prescalers[selectedIndex] * counts));
}
