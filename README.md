# MemgrindDifferencer
Basic tool to track differences between memgrind (a valgrind tool) runs

What It Does
============

Takes two (or more) valgrind files and attempts to cross-correlate leaks, errors, etc., so you can see the evolution
between files. I've used this for short vs. long runs, and for comparing runs after a supposed "fix". :-)

Development
===========
Requires VS 2013. Everything needed should come down with nuget.
