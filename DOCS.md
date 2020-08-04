![](https://storage.googleapis.com/replit/images/1593218325636_c176cdd8d0b9c70f1102fcf77353d9b8.png)
# Stekovaya v1.4.1

## The Stack
Push the numbers 1, 2, 7, 3 onto the stack
```
1 2 7 3
```

Push the word "Hello World!" onto the stack
```
STR Hello World! END
REM use {variable name} in the STR - END to can access to the variable
```

**Note**: Boolean values are stored as numbers, 1 for true and 0 for false.

---

## Words

### MSG
Pops the value at the top of the stack into the console
```
STR This is in the console END MSG
```

### REM, RMS, RME
Used for comments
```
REM This is a comment, so it will not be interpreted
RMS
Multiline comment
RME
```

### DEF
Stores a value into the given variable ( **Note**: If variable name starts from _ then, you cant change variable value. )
```
STR VariableName END STR Value END DEF
VariableName MSG
REM This outputs "Value" to the console
```

### STK
Outputs the entire stack to the console
```
1 2 3 5 STK
REM Outputs this: STACK<4> 1 2 3 5
```
### MOD
Calculate modulos the top two values of the stack and pushes the result
```
3 2 MOD MSG
REM Prints 1
```

### ADD
Adds the top two values of the stack and pushes the result
```
5 2 ADD MSG
REM Prints 7
```

### SUB
Subtracts the top two values of the stack and pushes the result
```
9 3 SUB MSG
REM Prints 6
```

### MUL
Multiplies the top two values of the stack and pushes the result
```
5 3 MUL MSG
REM Prints 15
```

### DIV
Divides the top value of the stack by the one below it and pushes the result
```
7 2 DIV MSG
REM Prints 3.5
```

### POW
Calculates the top value of the stack to the power of the one below it and pushes the result
```
2 5 POW MSG
REM Prints 32
```

### ROT
Calulates the root of the second value of the stack with a base of the first and pushes the result
```
2 8 ROT MSG
REM Prints 2.82842712474619, which is √8
```

### FOR ... EFOR
Loops a block of code forever
```
FOR
STR Spam Time :D END MSG
EFOR
```

### THN
Executes code iff the top value of the stack > 0
```
4 THN STR This is run because 4 > 0 END MSG
```

### LSS, LEQ, GTR, GEQ, EQU, NEQ
Compares the top two values of the stack and pushes the result to the stack
<, <=, >, >=, ==, and != respectively
```
5 2 GTR
```

### SLP
Pops the value at the top of the stack for sleep millisecond(s)

```
1000 SLP
```

### POP
Pops the value at the top of the stack

```
1 2 POP MSG
REM Prints 1
```

### NOT
Reverse the top value of the stack and if top value is 0 then push 1, if not then push 1

```
1 NOT
```

### TOB
Convert the top value of stack to boolean

```
2 TOB
```

### BRK
Break from FOR’s loop
```
FOR
_F00 MSG
_F01 10 LSS NOT THN BRK
EFOR
```

### DMSG
Prints the value at the top of the stack into the console

**Note**: DMSG doesnt pop stack.
```
1 DMSG
REM Prints 1
```

### EMP
Do nothing
```
EMP
```

### ZET
Pops the value at the top of the stack and Calc ζ(x)
```
3 ZET MSG
REM Prints ζ(3)
```

### EXT
Exit program by exit status 0
```
EXT
```

### ERX
Exit program by exit status 1
```
ERX
```

### DUP
Duplicate the value at the top of the stack
```
2 1 DUP STK
REM Prints STACK<3> 2 1 1
```

### JBP
Pops the value at the top of the stack and jump to same line’s position
```
10 MSG 1 JBP
REM This will loops
```

### JBL
Pops the value st the top of the stack and jump to other line
```
1 MSG
1 JBL
```

### LAD
Define label
```
1 LAD
```

### LAA
Access to label
```
1 LAA
```

### ARR
Define Array
```
1 2 3 "size" STR Name END ARR
REM Can access by Name / Name-Number
```

### APSH
Push content to array
```
STR New something END "Name" APSH
```

### APOP
Pops the value at the top of the array
```
"Name" APOP
```

### LEN
Pushes the length of an array
```
1 3 5 7 "Array" ARR "Array" LEN
```

### SLEN
Pushes the length of the value at the top of the stack
```
STR A string END SLEN
```

### INC
Load package, you can select package from these packages
 - Include.Util.Stdin
 - Include.Util.File
 - Include.Util.Error

Include.Util.Stdin is for INP, Include.Util.File is for control file, Include.Util.Error is for generate error / warn and stderr
```
"Include.Util.Stdin" INC
"Include.Util.File" INC
"Include.Util.Error" INC

STR Message  END INP
REM Get input from user

STR Filename END STR Content END WRT
REM Write to file

STR Filename END RED
REM Read file and push content to stack

STR Filename END DEL
REM Delete file

STR Filename END EXS
REM If found file then push 1 if not then push 0

STR Error END ERR
REM Generate error

STR Warn END WRN
REM Generate warn

STR Error END WTE
REM write to stderr

STR Error END NWE
REM write to stderr, but without newline

STR Error END DNWE
REM write to stderr, but without newline and pop

STR Error END WTE
REM write to stderr, but without pop
```

### SCO
Concat the value at the top of the stack and the value at the 2 from the top of the stack
```
"A" "B" SCO MSG
REM Prints AB
```

### GCD
Calculate GCD(m,n)
```
6 10 GCD
```

### LCM
Calculate LCM(m,n)
```
6 10 LCM
```

### ETF
Get element from array
```
"name" 0 ETF
```

### FLR
Calculate floor(n)
```
6.5 FLR
```

### CEL
Calculate ceil(n)
```
6.5 CEL
```

### ROD
Calculate round(n)
```
6.5 ROD
```

### TRC
Calculate trunc(n)
```
6.5 TRC
```

### ABS
Calculate ABS(n)
```
6.5 ABS
```

### SIN
Calculate sin(n)
```
6.5 SIN
```

### COS
Calculate cos(n)
```
6.5 COS
```

### TAN
Calculate tan(n)
```
6.5 TAN
```

### ASN
Calculate asin(n)
```
6.5 ASN
```

### ACS
Calculate acos(n)
```
6.5 ACS
```

### ATN
Calculate atan(n)
```
6.5 ATN
```

### DRW
Plot to x,y and set color to r,g,b ( note, bitmap size is 100x100 )
```
0 0 255 255 255 DRW
```

### SVE
Output bitmap to file
```
"test.png" SVE
```
### CLS
Clear screen
```
CLS
```

### BEL
Play beep sound
```
BEL
```

### NMG
Pops the value at the top of the stack into the console without newline
```
STR This is in the console END NMG
```

### DNMG
Prints the value at the top of the stack into the console without newline

**Note**: DNMG doesnt pop stack.
```
1 DMNG
```

### EAR
Edit array content
```
"array" 0 "content" EAR
```
### DAT
Get local time
```
DAT
```

### UTC
Get UTC time
```
UTC
```

### SPL
Split string
```
"string" "splitby" "VariableName" SPL
```

### Escaped strings
Here is the list
 - `\ESC` Escape (U+001B)
 - `\NUL` NULL (U+0000)
 - `\BEL` BELL (U+0007)
 - `\UNFxxxx` (U+xxxx)
 - `\UNTxx` (U+00xx)
 - `\CRT` Carriage return (U+000D)
 - `\LFD` Line feed (U+000A)
 - `\ATR` Reset text's color
 - `\ABR` Reset background's color
 - `\BLD` Set text to bold
 - `\DIM` Set text to dim
 - `\ITL` Set text to italic
 - `\UDL` Set text to underline
 - `\REV` Reverse background's color and text's color
 - `\SME` Set text's color to background's color
 - `\DIS` Reset style
 - `\ACT#id` Set color to text
   - `#0` Black
   - `#1` Red
   - `#2` Green
   - `#3` Yellow
   - `#4` Blue
   - `#5` Magenta
   - `#6` Cyan
   - `#7` White
 - `\ACB#id` Set color to background
   - `#0` Black
   - `#1` Red
   - `#2` Green
   - `#3` Yellow
   - `#4` Blue
   - `#5` Magenta
   - `#6` Cyan
   - `#7` White
 - `\ATE#id` Set extended color to text ( id = 0 ~ 255 )
 - `\ABE#id` Set extended color to background ( id = 0 ~ 255 )