@echo off
csc /out:asm2.exe /t:exe Assem.cs PVMAsm.cs PVMInLine.cs Library.cs /warn:2
