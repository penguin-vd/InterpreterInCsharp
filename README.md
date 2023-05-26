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

## If Statement Example

``` bigl
// A quick program with if statements
let name = read("Please enter your name: ")
let getAge = fn() {
    let age = read("Please enter your age: ")
    if (isDigit(age)) {
        return toInt(age);
    } else { println("Please enter a digit!"); return getAge(); }
}

let age = getAge();
println("Name: " + name + ", Age: " + toStr(age));

```

## Future Features

- Loops
- Standard Library (very minimal)
- Arrays
