#include <PWM.h> 
#include <LiquidCrystal.h>

String inputString = "";
boolean stringComplete = false;  
String commandString = "";


const int signal_pin = 13;  
      
int32_t frequency;
int cif = 0;

int flowPin = 2;    
double flowRate;     
volatile int count;   
LiquidCrystal lcd(8, 3, 4, 5, 6, 7);

void setup()
{
    Serial.begin(9600); 
    InitTimersSafe();
    pinMode(flowPin, INPUT);           
    attachInterrupt(0, Flow, RISING);  
}

void loop()
{
pumpaj();
}


void getCommand()
{
  if(inputString.length()>0)
  {
     commandString = inputString.substring(1,5);
  }
}

void pumpaj(){
     int i = 0;

      
     pwmWriteHR(signal_pin, 32768); 
     SetPinFrequencySafe(signal_pin, frequency);
     for(i=0;i<15;i++){
        count = 0;      
        interrupts();   
        delay (1000);    
        noInterrupts(); 
   
        flowRate = (count * 2.25);         
        flowRate = flowRate * 60;         
        flowRate = flowRate / 1000;       
 
        lcd.begin(16, 2);
        lcd.setCursor(0,0);
        lcd.print("Protok:");
        lcd.setCursor(8, 0);
        lcd.print(flowRate);
        Serial.println(flowRate);
        lcd.setCursor(13, 0);
        lcd.print("L/Min");
        lcd.setCursor(0, 1);
        lcd.print("Frekvencija:");
        lcd.setCursor(12,1);
        lcd.print(frequency);
     }
     pwmWriteHR(signal_pin, 0);
     delay(1000);
     lcd.clear();
}

void serialEvent() {
  while (Serial.available()) {
    char inChar = (char)Serial.read();
    Serial.println(inChar);
    inputString += inChar;
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}

void Flow()
{
   count++; 
}
