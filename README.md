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
println(y + x); // You can comment inline

// This would be a function
let add = fn(x, y) { return x + y; };
println(add(10, 2));

// Whitespace does not matter
let adder = fn(x){
    fn(y) {
    x + y;
    }};
let addTwo = adder(2);
println(addTwo(3));

// This function would als work with strings
let hello = adder("Hello, ");
println(hello("World!"));
```

## Future Features

- Loops
- Standard Library (very minimal)
