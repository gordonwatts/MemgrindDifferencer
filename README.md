# MemgrindDifferencer
Basic tool to track differences between memgrind (a valgrind tool) runs

What It Does
============

Takes two (or more) valgrind files and attempts to cross-correlate leaks, errors, etc., so you can see the evolution
between files. I've used this for short vs. long runs, and for comparing runs after a supposed "fix". :-)

Out put is in a large Excel file, which can then be sorted and otherwise manipulated.

Installation
============

- Build it from sources (see Development section below)
- choc install valgrindparsetools

Usage
=====

These tools use a library (in this project) that reads memcheck log files. Make sure the log files contain valgrind messages
that start with "== xxxx ==" otherwise they will not be recognized.

memgrind_diff
=============

memgrind_diff <input-file-1.log> <input-file-2.log>

Will write out an excel file which contains the info from the log files. It will attempt to find different memory leaks and problems in each file and
correlate them in the excel log file.

In general I find it useful to shrink the column that has the crash dumps to 8 pt font, and use a very large screen.

memgrind_dump
=============

Looks only at memory leaks. It will extract each method name (tuned for C++) from each memory leak, and sum each time it appears by # of blocks
lost and # of bytes lost. You can then look at the resulting file and sort by size to see what is what.

Development
===========
Requires VS 2013. Everything needed should come down with nuget.

License
=======
MIT: http://choosealicense.com/licenses/mit/