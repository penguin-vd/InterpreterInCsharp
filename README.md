# InterpreterInCsharp

## Features

- Int
- Bool
- Functions
- Variables
- JIT Interpreter

## Example Code

``` bigl
let x = 20;
let y = 10;
y + x;

let add = fn(x, y) { return x + y; };
add(10, 2);

let adder = fn(x) { fn(y) { x + y } };
let addTwo = adder(2);
addTwo(3);
```
