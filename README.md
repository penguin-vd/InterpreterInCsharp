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
- Include

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

// The function types do not matter
let hello = adder("Hello, ");
println(hello("World!"));
```

## Include Example

### main.bigl

``` bigl
include("getage") // reference could also be getage.bigl

// A quick program with include
let name = read("Please enter your name: ")
let age = getAge();
println("Name: " + name + ", Age: " + toStr(age));
```

### getage.bigl

``` bigl
let getAge = fn() {
    let age = read("Please enter your age: ")
    if (isDigit(age)) {
        return toInt(age);
    } else { println("Please enter a digit!"); return getAge(); }
}

let main = fn() {
    println("This is not call in the function")
}

if (_name == "_main") {
    main();
}
```

## Future Features

- Loops
- Standard Library (very minimal)
- Arrays
