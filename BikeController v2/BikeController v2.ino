//Control code for adjusting the resistance of the exercycle
//Sending a value between 1 and 25 causes the bike to alter its resistance to match that value
//Sending the value 0 causes the bike to reset the resistance to 1 no matter what it was on previously
//Other values are ignored


//Pins for adjusting the bike levels
const int pinDown = 4;
const int pinUp   = 3;
const int pinFan  = 10;

//The level we currently believe the bike to be at
int currentLevel = 25; // assuming the worst for reset
int desiredLevel = 1;

const int keypressDelay = 80;

// the setup routine runs once when you press reset:
void setup() {                
  // initialize the digital pin as an output.
  pinMode(pinDown, OUTPUT);     
  pinMode(pinUp, OUTPUT);
  
  // set the PWM frequency for pin 10 down to 30Hz (or anything less than 50)
  // Source: http://arduino-info.wikispaces.com/Arduino-PWM-Frequency
  // This does not affect delay/millis (driven by timer 0)
  // Source: http://arduino.cc/en/Tutorial/SecretsOfArduinoPWM
  //
  TCCR1B = TCCR1B & B11111000 | B00000101;
  pinMode(pinFan, OUTPUT);
  analogWrite(pinFan, 0); // fan off initially
  
  Serial.begin(9600);
}

// Loop constantly, checking whether data has been sent
void loop() 
{
  int inData = -1;

  while ( Serial.available() > 0 )
  {
    inData = Serial.read();

    if ( inData >=0 )
    {
      //Serial.println(inData);
      if ( (inData >= 100) && (inData <= 200) )
      {
        // values between 100 and 200 are interpredted as fan speed (100=0%, 150=50%, 200=100%)
        int fanSpeed = (inData - 100);
        int pwmValue = fanSpeed * 255 / 100;
        analogWrite(pinFan, pwmValue);
      }
      else if (inData >= 1 && inData <= 25)
      {
        // values between 0 and 25 are interpredted as resistance settings
        desiredLevel = inData;
      }
      else if (inData == 0)
      {
        resetLevel();
      }
    }
  }
  
  checkLevel();
}

//Sets the level to 1, regardless of where it was
void resetLevel()
{
  currentLevel = 25; // assume maximum, so the down keys will be pressed a lot
  desiredLevel = 1;
}

// Adapts the current level of the bike until it reachres the desired level
void checkLevel()
{
  if ( currentLevel < desiredLevel )
  {
    digitalWrite(pinUp, HIGH);
    delay(keypressDelay);
    digitalWrite(pinUp, LOW);
    delay(keypressDelay);
    currentLevel++;
  }
  else if ( currentLevel > desiredLevel )
  {
    digitalWrite(pinDown, HIGH);   
    delay(keypressDelay);        
    digitalWrite(pinDown, LOW); 
    delay(keypressDelay);   
    currentLevel--;
  }
}




