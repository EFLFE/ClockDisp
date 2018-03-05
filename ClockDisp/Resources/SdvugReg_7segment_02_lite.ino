#define SCLK A7  // Shift
#define Latch A6 // RCK(RCLK)
#define DIO A5 //SER/DS/SI/A

byte digitBuffer[6]; //буфер символов для 6ти разрядов

void setup() {
  Serial.begin(9600);
  pinMode(Latch, OUTPUT);
  pinMode(SCLK, OUTPUT);
  pinMode(DIO, OUTPUT);
}

void loop() {
  timeNow();
  showDisplay(); //однократно выводим данные из массива на 7сигмент, то есть функция должна постоянно крутится.
}

//Режим отображения реального времени
void timeNow () {
  digitBuffer[0] = 4; // Разряд единиц минут
  digitBuffer[1] = 3; // Разряд десятков минут
  digitBuffer[2] = 11; // Разряд секунд (двоеточие)
  digitBuffer[3] = 2; // Разряд единиц часов
  digitBuffer[4] = 1; // Разряд десятков часов
  digitBuffer[5] = 12; // Разряд дней недели
}

// функция вывода битовых масок в регистры
void showDisplay() {
  const byte digit[45] = {  // маска для 8ми сегментов
    //0bHGFEDCBA
    0b01110111, //n0  - 0,O
    0b01000100, //n1  - 1,I,i
    0b00111101, //n2  - 2
    0b01011101, //n3  - 3
    0b01001110, //n4  - 4,Ч
    0b01011011, //n5  - 5,S
    0b01111011, //n6  - 6,Б
    0b01000101, //n7  - 7
    0b01111111, //n8  - 8
    0b01011111, //n9  - 9
    0b10000000, //n10 - :
    0b00000000, //n11 - clear
    0b00000001, //n12 - Пн
    0b10000000, //n13 - Вт
    0b00000010, //n14 - Ср
    0b00000100, //n15 - Чт
    0b01000000, //n16 - Пт
    0b00001000, //n17 - Сб
    0b00010000, //n18 - Вс
    0b01101111, //n19 - A
    0b01111010, //n20 - b
    0b00110011, //n21 - C
    0b01111100, //n22 - d
    0b01111101, //n23 - д
    0b00111011, //n24 - E
    0b00111111, //n25 - e
    0b00101011, //n26 - F
    0b01110011, //n27 - G
    0b01101110, //n28 - H,Эн
    0b01101010, //n29 - h
    0b01010100, //n30 - J
    0b00110010, //n31 - L
    0b01101001, //n32 - M,m,Эм
    0b01101000, //n33 - n
    0b01111000, //n34 - o
    0b00101111, //n35 - P
    0b00101000, //n36 - r
    0b00111010, //n37 - t,Температура
    0b01110110, //n38 - U
    0b01011110, //n39 - y
    0b00100011, //n40 - Г
    0b00001000, //n41 - "-"
    0b00010000, //n42 - "_"
    0b00001111, //n43 - °C+
    0b00011111  //n44 - °C-
  };

  const byte chr[6] = { // маска для разрядов
    0b00000001, // Разряд единиц минут
    0b00000010, // Разряд десятков минут
    0b00000100, // Разряд секунд (двоеточие)
    0b00001000, // Разряд единиц часов
    0b00010000, // Разряд десятков часов
    0b00100000  // Разряд дней недели
  };

  // отправляем в цикле по два байта в сдвиговые регистры
  for (byte i = 0; i <= 5; i++) {
    digitalWrite(Latch, LOW); // открываем защелку
    shiftOut(DIO, SCLK, MSBFIRST, digit[digitBuffer[i]]);  // отправляем байт с "числом"
    shiftOut(DIO, SCLK, MSBFIRST, chr[i]);   // включаем разряд
    digitalWrite(Latch, HIGH); // защелкиваем регистры

    // Эти 4 строчки нужны для эмулятора ВЛИ дисплея
    Serial.write(digit[digitBuffer[i]]); //Debug
    Serial.write(chr[i]);                //Debug
    Serial.write(13);                    //Debug new line
    Serial.write(10);                    //Debug new line

    delay(1); // ждем немного перед отправкой следующего "числа"
  }
}

/*
  // функция сдвига битов, является стандартной в ардуино, и добавлена только для STM32generic
  void shiftOut(uint8_t dataPin, uint8_t clockPin, uint8_t bitOrder, uint8_t val) {
  uint8_t i;

  for (i = 0; i < 8; i++) {
    if (bitOrder == LSBFIRST) {
      digitalWrite(dataPin, !!(val & (1 << i)));
    }
    else {
      digitalWrite(dataPin, !!(val & (1 << (7 - i))));
    }
    digitalWrite(clockPin, HIGH);
    digitalWrite(clockPin, LOW);
  }
  }
*/
