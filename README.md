# InterpreterInCsharp

## Features

- Int
- Bool
- String
- Functions
- Variables
- JIT Interpreter
- Comments
- If Else statements

## Example Code

``` bigl
// Declare variables
let x = 20;
let y = 10;
y + x;

let hello = "Hello ";
let world = "World";

// This would be a function
let add = fn(x, y) { return x + y; };
add(10, 2);
add(hello, world);

// Whitespace does not matter
let adder = fn(x){
    fn(y) { 
    x + y; 
    }};
let addTwo = adder(2);
addTwo(3);

if (10 < 5) {
    x + y;
} else {
    x * y;
}
```

## Future Features

- Loops
- Standard Library (very minimal)
