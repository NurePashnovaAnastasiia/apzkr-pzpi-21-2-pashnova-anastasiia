#include<WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>

const char* ssid = "Wokwi-GUEST";
const char* pass = "";

unsigned const long interval = 2000;
unsigned long previousMillis = 0;

const int greenLedPin = 27;
const int redLedPin = 26;

void setup(){
  Serial.begin(115200);
  WiFi.begin(ssid, pass);
  while(WiFi.status() != WL_CONNECTED){
    delay(100);
    Serial.println(".");
  }
  Serial.println("WiFi Connected!");
  Serial.println(WiFi.localIP());

  pinMode(greenLedPin, OUTPUT);
  pinMode(redLedPin, OUTPUT);
}

void loop()
{
  unsigned long currentMillis = millis();
  if (currentMillis - previousMillis > interval)
  {
    previousMillis = currentMillis;

    HTTPClient http;
    http.begin("https://lightservewebapi20240424185555.azurewebsites.net/api/Order/getOrderById?orderId=1");
    int httpResponseCode = http.GET();
    if (httpResponseCode > 0)
    {
      String payload = http.getString();

      DynamicJsonDocument doc(1024);
      deserializeJson(doc, payload);

      bool isDone = doc["isDone"];
      Serial.print(isDone);

      if (isDone)
      {
        digitalWrite(greenLedPin, HIGH);
        digitalWrite(redLedPin, LOW);
      }
      else
      {
        digitalWrite(greenLedPin, LOW);
        digitalWrite(redLedPin, HIGH);
      }
    }
    else
    {
      Serial.print("HTTP request failed with error code: ");
      Serial.println(httpResponseCode);
    }

    http.end();
  }
}