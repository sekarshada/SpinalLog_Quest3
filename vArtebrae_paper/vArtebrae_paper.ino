#include "Adafruit_MLX90393.h"
#include <Wire.h>
#include <math.h>
#include "BluetoothSerial.h"

#define SERVICE_UUID "00001101-0000-1000-8000-00805F9B34FB"
String esp32_device_name = "ESP32-vARtebrae-Gabriella";

#define PCAADDR 0x70 // I2C Multiplexer Address

BluetoothSerial SerialBT;

Adafruit_MLX90393 sensors[8] = {
  Adafruit_MLX90393(), Adafruit_MLX90393(), Adafruit_MLX90393(), Adafruit_MLX90393(),
  Adafruit_MLX90393(), Adafruit_MLX90393(), Adafruit_MLX90393(), Adafruit_MLX90393()
};

uint8_t sensorAddresses[8] = {0xC, 0xD, 0xE, 0xF, 0xC, 0xD, 0xE, 0xF};
float x, y, z[8], distance[8];

// Column multiplexer control pins (read)
const byte s0 = 13;
const byte s1 = 33;
const byte s2 = 27;
const byte s3 = 4;
// Row multiplexer control pins (write)
const byte w0 = 15;
const byte w1 = 32;
const byte w2 = 19;
const byte w3 = 14;
// Signal pins
const byte SIG_pin = 36; // Analog input from column mux
const byte OUT_pin = 5;  // Output to row mux
// Multiplexer channel logic table (0–15)
const boolean muxChannel[16][4] = {
  {0,0,0,0}, {1,0,0,0}, {0,1,0,0}, {1,1,0,0},
  {0,0,1,0}, {1,0,1,0}, {0,1,1,0}, {1,1,1,0},
  {0,0,0,1}, {1,0,0,1}, {0,1,0,1}, {1,1,0,1},
  {0,0,1,1}, {1,0,1,1}, {0,1,1,1}, {1,1,1,1}
};
int baseline[9][11];

void setup() {
  Serial.begin(115200);
  SerialBT.begin(esp32_device_name, true);
  SerialBT.begin(SERVICE_UUID);
  Wire.begin();

  for (uint8_t i = 0; i < 8; i++) {
    uint8_t channel = i / 4;
    pcaselect(channel);
    sensors[i].begin_I2C(sensorAddresses[i]);
    sensors[i].setGain(MLX90393_GAIN_1X);
    sensors[i].setResolution(MLX90393_Z, MLX90393_RES_16);
    sensors[i].setOversampling(MLX90393_OSR_1);
    sensors[i].setFilter(MLX90393_FILTER_0);
  }

  byte pins[] = {s0, s1, s2, s3, w0, w1, w2, w3, OUT_pin};
  for (byte i = 0; i < sizeof(pins); i++) {
    pinMode(pins[i], OUTPUT);
    digitalWrite(pins[i], LOW);
  }
  digitalWrite(OUT_pin, HIGH);  // Enable voltage to row mux
  // Take baseline readings
  for (int row = 0; row < 9; row++) {
    writeMux(row);
    delayMicroseconds(300);
    for (int col = 0; col < 11; col++) {
      baseline[row][col] = readMux(col);
    }
  }
  Serial.println("Baseline calibrated.");
}

void loop() {
  if (SerialBT.connected()) {
    Serial.println("Device connected via Bluetooth!");

    while (true) {
      for (uint8_t i = 0; i < 8; i++) {
        uint8_t channel = i / 4;
        pcaselect(channel);
        sensors[i].readData(&x, &y, &z[i]);
        distance[i] = convertToDistance(z[i]);
        //distance[i] = 20 + (rand() % 6); // Generates a random number between 20 and 25
      }

      for (int row = 0; row < 9; row++) {
        writeMux(row);
        delayMicroseconds(300);
        for (int col = 0; col < 11; col++) {
          int val = readMux(col);
          SerialBT.print(val);
          Serial.print(val);
          SerialBT.print(",");
        }
      }
      Serial.println(); 
      
      //if (SerialBT.available()) {
      //}
      printAllData();

      if (!SerialBT.connected()) {
        Serial.println("Device disconnected!");
        break;
      }
    }
  }
}

void printAllData() {
  uint8_t order[] = {3, 7, 2, 6, 1, 5, 0, 4}; // Order based on ch1-11, ch0-11, ..., ch0-00
  
  for (uint8_t i = 0; i < 8; i++) {
    uint8_t idx = order[i];
    Serial.print("Z"); Serial.print(idx); Serial.print(": "); Serial.print(z[idx]); 
    Serial.print(" uT, Distance: "); Serial.print(distance[idx]); Serial.println(" mm");
  }
  Serial.println("--------------");
  
  // Print distances to Bluetooth
  for (uint8_t i = 0; i < 8; i++) {
    SerialBT.print(distance[order[i]]);
    if (i < 7) SerialBT.print(",");
  }
  SerialBT.println();
} 

float convertToDistance(float magneticField) {
  return -20.032 * pow(magneticField, 1.0/3.0) + 3.995 * sqrt(magneticField) - 0.006 * magneticField + 107.15;
}

void pcaselect(uint8_t channel) {
  if (channel > 1) return;
  Wire.beginTransmission(PCAADDR);
  Wire.write(1 << channel);
  Wire.endTransmission();
}

// Set the column (read mux)
int readMux(byte channel) {
  byte sel[] = {s0, s1, s2, s3};
  // First: disable all column control lines (set LOW)
  for (int i = 0; i < 4; i++) {
    digitalWrite(sel[i], LOW);
  }
  // Now: activate just the selected channel
  for (int i = 0; i < 4; i++) {
    digitalWrite(sel[i], muxChannel[channel][i]);
  }
  delayMicroseconds(20);  // give time for settling
  return analogRead(SIG_pin);
}
// Set the row (write mux)
void writeMux(byte channel) {
  digitalWrite(OUT_pin, LOW);  // disable output while switching
  byte sel[] = {w0, w1, w2, w3};
  for (int i = 0; i < 4; i++) {
    digitalWrite(sel[i], muxChannel[channel][i]);
  }
  digitalWrite(OUT_pin, HIGH); // enable selected row
}

