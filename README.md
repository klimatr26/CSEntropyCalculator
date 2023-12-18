# CSEntropyCalculator
This is a C# program that will calculate the [entropy](https://en.wikipedia.org/wiki/Entropy_(information_theory)) of files.

Higher values (max 8.000) mean that the file is more random and less compressible, while lower values (min 0.000) mean less randomness and more compressibility.
As an example, this value is used in the [CSC compression algorithm](https://github.com/fusiyuan2010/CSC/blob/master/src/libcsc/csc_analyzer.cpp) to determine the compression method for files.

The code will work on the .NET Framework 2.0+ (with unsafe code enabled). If you are using .NET (Core), you can replace the line `int* count = stackalloc int[256];` with `Span<int> count = stackalloc int[256];` and get rid of the `unsafe` keyword. Alternatively, you can replace that line with `int[] count = new int[256];` to also get rid of the `unsafe` keyword, but performance may be impacted.

## Usage
You can download the program from [releases](https://github.com/klimatr26/CSEntropyCalculator/releases).

You can calculate the entropy of a sigle or several files:

    CSEntropyCalculator myfile.ext
    CSEntropyCalculator *.exe file*.dat

You can also export those results to a CSV file using `c` as the first argument to the program:

    CSEntropyCalculator c file1.exe file2.dat

This will save the results to `EntropyResults.csv`.

You can also use `CSEntropyCalculator myfile.ext > results.txt` to save the printed results to a text file.
