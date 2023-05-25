# InterpreterInCsharp

## Features

- Int
- Bool
- Functions
- Variables
- JIT Interpreter

## Example Code

``` bigl
// Declare variables
let x = 20;
let y = 10;
y + x;

// This would be a function
let add = fn(x, y) { return x + y; };
add(10, 2);

// Whitespace does not matter for functions
let adder = fn(x){
    fn(y) { 
    x + y; 
    }};
let addTwo = adder(2);
addTwo(3);
```

## Future Features

- String (Data type)
- Loops
- Standard Library (very minimal)
