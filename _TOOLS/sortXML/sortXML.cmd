@echo off

set DutchLocation=D:\GitRepository\MIUI-Dutch\Translations-Dutch\Dutch

for /f %%f in ('dir /b /s %DutchLocation%\strings.xml') do (
echo %%f
sortXML.exe %%f
)
echo sort XML is READY!
pause