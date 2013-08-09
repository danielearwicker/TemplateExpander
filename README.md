TemplateExpander
================

Extremely little language consisting of operators that perform stack manipulation, maybe useful for simple 
text re-formatting

Released under standard MIT license.

Copyright (c) 2013 Daniel Earwicker

Usage
-----

See NUnit tests for Interpreter class for several examples. Broadly speaking, the language is space-separated 
tokens, and the tokens manipulate a stack of values, so it's like a very simple FORTH. The stack values are
potentially any CLR types. It's easy to define your own custom operators (the code that defines the built-in
operators is very short).

"A test" @upper
=> A TEST

"Another test" 7 @right
=> er test

The named built-in operators are prefixed with @, so you can use other names freely for your own operators.

The result of an interpratation is just the resulting stack. The built-in operator @join will concatenate the
contents of the stack into a single string.

The exception handling is damn lazy. (Sorry about that.)
