# InterpreterInCsharp

## Features

- Int
- Bool
- String
- Float
- Functions
- Variables
- JIT Interpreter
- Comments
- If Else statements
- Include
- Arrays
- Hashes
- For loop
- While loop

## Example Code

### example.bigl

``` bigl
include("functions")

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

// Read input from a user
let name = read("Please enter your name: ")
println("Hello, " + name + "!");

// Arrays
let a = [1, 2, 3, 4];
let b = push(a, 5);
println(a);
println(b);
println(map(b, fn (x) { x * 2 }));

// Hashes
let person = {"name": name, "age": 69};
println(person);

// Assign features
let player = {"name": name, "health": 20, "take_damage": fn (x) {player["health"] -= x}, "take_big_damage": fn (x) { player["health"] -= x * 2;} };
println("Name: " + player["name"] + ", Health: " + toStr(player["health"]));
player["take_damage"](2);
println("Name: " + player["name"] + ", Health: " + toStr(player["health"]));
player["take_big_damage"](2);
println("Name: " + player["name"] + ", Health: " + toStr(player["health"]));

// For loops
let sum = 0;
for(i in range(1, 100)) {
    sum = sum + i;
}
println(sum);

// Nested For loops
let adj = ["red", "big", "tasty"]
let fruits = ["apple", "banana", "cherry"]

for (x in adj) {
    for (y in fruits) {
        println(x + " " + y);
    }
}

// While loops
while (true) {
    let input = read("Enter 'exit' to exit: ")
    if (input == "exit") {
        break;
    }
    println("that is not exit!!");
}
```

## Include Example

### main.bigl

``` bigl
include("functions")

// A quick program with include
let name = read("Please enter your name: ")
let age = getAge();
println("Name: " + name + ", Age: " + toStr(age));
```

### functions.bigl

``` bigl
let getAge = fn() {
    let age = read("Please enter your age: ")
    if (isDigit(age)) {
        return toInt(age);
    } else { println("Please enter a digit!"); return getAge(); }
}

let map = fn(arr, f) {
    let iter = fn(arr, accumulated) {
        if (len(arr) == 0) {
            accumulated;
        } else {
            iter(rest(arr), push(accumulated, f(first(arr))));
        }
    };
    iter(arr, []);
};

let main = fn() {
    println("This is not call in the function")
}

if (_name == "_main") {
    main();
}
```
