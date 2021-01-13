@echo off
csc /out:asm1.exe /t:exe Assem.cs PVMAsm.cs PVMPushPop.cs Library.cs /warn:2
