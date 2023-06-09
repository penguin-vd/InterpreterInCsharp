import time

def fib(n):
    a = 0.0
    b = 1.0
    for _ in range(0, n):
        (a, b) = (b, a + b)
    return a

def run(f, i):
    t0 = time.time(); f(i); t1 = time.time()
    print((t1 - t0) * 1000)

run(fib, 10000000)